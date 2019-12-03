using System;
using System.IO;
using System.Net;
using System.Text;
using Serilog;
using Unity;
using XI.Portal.Data.Core.Models;

namespace XI.Portal.App.SyncService.Extensions
{
    public static class BanSyncExtensions
    {
        private static ILogger Logger => UnityContainerBox.UnityContainer.Resolve<ILogger>();

        private static WebRequest CreateWebRequest(BanFileMonitor banFileMonitor)
        {
            var ftpHostname = banFileMonitor.GameServer.FtpHostname;
            var ftpUsername = banFileMonitor.GameServer.FtpUsername;
            var ftpPassword = banFileMonitor.GameServer.FtpPassword;

            var requestPath = $"ftp://{ftpHostname}{banFileMonitor.FilePath}";

            Logger.Information($"Request Path: {requestPath}");

            var request = WebRequest.Create(new Uri(requestPath));
            request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
            return request;
        }

        public static long GetRemoteBanFileSize(this BanFileMonitor banFileMonitor)
        {
            var request = CreateWebRequest(banFileMonitor);
            request.Method = WebRequestMethods.Ftp.GetFileSize;

            using (var webResponse = request.GetResponse())
            {
                return webResponse.ContentLength;
            }
        }

        public static string GetRemoteBanData(this BanFileMonitor banFileMonitor)
        {
            var request = CreateWebRequest(banFileMonitor);
            request.Method = WebRequestMethods.Ftp.DownloadFile;

            using (var response = (FtpWebResponse)request.GetResponse())
            {
                using (var responseStream = response.GetResponseStream())
                {
                    using (var streamReader = new StreamReader(responseStream))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
        }

        public static void UpdateFromLocal(this BanFileMonitor banFileMonitor, string dataPath)
        {
            var request = CreateWebRequest(banFileMonitor);
            request.Method = WebRequestMethods.Ftp.UploadFile;

            using (var streamReader = new StreamReader(dataPath))
            {
                var fileContents = Encoding.UTF8.GetBytes(streamReader.ReadToEnd());

                using (var requestStream = request.GetRequestStream())
                {
                    requestStream.Write(fileContents, 0, fileContents.Length);
                }
            }
        }
    }
}