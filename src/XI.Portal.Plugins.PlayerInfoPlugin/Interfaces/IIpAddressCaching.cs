namespace XI.Portal.Plugins.PlayerInfoPlugin.Interfaces
{
    public interface IIpAddressCaching
    {
        void AddToCache(string ipAddress);
        bool IpAddressInCache(string ipAddress);
        void ReduceCache();
    }
}