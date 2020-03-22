using System;
using System.Collections.Generic;
using Serilog;
using XI.Portal.Data.CommonTypes;
using XI.Portal.Library.Rcon.Clients;
using XI.Portal.Library.Rcon.Interfaces;

namespace XI.Portal.Library.Rcon.Factories
{
    public class RconClientFactory : IRconClientFactory
    {
        private readonly ILogger logger;

        public RconClientFactory(ILogger logger)
        {
            this.logger = logger;
        }

        public IRconClient CreateInstance(GameType gameType, string serverName, string hostname, int queryPort, string rconPassword)
        {
            return CreateInstance(gameType, serverName, hostname, queryPort, rconPassword, null);
        }

        public IRconClient CreateInstance(GameType gameType, string serverName, string hostname, int queryPort, string rconPassword, List<TimeSpan> retryOverride)
        {
            IRconClient rconClient;

            switch (gameType)
            {
                case GameType.CallOfDuty2:
                    rconClient = new Cod2RconClient(logger);
                    break;
                case GameType.CallOfDuty4:
                    rconClient = new Cod4RconClient(logger);
                    break;
                case GameType.CallOfDuty5:
                    rconClient = new Cod5RconClient(logger);
                    break;
                case GameType.Insurgency:
                    rconClient = new InsurgencyRconClient(logger);
                    break;
                default:
                    throw new Exception("Unsupported game type");
            }

            rconClient.Configure(serverName, hostname, queryPort, rconPassword, retryOverride);
            return rconClient;
        }
    }
}