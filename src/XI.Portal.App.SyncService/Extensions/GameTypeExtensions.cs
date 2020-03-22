using System;
using System.IO;
using Unity;
using XI.Portal.App.SyncService.Configuration;
using XI.Portal.Data.CommonTypes;

namespace XI.Portal.App.SyncService.Extensions
{
    public static class GameTypeExtensions
    {
        private static IConfigurationWrapper ConfigurationWrapper =>
            UnityConfig.Container.Resolve<IConfigurationWrapper>();

        public static string DataPath(this GameType gameType)
        {
            var dataPathRoot = ConfigurationWrapper.GetAppSetting("DataPathRoot");
            return Path.Combine(dataPathRoot, $"{gameType}-combined.txt");
        }

        public static string ExternalDataPath(this GameType gameType)
        {
            var dataPathRoot = ConfigurationWrapper.GetAppSetting("DataPathRoot");
            return Path.Combine(dataPathRoot, $"{gameType}-external.txt");
        }

        public static string ExternalSourceDataPath(this GameType gameType)
        {
            return Path.Combine(Environment.CurrentDirectory, "DataFiles", $"{gameType}.dat");
        }

        public static string GuidRegex(this GameType gameType)
        {
            switch (gameType)
            {
                case GameType.CallOfDuty2:
                    return @"^([a-z0-9]{4,32})$";
                case GameType.CallOfDuty4:
                    return @"^([a-z0-9]{32})$";
                case GameType.CallOfDuty5:
                    return @"^([a-z0-9]{4,32})$";
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameType), gameType, null);
            }
        }
    }
}