﻿using System;
using System.Threading;
using Serilog;
using XI.Portal.App.RconMonitorService.Interfaces;
using XI.Portal.Data.CommonTypes;
using XI.Portal.Library.Rcon.Interfaces;
using XI.Portal.Plugins.Events;

namespace XI.Portal.App.RconMonitorService.RconMonitors
{
    internal class BaseRconMonitor : IRconMonitor
    {
        private readonly ILogger logger;
        private readonly IRconClientFactory rconClientFactory;

        public BaseRconMonitor(ILogger logger, IRconClientFactory rconClientFactory)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.rconClientFactory = rconClientFactory ?? throw new ArgumentNullException(nameof(rconClientFactory));
        }

        public Guid MonitorId { get; private set; }
        public Guid ServerId { get; private set; }
        public string ServerName { get; private set; }
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

        public void Configure(Guid monitorId, Guid serverId, string serverName, GameType gameType, string hostname, int port, string rconPassword, bool monitorMapRotation, bool monitorPlayers, bool monitorPlayerIPs, CancellationTokenSource cancellationTokenSource)
        {
            MonitorId = monitorId;
            ServerId = serverId;
            ServerName = serverName;
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

        public virtual void GetMapRotation()
        {
            LastMapRotation = DateTime.UtcNow;
            logger.Information("[{serverName}] Last Map Rotation set to {LastMapRotation}", ServerName, LastMapRotation);
        }

        public virtual void GetStatus()
        {
            LastStatus = DateTime.UtcNow;
            logger.Information("[{serverName}] Last Status set to {LastStatus}", ServerName, LastStatus);
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
                        if (LastMapRotation < DateTime.UtcNow.AddMinutes(-5))
                            try
                            {
                                GetMapRotation();
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex, "[{serverName}] Error getting map rotation", ServerName);
                            }

                    if (MonitorPlayers || MonitorPlayerIPs)
                        if (LastStatus < DateTime.UtcNow.AddMinutes(-1))
                            try
                            {
                                GetStatus();
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex, "[{serverName}] Error getting status", ServerName);
                            }

                    lastLoop = DateTime.UtcNow;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "[{serverName}] Top level error monitoring server", ServerName);
            }
        }

#pragma warning disable 67
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
#pragma warning restore 67
    }
}