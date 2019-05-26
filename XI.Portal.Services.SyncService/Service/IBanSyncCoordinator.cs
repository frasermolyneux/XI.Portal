using System;

namespace XI.Portal.Services.SyncService.Service
{
    public interface IBanSyncCoordinator
    {
        void ExecuteBanSync(Guid banFileMonitorId);
    }
}