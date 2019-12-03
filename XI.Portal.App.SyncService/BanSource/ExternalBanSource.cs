using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Serilog;
using XI.Portal.App.SyncService.Extensions;
using XI.Portal.Library.CommonTypes;

namespace XI.Portal.App.SyncService.BanSource
{
    public class ExternalBanSource : IExternalBanSource
    {
        private readonly IFileSystem fileSystem;
        private readonly ILogger logger;

        public ExternalBanSource(ILogger logger, IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public ICollection<string> GetBans(GameType gameType)
        {
            if (!UpToDateBanFileExists(gameType)) GenerateBanFile(gameType);

            var dataPath = gameType.ExternalDataPath();

            var entries = new List<string>();

            foreach (var line in fileSystem.File.ReadAllLines(dataPath))
                if (!entries.Contains(line))
                    entries.Add(line);

            return entries;
        }

        private bool UpToDateBanFileExists(GameType gameType)
        {
            var dataPath = gameType.ExternalDataPath();
            var fileInfo = fileSystem.FileInfo.FromFileName(dataPath);

            return fileSystem.File.Exists(dataPath) &&
                   DateTime.UtcNow - fileInfo.LastWriteTimeUtc <= TimeSpan.FromDays(14);
        }

        private void GenerateBanFile(GameType gameType)
        {
            var sourceDataPath = gameType.ExternalSourceDataPath();
            var dataPath = gameType.ExternalDataPath();

            if (fileSystem.File.Exists(dataPath)) fileSystem.File.Delete(dataPath);

            var entries = new BlockingCollection<string>();

            Parallel.ForEach(fileSystem.File.ReadAllLines(sourceDataPath), line =>
            {
                var guid = ParseLine(line);

                if (guid != null) entries.Add(guid);
            });

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                foreach (var entry in entries) streamWriter.WriteLine(entry);
                streamWriter.Flush();
                memoryStream.Position = 0;

                var dataPathDirectory = Path.GetDirectoryName(dataPath);
                if (!fileSystem.Directory.Exists(dataPathDirectory))
                    fileSystem.Directory.CreateDirectory(dataPathDirectory);

                fileSystem.File.WriteAllBytes(dataPath, memoryStream.ToArray());
            }

            logger.Information(
                $"Created external ban file at path {dataPath} with {entries.Count} external bans");
        }

        private static string ParseLine(string line)
        {
            const string regex =
                @"\[PBBans:[0-9]{5,6}\s[0-9]{4}-[0-9]{2}-[0-9]{2}\]\s([a-zA-Z0-9]{32})\s\""(.+)\""\s\""[0-9]{1,3}\.[0-9]{1,3}\.\*\.\*\""\s.+";

            if (string.IsNullOrWhiteSpace(line)) return null;

            var regexMatch = Regex.Match(line, regex);
            return regexMatch.Success ? regexMatch.Groups[1].Value.ToLower() : null;
        }
    }
}