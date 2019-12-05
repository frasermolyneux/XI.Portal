using Serilog;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using Unity;
using XI.Portal.Data.CommonTypes;
using XI.Portal.Data.Core.Context;
using XI.Portal.Data.Core.Models;
using XI.Portal.Services.MapRedirect.Interfaces;
using System.Threading.Tasks;

namespace XI.Portal.App.MapRedirectSync
{
    class Program
    {
        static void Main()
        {
            var container = (UnityContainer)UnityConfig.Container;
            var logger = container.Resolve<ILogger>();

            logger.Information("Map Redirect Sync Starting");

            var mapRedirectRepository = container.Resolve<IMapRedirectRepository>();
            var contextProvider = container.Resolve<IContextProvider>();

            var gamesToSync = new Dictionary<GameType, string>
            {
                {GameType.CallOfDuty4, "cod4"},
                {GameType.CallOfDuty5, "cod5"}
            };


            foreach (var game in gamesToSync)
            {
                var maps = mapRedirectRepository.GetMapEntriesForGame(game.Value);

                logger.Information("Total maps retrieved from redirect for {game} is {mapCount}", game, maps.Count);

                Parallel.ForEach(maps, mapRedirectEntry => 
                {
                    using (var context = contextProvider.GetContext())
                    {
                        var mapName = mapRedirectEntry.MapName;
                        var mapFiles = mapRedirectEntry.MapFiles
                            .Where(file => file.EndsWith(".iwd") | file.EndsWith(".ff"));

                        var map = context.Maps.Include(m => m.MapFiles).SingleOrDefault(m => m.MapName == mapName && m.GameType == game.Key);

                        if (map == null)
                        {
                            logger.Debug("[{game}] {mapName} does not exist in portal, creating with {fileCount} files", game.Key, mapName, mapFiles.Count());

                            var mapToAdd = new Map
                            {
                                GameType = game.Key,
                                MapName = mapName,
                                MapFiles = mapFiles.Select(mf => new MapFile
                                {
                                    FileName = mf
                                }).ToList()
                            };

                            context.Maps.Add(mapToAdd);
                        }
                        else
                        {
                            if (mapFiles.Count() != map.MapFiles.Count())
                            {
                                logger.Debug("[{game}] {mapName} exists in portal but file count {currentfileCount} differs to {newFileCount}", game.Key, mapName, mapFiles.Count(), map.MapFiles.Count);

                                map.MapFiles?.Clear();
                                map.MapFiles = mapFiles.Select(mf => new MapFile
                                {
                                    FileName = mf
                                }).ToList();
                            }
                            else
                            {
                                logger.Debug("[{game}] {mapName} is up to date", game.Key, mapName);
                            }
                        }

                        context.SaveChanges();
                    }
                });
            }

            logger.Information("Map Redirect Sync Completed");

#if DEBUG
            Console.WriteLine("Press ENTER to exit application");
            Console.ReadKey();
#endif
        }
    }
}
