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

        public string Hostname { get; private set; }
        public int QueryPort { get; private set; }
        public string RconPassword { get; private set; }

        public void Configure(string hostname, int queryPort, string rconPassword)
        {
            Hostname = hostname;
            QueryPort = queryPort;
            RconPassword = rconPassword;
        }

        public virtual string PlayerStatus()
        {
            logger.Warning("PlayerStatus not currently implemented");
            return string.Empty;
        }

        public virtual string KickPlayer(string targetPlayerNum)
        {
            logger.Warning("KickPlayer not currently implemented");
            return string.Empty;
        }

        public virtual string BanPlayer(string targetPlayerNum)
        {
            logger.Warning("BanPlayer not currently implemented");
            return string.Empty;
        }

        public virtual string RestartServer()
        {
            logger.Warning("RestartServer not currently implemented");
            return string.Empty;
        }

        public virtual string RestartMap()
        {
            logger.Warning("RestartMap not currently implemented");
            return string.Empty;
        }

        public virtual string NextMap()
        {
            logger.Warning("NextMap not currently implemented");
            return string.Empty;
        }

        public virtual string MapRotation()
        {
            logger.Warning("MapRotation not currently implemented");
            return string.Empty;
        }

        public virtual string Say(string message)
        {
            logger.Warning("Say not currently implemented");
            return string.Empty;
        }
    }
}