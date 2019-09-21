namespace XI.Portal.Plugins.Interfaces
{
    public interface IPlugin
    {
        void RegisterEventHandlers(IPluginEvents events);
    }
}