using System;
using System.Threading;
using Serilog;
using XI.Portal.Library.CommonTypes;
using XI.Portal.Plugins.Events;
using XI.Portal.Services.RconMonitorService.Interfaces;

namespace XI.Portal.Services.RconMonitorService.RconMonitors
{
    internal class BaseRconMonitor : IRconMonitor
    {
        private readonly ILogger logger;

        public BaseRconMonitor(ILogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Guid ServerId { get; private set; }
        public GameType GameType { get; private set; }
        public string Hostname { get; private set; }
        public int Port { get; private set; }
        public string RconPassword { get; private set; }
        public bool MonitorMapRotation { get; set; }
        public bool MonitorPlayers { get; set; }
        public bool MonitorPlayerIPs { get; set; }

        public DateTime LastMapRotation { get; set; } = DateTime.MinValue;
        public DateTime LastStatus { get; set; } = DateTime.MinValue;

        private CancellationTokenSource CancellationTokenSource { get; set; }

        public event EventHandler LineRead;
        public event EventHandler PlayerConnected;
        public event EventHandler PlayerDisconnected;
        public event EventHandler ChatMessage;
        public event EventHandler Kill;
        public event EventHandler TeamKill;
        public event EventHandler Suicide;
        public event EventHandler RoundStart;
        public event EventHandler Action;
        public event EventHandler Damage;
        public event EventHandler MapRotationRconResponse;
        public event EventHandler StatusRconResponse;

        public void Configure(Guid serverId, GameType gameType, string hostname, int port, string rconPassword, bool monitorMapRotation, bool monitorPlayers, bool monitorPlayerIPs, CancellationTokenSource cancellationTokenSource)
        {
            ServerId = serverId;
            GameType = gameType;
            Hostname = hostname;
            Port = port;
            RconPassword = rconPassword;
            MonitorMapRotation = monitorMapRotation;
            MonitorPlayers = monitorPlayers;
            MonitorPlayerIPs = monitorPlayerIPs;
            CancellationTokenSource = cancellationTokenSource;

            var monitorThread = new Thread(MonitorServer);
            monitorThread.Start();
        }

        protected virtual void OnMapRotationRconResponse(OnMapRotationRconResponse eventArgs)
        {
            MapRotationRconResponse?.Invoke(this, eventArgs);
        }

        protected virtual void OnStatusRconResponse(OnStatusRconResponse eventArgs)
        {
            StatusRconResponse?.Invoke(this, eventArgs);
        }

        private void MonitorServer()
        {
            try
            {
                var lastLoop = DateTime.MinValue;

                while (!CancellationTokenSource.IsCancellationRequested)
                {
                    if (DateTime.UtcNow < lastLoop.AddSeconds(5))
                    {
                        Thread.Sleep(1000);
                        continue;
                    }

                    if (MonitorMapRotation)
                    {
                        if (LastMapRotation < DateTime.UtcNow.AddMinutes(-5))
                        {
                            GetMapRotation();
                        }
                    }

                    if (MonitorPlayers)
                    {
                        if (LastStatus < DateTime.UtcNow.AddMinutes(-1))
                        {
                            GetStatus();
                        }
                    }

                    lastLoop = DateTime.UtcNow;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"[{ServerId}] Top level error monitoring file");
            }
        }

        public virtual void GetMapRotation()
        {
            LastMapRotation = DateTime.UtcNow;
            logger.Information($"[{ServerId}] Last Map Rotation set to {LastMapRotation}");
        }

        public virtual void GetStatus()
        {
            LastStatus = DateTime.UtcNow;
            logger.Information($"[{ServerId}] Last Status set to {LastStatus}");
        }
    }
}