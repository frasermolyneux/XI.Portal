using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using XI.Portal.App.SyncService.Models;
using XI.Portal.App.SyncService.PlayerSync;
using XI.Portal.Data.CommonTypes;

namespace XI.Portal.App.SyncService.BanFiles
{
    public class BanFileImporter : IBanFileImporter
    {
        private readonly IGuidValidator guidValidator;
        private readonly ILogger logger;
        private readonly IPlayerSynchronizer playerSynchronizer;

        public BanFileImporter(ILogger logger, IGuidValidator guidValidator, IPlayerSynchronizer playerSynchronizer)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.guidValidator = guidValidator ?? throw new ArgumentNullException(nameof(guidValidator));
            this.playerSynchronizer = playerSynchronizer ?? throw new ArgumentNullException(nameof(playerSynchronizer));
        }

        public void ImportData(GameType gameType, string banData, Guid serverId)
        {
            var localPlayers = new List<LocalPlayer>();
            var skipTags = new[] { "[PBBAN]", "[B3BAN]", "[BANSYNC]", "[EXTERNAL]" };

            foreach (var line in banData.Split('\n'))
            {
                if (string.IsNullOrWhiteSpace(line) || skipTags.Any(skipTag => line.Contains(skipTag))) continue;

                ParseLine(line, out var guid, out var name);

                if (string.IsNullOrWhiteSpace(guid) || string.IsNullOrWhiteSpace(name)) continue;

                if (!guidValidator.IsValid(gameType, guid))
                {
                    logger.Error($"Could not validate guid {guid}");
                    continue;
                }

                if (localPlayers.All(p => p.Guid != guid))
                    localPlayers.Add(new LocalPlayer
                    {
                        Guid = guid,
                        Name = name
                    });
            }

            playerSynchronizer.SynchronizeLocalPlayers(gameType, localPlayers, serverId);
        }

        private void ParseLine(string line, out string guid, out string name)
        {
            try
            {
                var trimmedLine = line.Trim();
                var indexOfSpace = trimmedLine.IndexOf(' ');
                var lengthOfLine = trimmedLine.Length;

                guid = trimmedLine.Substring(0, indexOfSpace).Trim().ToLower();
                name = trimmedLine.Substring(indexOfSpace, lengthOfLine - indexOfSpace).Trim();
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Failed to parse {line}");
                guid = name = null;
            }
        }
    }
}