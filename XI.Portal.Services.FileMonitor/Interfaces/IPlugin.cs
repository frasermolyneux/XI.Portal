namespace XI.Portal.Services.FileMonitor.Interfaces
{
    internal interface IPlugin
    {
        void RegisterEventHandlers(IParser parser);
    }
}