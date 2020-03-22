using System;
using System.Linq;
using System.Text.RegularExpressions;
using FM.GeoLocation.Client;
using Serilog;
using XI.Portal.Data.CommonTypes;
using XI.Portal.Data.Core.Context;
using XI.Portal.Data.Core.Models;
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
        private readonly IGeoLocationClient geoLocationClient;
        private readonly ILogger logger;
        private readonly IPlayerCaching playerCaching;

        public PlayerInfoPlugin(ILogger logger,
            IContextProvider contextProvider,
            IDatabaseLogger databaseLogger,
            IGeoLocationClient geoLocationClient,
            IPlayerCaching playerCaching)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
            this.databaseLogger = databaseLogger ?? throw new ArgumentNullException(nameof(databaseLogger));
            this.geoLocationClient = geoLocationClient ?? throw new ArgumentNullException(nameof(geoLocationClient));
            this.playerCaching = playerCaching ?? throw new ArgumentNullException(nameof(playerCaching));
        }

        public void RegisterEventHandlers(IPluginEvents events)
        {
            events.PlayerConnected += Parser_PlayerConnected;
            events.PlayerDisconnected += Parser_PlayerDisconnected;
            events.StatusRconResponse += Events_StatusRconResponse;
        }

        private void Parser_PlayerConnected(object sender, EventArgs e)
        {
            var eventArgs = (OnPlayerConnectedEventArgs) e;

            logger.Information("[{serverName}] Player {name} connected with GUID {guid} at {timestamp}",
                eventArgs.ServerName, eventArgs.Name, eventArgs.Guid, DateTime.UtcNow);

            EnsurePlayerExists(eventArgs.GameType, eventArgs.Guid, eventArgs.Name, eventArgs.ServerName);
        }

        private void Parser_PlayerDisconnected(object sender, EventArgs e)
        {
            var eventArgs = (OnPlayerDisconnectedEventArgs) e;

            logger.Information("[{serverName}] Player {name} disconnected with GUID {guid} at {timestamp}",
                eventArgs.ServerName, eventArgs.Name, eventArgs.Guid, DateTime.UtcNow);

            EnsurePlayerExists(eventArgs.GameType, eventArgs.Guid, eventArgs.Name, eventArgs.ServerName);
        }

        private void Events_StatusRconResponse(object sender, EventArgs e)
        {
            var eventArgs = (OnStatusRconResponse) e;

            logger.Debug(eventArgs.ResponseData);

            switch (eventArgs.GameType)
            {
                case GameType.CallOfDuty2:
                case GameType.CallOfDuty4:
                case GameType.CallOfDuty5:
                    HandleCodStatusRconResponse(eventArgs);
                    break;
                case GameType.Insurgency:
                    HandleInsurgencyStatusRconResponse(eventArgs);
                    break;
                default:
                    logger.Warning("[{serverName}] Invalid game type {gameType} for Status Rcon Response",
                        eventArgs.ServerName, eventArgs.GameType);
                    break;
            }
        }

        private void HandleCodStatusRconResponse(OnStatusRconResponse eventArgs)
        {
            Regex regex;
            switch (eventArgs.GameType)
            {
                case GameType.CallOfDuty2:
                    regex = new Regex(
                        "^\\s*([0-9]+)\\s+([0-9-]+)\\s+([0-9]+)\\s+([0-9]+)\\s+(.*?)\\s+([0-9]+?)\\s*((?:(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])\\.){3}(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])):?(-?[0-9]{1,5})\\s*(-?[0-9]{1,5})\\s+([0-9]+)$");
                    break;
                case GameType.CallOfDuty4:
                    regex = new Regex(
                        "^\\s*([0-9]+)\\s+([0-9-]+)\\s+([0-9]+)\\s+([0-9a-f]+)\\s+(.*?)\\s+([0-9]+?)\\s*((?:(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])\\.){3}(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])):?(-?[0-9]{1,5})\\s*(-?[0-9]{1,5})\\s+([0-9]+)$");
                    break;
                case GameType.CallOfDuty5:
                    regex = new Regex(
                        "^\\s*([0-9]+)\\s+([0-9-]+)\\s+([0-9]+)\\s+([0-9]+)\\s+(.*?)\\s+([0-9]+?)\\s*((?:(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])\\.){3}(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])):?(-?[0-9]{1,5})\\s*(-?[0-9]{1,5})\\s+([0-9]+)$");
                    break;
                default:
                    logger.Error("[{serverName}] Invalid game type {gameType} for Status Rcon Response",
                        eventArgs.ServerName, eventArgs.GameType);
                    throw new Exception($"Invalid game type {eventArgs.GameType} for Status Rcon Response");
            }

            var lines = eventArgs.ResponseData.Split('\n').Where(line => !string.IsNullOrWhiteSpace(line)).ToList();

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

                    if (eventArgs.MonitorPlayers)
                        EnsurePlayerExists(eventArgs.GameType, guid, name, eventArgs.ServerName);

                    if (eventArgs.MonitorPlayerIPs)
                    {
                        var player =
                            context.Players.SingleOrDefault(p => p.Guid == guid && p.GameType == eventArgs.GameType);

                        if (player == null)
                            continue;

                        UpdatePlayerIpAddress(context, ipAddress, player, eventArgs.ServerName);
                    }

                    UpdateLivePlayerIpAddress(context, ipAddress, eventArgs.ServerName);
                }

                ReduceCaches(context);
            }
        }

        private void HandleInsurgencyStatusRconResponse(OnStatusRconResponse eventArgs)
        {
            var regex = new Regex(
                "^\\#\\s([0-9]+)\\s([0-9]+)\\s\\\"(.+)\\\"\\s([STEAM0-9:_]+)\\s+([0-9:]+)\\s([0-9]+)\\s([0-9]+)\\s([a-z]+)\\s([0-9]+)\\s((?:(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])\\.){3}(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])):?(-?[0-9]{1,5})");

            var lines = eventArgs.ResponseData.Split('\n').Where(line => !string.IsNullOrWhiteSpace(line)).ToList();

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

                    if (eventArgs.MonitorPlayers)
                        EnsurePlayerExists(eventArgs.GameType, guid, name, eventArgs.ServerName);

                    if (eventArgs.MonitorPlayerIPs)
                    {
                        var player =
                            context.Players.SingleOrDefault(p => p.Guid == guid && p.GameType == eventArgs.GameType);

                        if (player == null)
                            continue;

                        UpdatePlayerIpAddress(context, ipAddress, player, eventArgs.ServerName);
                    }

                    UpdateLivePlayerIpAddress(context, ipAddress, eventArgs.ServerName);
                }

                ReduceCaches(context);
            }
        }

        private void ReduceCaches(PortalContext context)
        {
            var livePlayerLocationCutOff = DateTime.UtcNow.AddDays(-1);
            context.LivePlayerLocations.RemoveRange(
                context.LivePlayerLocations.Where(lpl => lpl.LastSeen < livePlayerLocationCutOff));
            context.SaveChanges();

            playerCaching.ReduceCache();
        }

        private void EnsurePlayerExists(GameType gameType, string guid, string name, string serverName)
        {
            if (guid == "allies" || guid == "axis")
            {
                logger.Warning("[{serverName}] Cannot have guid {guid} for {gameType}", serverName, guid, gameType);
                return;
            }

            if (playerCaching.PlayerInCache(gameType, guid, name)) return;

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
                    logger.Information("[{serverName}] New player {name} with {guid} created under {gameType}",
                        serverName, name, guid, gameType);
                }
                else
                {
                    var playerAlias = context.PlayerAliases.SingleOrDefault(pa =>
                        pa.Player.PlayerId == player.PlayerId && pa.Name == name);

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

                    if (player.Username != name)
                        logger.Information("[{serverName}] New name {name} recorded for guid {guid} under {gameType}",
                            serverName, name, guid, gameType);

                    player.LastSeen = DateTime.UtcNow;
                    player.Username = name;
                }

                context.SaveChanges();

                playerCaching.AddToCache(gameType, guid, name);
            }
        }

        private void UpdateLivePlayerIpAddress(PortalContext context, string ipAddress, string serverName)
        {
            try
            {
                var ipLocation = geoLocationClient.LookupAddress(ipAddress).Result;

                if (ipLocation == null)
                {
                    var storedIpLocation =
                        context.LivePlayerLocations.SingleOrDefault(lpl => lpl.IpAddress == ipAddress);

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
                logger.Error(ex, "[{serverName}] Failed to get location for {ipAddress}", serverName, ipAddress);
            }
        }

        private void UpdatePlayerIpAddress(PortalContext context, string ipAddress, Player2 player, string serverName)
        {
            var playerIpAddress = context.PlayerIpAddresses.SingleOrDefault(pip =>
                pip.Player.PlayerId == player.PlayerId && pip.Address == ipAddress);

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
                logger.Information("[{serverName}] New ipAddress {ipAddress} recorded for {name} under {gameType}",
                    serverName, ipAddress, player.Username, player.GameType);

            player.LastSeen = DateTime.UtcNow;
            player.IpAddress = ipAddress;
        }
    }
}