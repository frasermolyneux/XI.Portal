using System;
using System.IO;
using System.IO.Abstractions;
using Serilog;
using XI.Portal.App.SyncService.BanSource;
using XI.Portal.App.SyncService.Extensions;
using XI.Portal.Library.CommonTypes;

namespace XI.Portal.App.SyncService.BanFiles
{
    public class LocalBanFileManager : ILocalBanFileManager
    {
        private readonly IDatabaseBanSource databaseBanSource;
        private readonly IExternalBanSource externalBanSource;
        private readonly IFileSystem fileSystem;
        private readonly ILogger logger;

        public LocalBanFileManager(ILogger logger, IFileSystem fileSystem, IDatabaseBanSource databaseBanSource,
            IExternalBanSource externalBanSource)
        {
            this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.databaseBanSource = databaseBanSource ?? throw new ArgumentNullException(nameof(databaseBanSource));
            this.externalBanSource = externalBanSource ?? throw new ArgumentNullException(nameof(externalBanSource));
        }

        public bool UpToDateBanFileExists(GameType gameType)
        {
            var dataPath = gameType.DataPath();
            var fileInfo = fileSystem.FileInfo.FromFileName(dataPath);

            var fileExists = fileSystem.File.Exists(dataPath);

            return fileExists && DateTime.UtcNow - fileInfo.LastWriteTimeUtc <=
                   TimeSpan.FromHours(1);
        }

        public void GenerateBanFileIfRequired(GameType gameType)
        {
            if (!UpToDateBanFileExists(gameType))
                GenerateBanFile(gameType);
            else
                logger.Information($"Ban file regeneration not required for {gameType}");
        }

        public long GetLocalBanFileSize(GameType gameType)
        {
            var dataPath = gameType.DataPath();
            var fileInfo = fileSystem.FileInfo.FromFileName(dataPath);

            return fileInfo.Length;
        }

        public void GenerateBanFile(GameType gameType)
        {
            var dataPath = gameType.DataPath();

            if (fileSystem.File.Exists(dataPath)) fileSystem.File.Delete(dataPath);

            var databaseBans = databaseBanSource.GetBans(gameType);
            var externalBans = externalBanSource.GetBans(gameType);

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                foreach (var externalBan in externalBans) streamWriter.WriteLine($"{externalBan} [EXTERNAL]");

                foreach (var databaseBan in databaseBans)
                    streamWriter.WriteLine($"{databaseBan.Key} [BANSYNC]-{databaseBan.Value}");

                streamWriter.Flush();
                memoryStream.Position = 0;

                var dataPathDirectory = Path.GetDirectoryName(dataPath);
                if (!fileSystem.Directory.Exists(dataPathDirectory))
                    fileSystem.Directory.CreateDirectory(dataPathDirectory);

                fileSystem.File.WriteAllBytes(dataPath, memoryStream.ToArray());
            }

            logger.Information(
                $"Created ban file at path {dataPath} with {databaseBans.Count} database bans and {externalBans.Count} external bans");
        }
    }
}