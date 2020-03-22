namespace XI.Portal.Configuration.Interfaces
{
    public interface IConfigurationProvider
    {
        string GetConfigurationItem(string configurationKey);
    }
}
