using System;

namespace XI.Portal.App.SyncService.Service
{
    public interface IBanSyncCoordinator
    {
        void ExecuteBanSync(Guid banFileMonitorId);
    }
}