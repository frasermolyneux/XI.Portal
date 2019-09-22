using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Serilog;
using XI.Portal.Library.CommonTypes;
using XI.Portal.Plugins.Events;

namespace XI.Portal.Services.FileMonitorService.FtpFileMonitor
{
    public class FtpFileMonitor
    {
        private readonly ILogger logger;

        public FtpFileMonitor(ILogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private string RequestPath { get; set; }
        private string FtpUsername { get; set; }
        private string FtpPassword { get; set; }
        private Guid ServerId { get; set; }
        public string ServerName { get; set; }
        private GameType GameType { get; set; }

        private CancellationTokenSource CancellationTokenSource { get; set; }

        private long BytesRead { get; set; }

        public void Configure(string requestPath, string ftpUsername, string ftpPassword, Guid gameServerId, string serverName, GameType gameType, CancellationTokenSource cancellationTokenSource)
        {
            RequestPath = requestPath;
            FtpUsername = ftpUsername;
            FtpPassword = ftpPassword;
            ServerId = gameServerId;
            ServerName = serverName;
            GameType = gameType;
            CancellationTokenSource = cancellationTokenSource;

            var monitorThread = new Thread(MonitorLog);
            monitorThread.Start();
        }

        public event EventHandler LineRead;

        protected virtual void OnLineRead(LineReadEventArgs eventArgs)
        {
            LineRead?.Invoke(this, eventArgs);
        }

        private void MonitorLog()
        {
            try
            {
                var lastLoop = DateTime.MinValue;

                BytesRead = GetFileSize(FtpUsername, FtpPassword, RequestPath);
                logger.Information($"[{ServerId}] Setting bytes read to {BytesRead}");

                while (!CancellationTokenSource.IsCancellationRequested)
                {
                    if (DateTime.UtcNow < lastLoop.AddSeconds(5))
                    {
                        Thread.Sleep(1000);
                        continue;
                    }

                    try
                    {
                        var request = (FtpWebRequest) WebRequest.Create(RequestPath);
                        request.Credentials = new NetworkCredential(FtpUsername, FtpPassword);
                        request.ContentOffset = BytesRead;
                        request.Method = WebRequestMethods.Ftp.DownloadFile;

                        using (var stream = request.GetResponse().GetResponseStream())
                        {
                            using (var sr = new StreamReader(stream ?? throw new InvalidOperationException()))
                            {
                                var prev = -1;

                                var byteList = new List<byte>();

                                while (true)
                                {
                                    var cur = sr.Read();

                                    if (cur == -1) break;

                                    byteList.Add((byte) cur);

                                    if (prev == '\r' && cur == '\n')
                                    {
                                        BytesRead += byteList.Count;

                                        var lineReadEventArgs = new LineReadEventArgs(ServerId, ServerName, GameType, Encoding.UTF8.GetString(byteList.ToArray()).TrimEnd('\n'));

                                        OnLineRead(lineReadEventArgs);
                                        byteList = new List<byte>();
                                    }

                                    prev = cur;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex, $"[{ServerId}] Failed monitoring remote file");
                    }

                    lastLoop = DateTime.UtcNow;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"[{ServerId}] Top level error monitoring file");
            }
        }

        private static long GetFileSize(string username, string password, string requestPath)
        {
            var request = (FtpWebRequest) WebRequest.Create(requestPath);
            request.Credentials = new NetworkCredential(username, password);
            request.Method = WebRequestMethods.Ftp.GetFileSize;

            var fileSize = ((FtpWebResponse) request.GetResponse()).ContentLength;
            return fileSize;
        }
    }
}