namespace XI.Portal.Library.Rcon.Interfaces
{
    public interface IRconClient
    {
        void Configure(string hostname, int queryPort, string rconPassword);
        string PlayerStatus();
        string KickPlayer(string targetPlayerNum);
        string BanPlayer(string targetPlayerNum);
        string RestartServer();
        string RestartMap();
        string NextMap();
        string MapRotation();
        string Say(string message);
    }
}