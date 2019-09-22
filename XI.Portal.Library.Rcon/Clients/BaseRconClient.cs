using System;
using Serilog;
using XI.Portal.Library.Rcon.Interfaces;

namespace XI.Portal.Library.Rcon.Clients
{
    public class BaseRconClient : IRconClient
    {
        private readonly ILogger logger;

        public BaseRconClient(ILogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string ServerName { get; set; }
        public string Hostname { get; private set; }
        public int QueryPort { get; private set; }
        public string RconPassword { get; private set; }

        public void Configure(string serverName, string hostname, int queryPort, string rconPassword)
        {
            ServerName = serverName;
            Hostname = hostname;
            QueryPort = queryPort;
            RconPassword = rconPassword;
        }

        public virtual string PlayerStatus()
        {
            logger.Warning("[{serverName}] PlayerStatus not currently implemented", ServerName);
            return string.Empty;
        }

        public virtual string KickPlayer(string targetPlayerNum)
        {
            logger.Warning("[{serverName}] KickPlayer not currently implemented", ServerName);
            return string.Empty;
        }

        public virtual string BanPlayer(string targetPlayerNum)
        {
            logger.Warning("[{serverName}] BanPlayer not currently implemented", ServerName);
            return string.Empty;
        }

        public virtual string RestartServer()
        {
            logger.Warning("[{serverName}] RestartServer not currently implemented", ServerName);
            return string.Empty;
        }

        public virtual string RestartMap()
        {
            logger.Warning("[{serverName}] RestartMap not currently implemented", ServerName);
            return string.Empty;
        }

        public virtual string NextMap()
        {
            logger.Warning("[{serverName}] NextMap not currently implemented", ServerName);
            return string.Empty;
        }

        public virtual string MapRotation()
        {
            logger.Warning("[{serverName}] MapRotation not currently implemented", ServerName);
            return string.Empty;
        }

        public virtual string Say(string message)
        {
            logger.Warning("[{serverName}] Say not currently implemented", ServerName);
            return string.Empty;
        }
    }
}