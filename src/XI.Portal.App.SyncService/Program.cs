using System.IO.Abstractions;
using Serilog;
using Topshelf;
using Topshelf.Unity;
using Unity;
using Unity.Lifetime;
using XI.Portal.App.SyncService.BanFiles;
using XI.Portal.App.SyncService.BanSource;
using XI.Portal.App.SyncService.Configuration;
using XI.Portal.App.SyncService.PlayerSync;
using XI.Portal.App.SyncService.Service;
using XI.Portal.Configuration.AwsSecrets;
using XI.Portal.Configuration.Database;
using XI.Portal.Configuration.Demos;
using XI.Portal.Configuration.Forums;
using XI.Portal.Configuration.GeoLocation;
using XI.Portal.Configuration.Interfaces;
using XI.Portal.Configuration.LogProxyPlugin;
using XI.Portal.Configuration.Maps;
using XI.Portal.Configuration.Providers;
using XI.Portal.Data.Contracts.Repositories;
using XI.Portal.Data.Core.Context;
using XI.Portal.Data.Repositories;
using XI.Portal.Library.Forums;
using XI.Portal.Library.Ftp.Helpers;
using XI.Portal.Library.Ftp.Interfaces;
using XI.Portal.Library.Logging;

namespace XI.Portal.App.SyncService
{
    internal class Program
    {
        private static void Main()
        {
            var container = new UnityContainer();

            var logger = new LoggerConfiguration()
                .ReadFrom.AppSettings()
                .WriteTo.ColoredConsole()
                .CreateLogger();
            Log.Logger = logger;

            container.RegisterFactory<ILogger>((ctr, type, name) => logger, new ContainerControlledLifetimeManager());

            // Libraries
            container.RegisterType<IFtpHelper, FtpHelper>();

            // Configuration Providers
            container.RegisterType<IConfigurationProvider, ConfigurationProvider>();
            container.RegisterType<IAwsSecretConfigurationProvider, AwsSecretConfigurationProvider>();
            container.RegisterType<ILocalConfigurationProvider, LocalConfigurationProvider>();

            // Configurations
            container.RegisterType<IAwsSecretsConfiguration, AwsSecretsConfiguration>();
            container.RegisterType<IDatabaseConfiguration, DatabaseConfiguration>();
            container.RegisterType<IDemosConfiguration, DemosConfiguration>();
            container.RegisterType<IForumsConfiguration, ForumsConfiguration>();
            container.RegisterType<ILogProxyPluginConfiguration, LogProxyPluginConfiguration>();
            container.RegisterType<IMapsConfiguration, MapsConfiguration>();

            // Repositories
            container.RegisterType<IAdminActionsRepository, AdminActionsRepository>();

            // Other
            container.RegisterType<IContextProvider, ContextProvider>();
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