using System;
using XI.Portal.Configuration.Interfaces;

namespace XI.Portal.Data.Core.Context
{
    public interface IContextProvider : IDisposable
    {
        PortalContext GetContext();
    }

    public class ContextProvider : IContextProvider
    {
        private readonly IDatabaseConfiguration databaseConfiguration;

        private bool disposed;

        public ContextProvider(IDatabaseConfiguration databaseConfiguration)
        {
            this.databaseConfiguration = databaseConfiguration;
        }

        public PortalContext GetContext()
        {
            var portalContext = new PortalContext(databaseConfiguration.PortalDbConnectionString);
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