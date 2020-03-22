using Serilog;

namespace XI.Portal.Library.Rcon.Clients
{
    public class Cod4RconClient : CodBaseRconClient
    {
        public Cod4RconClient(ILogger logger) : base(logger)
        {
        }
    }
}