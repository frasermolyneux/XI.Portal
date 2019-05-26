namespace XI.Portal.Library.Rcon.Client
{
    public interface IRconClient
    {
        string StatusCommand();
        string KickCommand(string targetPlayerNum);
        string BanCommand(string targetPlayerNum);
        string RestartServer();
        string RestartMap();
        string NextMap();
        string MapRotation();
    }
}