using System;
using System.Data.Entity;
using XI.Portal.Data.Core.Migrations;
using XI.Portal.Library.Configuration;

namespace XI.Portal.Data.Core.Context
{
    public interface IContextProvider : IDisposable
    {
        PortalContext GetContext();
    }

    public class ContextProvider : IContextProvider
    {
        private readonly DatabaseConfiguration databaseConfiguration;

        private bool disposed;

        public ContextProvider(DatabaseConfiguration databaseConfiguration)
        {
            this.databaseConfiguration = databaseConfiguration;
        }

        public PortalContext GetContext()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<PortalContext, Configuration>());
            var portalContext = new PortalContext(databaseConfiguration.DbConnectionString);
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