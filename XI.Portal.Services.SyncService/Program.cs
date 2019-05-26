using System;
using System.Configuration;
using System.IO.Abstractions;
using Serilog;
using Topshelf;
using Topshelf.Unity;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using XI.Portal.Data.Core.Context;
using XI.Portal.Library.Forums;
using XI.Portal.Library.Logging;
using XI.Portal.Library.Logging.Sinks;
using XI.Portal.Services.SyncService.BanFiles;
using XI.Portal.Services.SyncService.BanSource;
using XI.Portal.Services.SyncService.Configuration;
using XI.Portal.Services.SyncService.PlayerSync;
using XI.Portal.Services.SyncService.Service;

namespace XI.Portal.Services.SyncService
{
    internal class Program
    {
        private static void Main()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["PortalContext"];

            if (connectionString == null || connectionString.ConnectionString == "__ConnectionString__")
                throw new Exception("Connection string has not been configured correctly in app settings");

            var container = new UnityContainer();

            container.RegisterType<ContextOptions>(new ContainerControlledLifetimeManager(), new InjectionFactory(
                (ctr, type, name) => new ContextOptions
                {
                    ConnectionString = connectionString.Name
                }));
            container.RegisterType<IContextProvider, ContextProvider>();

            var contextProvider = container.Resolve<IContextProvider>();

            var logger = new LoggerConfiguration()
                .ReadFrom.AppSettings()
                .WriteTo.ColoredConsole()
                .WriteTo.DatabaseSink(contextProvider)
                .CreateLogger();

            Log.Logger = logger;

            container.RegisterType<ILogger>(new ContainerControlledLifetimeManager(),
                new InjectionFactory((ctr, type, name) => logger));

            container.RegisterType<IDatabaseLogger, DatabaseLogger>();

            container.RegisterType<IBanSyncCoordinator, BanSyncCoordinator>();
            container.RegisterType<ILocalBanFileManager, LocalBanFileManager>();
            container.RegisterType<IBanFileImporter, BanFileImporter>();
            container.RegisterType<IDatabaseBanSource, DatabaseBanSource>();
            container.RegisterType<IExternalBanSource, ExternalBanSource>();
            container.RegisterType<IPlayerSynchronizer, PlayerSynchronizer>();
            container.RegisterType<IGuidValidator, GuidValidator>();
            container.RegisterType<IFileSystem, FileSystem>();
            container.RegisterType<IConfigurationWrapper, ConfigurationWrapper>();
            container.RegisterType<IManageTopics, ManageTopics>();

            UnityContainerBox.UnityContainer = container;

            HostFactory.Run(x =>
            {
                x.UseUnityContainer(container);
                x.UseSerilog();

                x.Service<ExecuteSyncService>(s =>
                {
                    s.ConstructUsingUnityContainer();
                    s.WhenStarted(service => service.Start());
                    s.WhenStopped(service => service.Stop());
                    s.WhenShutdown(service => service.Shutdown());
                    x.UseSerilog();
                });

                x.RunAsLocalSystem();
                x.UseAssemblyInfoForServiceInfo();
            });
        }
    }
}