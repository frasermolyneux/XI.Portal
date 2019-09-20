﻿using System;
using System.Linq;
using Serilog;
using XI.Portal.Data.Core.Context;
using XI.Portal.Data.Core.Models;
using XI.Portal.Library.Logging;
using XI.Portal.Services.FileMonitor.Events;
using XI.Portal.Services.FileMonitor.Interfaces;

namespace XI.Portal.Services.FileMonitor.Plugins
{
    public class PlayerCorePlugin : IPlugin
    {
        private readonly IContextProvider contextProvider;
        private readonly IDatabaseLogger databaseLogger;
        private readonly ILogger logger;

        public PlayerCorePlugin(ILogger logger, IContextProvider contextProvider, IDatabaseLogger databaseLogger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
            this.databaseLogger = databaseLogger ?? throw new ArgumentNullException(nameof(databaseLogger));
        }

        public void RegisterEventHandlers(IParser parser)
        {
            parser.PlayerConnected += Parser_PlayerConnected;
            parser.PlayerDisconnected += Parser_PlayerDisconnected;
        }

        private void Parser_PlayerConnected(object sender, EventArgs e)
        {
            var onPlayerConnectedEventArgs = (OnPlayerConnectedEventArgs) e;

            logger.Information($"[{onPlayerConnectedEventArgs.ServerId}] Player {onPlayerConnectedEventArgs.Name} connected with GUID {onPlayerConnectedEventArgs.Guid} at {DateTime.UtcNow.ToShortTimeString()}");

            EnsurePlayerExists(onPlayerConnectedEventArgs.ServerId, onPlayerConnectedEventArgs.Guid, onPlayerConnectedEventArgs.Name);
        }

        private void Parser_PlayerDisconnected(object sender, EventArgs e)
        {
            var onPlayerDisconnectedEventArgs = (OnPlayerDisconnectedEventArgs) e;

            logger.Information($"[{onPlayerDisconnectedEventArgs.ServerId}] Player {onPlayerDisconnectedEventArgs.Name} disconnected with GUID {onPlayerDisconnectedEventArgs.Guid} at {DateTime.UtcNow.ToShortTimeString()}");

            EnsurePlayerExists(onPlayerDisconnectedEventArgs.ServerId, onPlayerDisconnectedEventArgs.Guid, onPlayerDisconnectedEventArgs.Name);
        }

        private void EnsurePlayerExists(Guid serverId, string guid, string name)
        {
            if (guid == "allies" || guid == "axis")
                throw new Exception("Guid cannot be allies or axis - invalid");

            using (var context = contextProvider.GetContext())
            {
                var gameServer = context.GameServers.Single(s => s.ServerId == serverId);

                var player = context.Players.SingleOrDefault(p =>
                    p.Guid == guid && p.GameType == gameServer.GameType);

                if (player == null)
                {
                    var playerToAdd = new Player2
                    {
                        GameType = gameServer.GameType,
                        Username = name,
                        Guid = guid,
                        FirstSeen = DateTime.UtcNow,
                        LastSeen = DateTime.UtcNow
                    };

                    context.Players.Add(playerToAdd);
                    databaseLogger.CreateSystemLogAsync("Info", $"New player created for {gameServer.GameType} with username {name} and guid {guid}");
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
    }
}