using System;
using System.IO;
using System.Net;
using XI.Portal.Library.CommonTypes;
using XI.Portal.Library.GameTracker.Extensions;
using XI.Portal.Services.GameTracker.Interfaces;

namespace XI.Portal.Services.GameTracker.Repositories
{
    public class MapImageRepository : IMapImageRepository
    {
        public MapImageRepository()
        {
            if (!Directory.Exists(FileStoreBasePath)) Directory.CreateDirectory(FileStoreBasePath);

            var noImageFile = Path.Combine(FileStoreBasePath, "NoMapImage.jpg");
            if (!File.Exists(noImageFile)) Properties.Resources.noimage.Save(noImageFile);
        }

        public string FileStoreBasePath { get; set; } = @"C:\Temp\GameTrackerImages";

        public string GetMapFilePath(GameType gameType, string mapName)
        {
            var mapFilePath = Path.Combine(FileStoreBasePath, $"{gameType}_{mapName}.jpg");
            if (File.Exists(mapFilePath)) return mapFilePath;

            try
            {
                var gameTrackerImageUrl = $"https://image.gametracker.com/images/maps/160x120/{gameType.ToGameTrackerGameType()}/{mapName}.jpg";

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                using (var client = new WebClient())
                {
                    client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36");
                    client.DownloadFile(new Uri(gameTrackerImageUrl), mapFilePath);
                }

                return mapFilePath;
            }
            catch
            {
                return Path.Combine(FileStoreBasePath, "NoMapImage.jpg");
            }
        }
    }
}