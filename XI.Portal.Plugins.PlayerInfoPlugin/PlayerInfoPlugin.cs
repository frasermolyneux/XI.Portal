using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Serilog;
using XI.Portal.Data.Core.Context;
using XI.Portal.Data.Core.Models;
using XI.Portal.Library.CommonTypes;
using XI.Portal.Library.GeoLocation.Extensions;
using XI.Portal.Library.GeoLocation.Repository;
using XI.Portal.Library.Logging;
using XI.Portal.Plugins.Events;
using XI.Portal.Plugins.Interfaces;
using XI.Portal.Plugins.PlayerInfoPlugin.Interfaces;

namespace XI.Portal.Plugins.PlayerInfoPlugin
{
    public class PlayerInfoPlugin : IPlugin
    {
        private readonly IContextProvider contextProvider;
        private readonly IDatabaseLogger databaseLogger;
        private readonly IGeoLocationApiRepository geoLocationApiRepository;
        private readonly IIpAddressCaching ipAddressCaching;
        private readonly ILogger logger;

        public PlayerInfoPlugin(ILogger logger, IContextProvider contextProvider, IDatabaseLogger databaseLogger, IGeoLocationApiRepository geoLocationApiRepository, IIpAddressCaching ipAddressCaching)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
            this.databaseLogger = databaseLogger ?? throw new ArgumentNullException(nameof(databaseLogger));
            this.geoLocationApiRepository = geoLocationApiRepository ?? throw new ArgumentNullException(nameof(geoLocationApiRepository));
            this.ipAddressCaching = ipAddressCaching ?? throw new ArgumentNullException(nameof(ipAddressCaching));
        }

        public void RegisterEventHandlers(IPluginEvents events)
        {
            events.PlayerConnected += Parser_PlayerConnected;
            events.PlayerDisconnected += Parser_PlayerDisconnected;
            events.StatusRconResponse += Events_StatusRconResponse;
        }

        private void Parser_PlayerConnected(object sender, EventArgs e)
        {
            var onPlayerConnectedEventArgs = (OnPlayerConnectedEventArgs) e;

            logger.Information($"[{onPlayerConnectedEventArgs.ServerId}] Player {onPlayerConnectedEventArgs.Name} connected with GUID {onPlayerConnectedEventArgs.Guid} at {DateTime.UtcNow.ToShortTimeString()}");

            EnsurePlayerExists(onPlayerConnectedEventArgs.GameType, onPlayerConnectedEventArgs.Guid, onPlayerConnectedEventArgs.Name);
        }

        private void Parser_PlayerDisconnected(object sender, EventArgs e)
        {
            var onPlayerDisconnectedEventArgs = (OnPlayerDisconnectedEventArgs) e;

            logger.Information($"[{onPlayerDisconnectedEventArgs.ServerId}] Player {onPlayerDisconnectedEventArgs.Name} disconnected with GUID {onPlayerDisconnectedEventArgs.Guid} at {DateTime.UtcNow.ToShortTimeString()}");

            EnsurePlayerExists(onPlayerDisconnectedEventArgs.GameType, onPlayerDisconnectedEventArgs.Guid, onPlayerDisconnectedEventArgs.Name);
        }

        private void Events_StatusRconResponse(object sender, EventArgs e)
        {
            var onStatusRconResponseEventArgs = (OnStatusRconResponse) e;

            logger.Debug(onStatusRconResponseEventArgs.ResponseData);

            switch (onStatusRconResponseEventArgs.GameType)
            {
                case GameType.CallOfDuty2:
                case GameType.CallOfDuty4:
                case GameType.CallOfDuty5:
                    HandleCodStatusRconResponse(onStatusRconResponseEventArgs);
                    break;
                case GameType.Insurgency:
                    HandleInsurgencyStatusRconResponse(onStatusRconResponseEventArgs);
                    break;
                default:
                    logger.Warning("Invalid game type for Status Rcon Response");
                    break;
            }
        }

        private void HandleCodStatusRconResponse(OnStatusRconResponse onStatusRconResponseEventArgs)
        {
            Regex regex;
            switch (onStatusRconResponseEventArgs.GameType)
            {
                case GameType.CallOfDuty2:
                    regex = new Regex("^\\s*([0-9]+)\\s+([0-9-]+)\\s+([0-9]+)\\s+([0-9]+)\\s+(.*?)\\s+([0-9]+?)\\s*((?:(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])\\.){3}(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])):?(-?[0-9]{1,5})\\s*(-?[0-9]{1,5})\\s+([0-9]+)$");
                    break;
                case GameType.CallOfDuty4:
                    regex = new Regex("^\\s*([0-9]+)\\s+([0-9-]+)\\s+([0-9]+)\\s+([0-9a-f]+)\\s+(.*?)\\s+([0-9]+?)\\s*((?:(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])\\.){3}(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])):?(-?[0-9]{1,5})\\s*(-?[0-9]{1,5})\\s+([0-9]+)$");
                    break;
                case GameType.CallOfDuty5:
                    regex = new Regex("^\\s*([0-9]+)\\s+([0-9-]+)\\s+([0-9]+)\\s+([0-9]+)\\s+(.*?)\\s+([0-9]+?)\\s*((?:(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])\\.){3}(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])):?(-?[0-9]{1,5})\\s*(-?[0-9]{1,5})\\s+([0-9]+)$");
                    break;
                default:
                    throw new Exception("Game Type is not supported");
            }

            var lines = onStatusRconResponseEventArgs.ResponseData.Split('\n').Where(line => !string.IsNullOrWhiteSpace(line)).ToList();

            using (var context = contextProvider.GetContext())
            {
                for (var i = 3; i < lines.Count; i++)
                {
                    var line = lines[i];
                    var match = regex.Match(line);

                    if (!match.Success)
                        continue;

                    var guid = match.Groups[4].ToString();
                    var name = match.Groups[5].ToString();
                    var ipAddress = match.Groups[7].ToString();

                    if (onStatusRconResponseEventArgs.MonitorPlayers) EnsurePlayerExists(onStatusRconResponseEventArgs.GameType, guid, name);

                    if (onStatusRconResponseEventArgs.MonitorPlayerIPs)
                    {
                        var player = context.Players.SingleOrDefault(p => p.Guid == guid && p.GameType == onStatusRconResponseEventArgs.GameType);

                        if (player == null)
                            continue;

                        UpdatePlayerIpAddress(context, ipAddress, player);
                    }

                    UpdateLivePlayerIpAddress(context, ipAddress);
                }

                CleanLivePlayerIps(context);
            }
        }

        private void HandleInsurgencyStatusRconResponse(OnStatusRconResponse onStatusRconResponseEventArgs)
        {
            var regex = new Regex("^\\#\\s([0-9]+)\\s([0-9]+)\\s\\\"(.+)\\\"\\s([STEAM0-9:_]+)\\s+([0-9:]+)\\s([0-9]+)\\s([0-9]+)\\s([a-z]+)\\s([0-9]+)\\s((?:(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])\\.){3}(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])):?(-?[0-9]{1,5})");

            var lines = onStatusRconResponseEventArgs.ResponseData.Split('\n').Where(line => !string.IsNullOrWhiteSpace(line)).ToList();

            using (var context = contextProvider.GetContext())
            {
                for (var i = 7; i < lines.Count; i++)
                {
                    var line = lines[i];
                    var match = regex.Match(line);

                    if (!match.Success)
                        continue;

                    var name = match.Groups[3].ToString();
                    var guid = match.Groups[4].ToString();
                    var ipAddress = match.Groups[10].ToString();

                    if (onStatusRconResponseEventArgs.MonitorPlayers) EnsurePlayerExists(onStatusRconResponseEventArgs.GameType, guid, name);

                    if (onStatusRconResponseEventArgs.MonitorPlayerIPs)
                    {
                        var player = context.Players.SingleOrDefault(p => p.Guid == guid && p.GameType == onStatusRconResponseEventArgs.GameType);

                        if (player == null)
                            continue;

                        UpdatePlayerIpAddress(context, ipAddress, player);
                    }

                    UpdateLivePlayerIpAddress(context, ipAddress);
                }

                CleanLivePlayerIps(context);
            }
        }

        private void CleanLivePlayerIps(PortalContext context)
        {
            var livePlayerLocationCutOff = DateTime.UtcNow.AddDays(-1);
            context.LivePlayerLocations.RemoveRange(context.LivePlayerLocations.Where(lpl => lpl.LastSeen < livePlayerLocationCutOff));
            context.SaveChanges();

            ipAddressCaching.ReduceCache();
        }

        private void EnsurePlayerExists(GameType gameType, string guid, string name)
        {
            if (guid == "allies" || guid == "axis")
                throw new Exception("Guid cannot be allies or axis - invalid");

            using (var context = contextProvider.GetContext())
            {
                var player = context.Players.SingleOrDefault(p => p.Guid == guid && p.GameType == gameType);

                if (player == null)
                {
                    var playerToAdd = new Player2
                    {
                        GameType = gameType,
                        Username = name,
                        Guid = guid,
                        FirstSeen = DateTime.UtcNow,
                        LastSeen = DateTime.UtcNow
                    };

                    context.Players.Add(playerToAdd);
                    databaseLogger.CreateSystemLogAsync("Info", $"New player created for {gameType} with username {name} and guid {guid}");
                }
                else
                {
                    var playerAlias = context.PlayerAliases.SingleOrDefault(pa => pa.Player.PlayerId == player.PlayerId && pa.Name == name);

                    if (playerAlias == null)
                    {
                        playerAlias = new PlayerAlias
                        {
                            Name = name,
                            Added = DateTime.UtcNow,
                            LastUsed = DateTime.UtcNow,
                            Player = player
                        };

                        context.PlayerAliases.Add(playerAlias);
                    }
                    else
                    {
                        playerAlias.LastUsed = DateTime.UtcNow;
                    }

                    if (player.Username != name) databaseLogger.CreateSystemLogAsync("Info", $"New name {name} recorded for {player.PlayerId}");

                    player.LastSeen = DateTime.UtcNow;
                    player.Username = name;
                }

                context.SaveChanges();
            }
        }

        private void UpdateLivePlayerIpAddress(PortalContext context, string ipAddress)
        {
            try
            {
                if (ipAddressCaching.IpAddressInCache(ipAddress))
                {
                    logger.Debug("Player address {ipAddress} already exists in local cache", ipAddress);
                    return;
                }

                var ipLocation = geoLocationApiRepository.GetLocation(ipAddress).Result;

                ipAddressCaching.AddToCache(ipAddress);

                if (!ipLocation.IsDefault())
                {
                    var storedIpLocation = context.LivePlayerLocations.SingleOrDefault(lpl => lpl.IpAddress == ipAddress);

                    if (storedIpLocation == null)
                    {
                        storedIpLocation = new LivePlayerLocation
                        {
                            IpAddress = ipAddress,
                            LastSeen = DateTime.UtcNow,
                            Lat = ipLocation.Latitude,
                            Long = ipLocation.Longitude
                        };

                        context.LivePlayerLocations.Add(storedIpLocation);
                    }
                    else
                    {
                        storedIpLocation.LastSeen = DateTime.UtcNow;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "[RconMonitor] Failed to get location for {ipAddress}", ipAddress);
            }
        }

        private void UpdatePlayerIpAddress(PortalContext context, string ipAddress, Player2 player)
        {
            var playerIpAddress = context.PlayerIpAddresses.SingleOrDefault(pip => pip.Player.PlayerId == player.PlayerId && pip.Address == ipAddress);

            if (playerIpAddress == null)
            {
                playerIpAddress = new PlayerIpAddress
                {
                    Address = ipAddress,
                    Added = DateTime.UtcNow,
                    LastUsed = DateTime.UtcNow,
                    Player = player
                };

                context.PlayerIpAddresses.Add(playerIpAddress);
            }
            else
            {
                playerIpAddress.LastUsed = DateTime.UtcNow;
            }

            if (player.IpAddress != ipAddress)
                logger.Information("[RconMonitor] New IpAddress {ipAddress} recorded for {PlayerId} ({Username})", ipAddress, player.PlayerId, player.Username);

            player.LastSeen = DateTime.UtcNow;
            player.IpAddress = ipAddress;
        }
    }
}