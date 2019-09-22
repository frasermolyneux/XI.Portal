using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using XI.Portal.Data.Core.Context;
using XI.Portal.Data.Core.Models;
using XI.Portal.Library.CommonTypes;
using XI.Portal.Plugins.Events;
using XI.Portal.Plugins.Interfaces;

namespace XI.Portal.Plugins.MapRotationPlugin
{
    public class MapRotationPlugin : IPlugin
    {
        private readonly ILogger logger;
        private readonly IContextProvider contextProvider;

        public MapRotationPlugin(ILogger logger, IContextProvider contextProvider)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
        }

        public void RegisterEventHandlers(IPluginEvents events)
        {
            events.MapRotationRconResponse += Events_MapRotationRconResponse;
        }

        private void Events_MapRotationRconResponse(object sender, EventArgs e)
        {
            var eventArgs = (OnMapRotationRconResponse)e;

            logger.Debug(eventArgs.ResponseData);

            switch (eventArgs.GameType)
            {
                case GameType.CallOfDuty2:
                case GameType.CallOfDuty4:
                case GameType.CallOfDuty5:
                    HandleCodMapRotationResponse(eventArgs);
                    break;
                case GameType.Insurgency:
                    HandleSourceMapRotationResponse(eventArgs);
                    break;
            }
        }

        private void HandleSourceMapRotationResponse(OnMapRotationRconResponse eventArgs)
        {
            
        }

        private void HandleCodMapRotationResponse(OnMapRotationRconResponse eventArgs)
        {
            // gametype ftag map mp_cgc_bog gametype ftag map mp_cgc_citystreets gametype ftag map mp_strike gametype ftag map mp_carentan gametype ftag
            // map mp_coldfront gametype ftag map mp_vac_2 gametype ftag map mp_cgc_crossfire gametype ftag map mp_overgrown gametype ftag map mp_crash gametype ftag
            // map mp_convoy gametype ftag map mp_4t4hangar gametype ftag map mp_farm gametype ftag map mp_pipeline gametype ftag map mp_shipment gametype ftag
            // map mp_countdown gametype ftag map mp_backlot gametype ftag map mp_bloc gametype ftag map mp_killhouse gametype ftag map mp_bog gametype ftag
            // map mp_cargoship gametype ftag map mp_tigertown_v2"

            //gametype dm map mp_airfield map mp_asylum map mp_castle map mp_shrine map mp_courtyard map mp_dome map mp_downfall map mp_hangar
            //map mp_makin map mp_outskirts map mp_roundhouse map mp_seelow map mp_suburban map mp_makin_day map mp_kneedeep map mp_nachtfeuer map mp_subway map mp_docks
            //map mp_kwai map mp_stalingrad map mp_drum map mp_bgate map mp_vodka"

            var line = eventArgs.ResponseData;

            line = line.Replace("\"sv_mapRotation\" is: \"", "");
            line = line.Replace("^7\" default: \"^7\"", "");
            line = line.Replace("Domain is any text", "");
            line = line.Trim();

            var allParts = line.Split(' ').Where(part => !string.IsNullOrWhiteSpace(part));
            var gameTypeCount = allParts.Count(part => part.Trim() == "gametype");

            using (var context = contextProvider.GetContext())
            {
                var gameServer = context.GameServers.Single(s => s.ServerId == eventArgs.ServerId);
                var currentMapRotation = context.MapRotations.Where(mr => mr.GameServer.ServerId == gameServer.ServerId);
                context.MapRotations.RemoveRange(currentMapRotation);

                var newMapRotation = new List<MapRotation>();
                if (gameTypeCount > 1)
                {
                    var parts = line.Split(new[] { "gametype" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var part in parts)
                    {
                        var subParts = part.Split(new[] { "map" }, StringSplitOptions.RemoveEmptyEntries);
                        var gameMode = subParts.First().Trim();
                        var mapName = subParts.Skip(1).Single().Trim();

                        logger.Debug("[{serverName}] Processing {map} with {mode} under {gameType}", eventArgs.ServerName, mapName, gameMode, eventArgs.GameType);

                        var map = GetMapAndEnsureExists(context, gameServer.GameType, mapName, eventArgs.ServerName);

                        if (map == null)
                        {
                            logger.Warning("[{serverName}] Could not find map {mapName} to add to rotation for {gameType}", eventArgs.ServerName, mapName, gameServer.GameType);
                            continue;
                        }

                        newMapRotation.Add(new MapRotation
                        {
                            GameServer = gameServer,
                            Map = map,
                            GameMode = gameMode
                        });
                    }
                }
                else
                {
                    line = line.Replace("gametype", "");
                    var parts = line.Split(new[] { "map" }, StringSplitOptions.RemoveEmptyEntries);
                    var gameMode = parts.First().Trim();

                    logger.Debug("[{serverName}] Game mode is {gameMode}", eventArgs.ServerName, gameMode);

                    foreach (var part in parts.Skip(1))
                    {
                        var mapName = part.Trim();

                        logger.Debug("[{serverName}] Processing {map} with {mode} under {gameType}", eventArgs.ServerName, mapName, gameMode, eventArgs.GameType);

                        var map = GetMapAndEnsureExists(context, gameServer.GameType, mapName, eventArgs.ServerName);

                        if (map == null)
                        {
                            logger.Warning("[{serverName}] Could not find map {mapName} to add to rotation for {gameType}", eventArgs.ServerName, mapName, gameServer.GameType);
                            continue;
                        }

                        newMapRotation.Add(new MapRotation
                        {
                            GameServer = gameServer,
                            Map = map,
                            GameMode = gameMode
                        });
                    }
                }

                context.MapRotations.AddRange(newMapRotation);
                context.SaveChanges();

                logger.Information("[{serverName}] Updated map rotation with {mapCount} maps", eventArgs.ServerName, newMapRotation.Count);
            }
        }

        private Map GetMapAndEnsureExists(PortalContext context, GameType gameType, string mapName, string serverName)
        {
            try
            {
                var map = context.Maps.SingleOrDefault(m => m.MapName == mapName && m.GameType == gameType);

                if (map == null)
                {
                    logger.Information("[{serverName}] Creating map entry in database for {mapName} under {gameType}", serverName, mapName, gameType);

                    context.Maps.Add(new Map()
                    {
                        GameType = gameType,
                        MapName = mapName
                    });

                    context.SaveChanges();

                    map = context.Maps.SingleOrDefault(m => m.MapName == mapName && m.GameType == gameType);
                }

                return map;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "[{serverName}] Failed to process {map} under {game}", serverName, mapName, gameType);
                throw;
            }
        }
    }
}