using System;
using System.Data.Entity;
using XI.Portal.Data.Core.Migrations;

namespace XI.Portal.Data.Core.Context
{
    public interface IContextProvider : IDisposable
    {
        PortalContext GetContext();
    }

    public class ContextProvider : IContextProvider
    {
        private readonly ContextOptions contextOptions;

        private bool disposed;

        public ContextProvider(ContextOptions contextOptions)
        {
            this.contextOptions = contextOptions;
        }

        public PortalContext GetContext()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<PortalContext, Configuration>());
            var portalContext = new PortalContext(contextOptions.ConnectionString);
            portalContext.Database.CreateIfNotExists();

            return portalContext;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            disposed = true;
        }
    }
}