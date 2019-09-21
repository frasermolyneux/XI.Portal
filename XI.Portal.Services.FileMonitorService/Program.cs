using System;
using Serilog;
using Topshelf;
using Topshelf.Unity;
using Unity;
using Unity.Lifetime;
using XI.Portal.Data.Core.Context;
using XI.Portal.Library.Configuration;
using XI.Portal.Library.Configuration.Providers;
using XI.Portal.Library.Logging;
using XI.Portal.Plugins.ChatMonitorPlugin;
using XI.Portal.Plugins.FuckYouPlugin;
using XI.Portal.Plugins.Interfaces;
using XI.Portal.Plugins.LogProxyPlugin;
using XI.Portal.Plugins.PlayerInfoPlugin;
using XI.Portal.Services.FileMonitorService.Factories;
using XI.Portal.Services.FileMonitorService.Interfaces;

namespace XI.Portal.Services.FileMonitorService
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

            container.RegisterType<AppSettingConfigurationProvider>();
            container.RegisterType<AwsSecretConfigurationProvider>();
            container.RegisterType<AwsConfiguration>();
            container.RegisterType<DatabaseConfiguration>();
            container.RegisterType<StatsLogProxyPluginConfiguration>();

            container.RegisterFactory<ILogger>((ctr, type, name) => logger, new ContainerControlledLifetimeManager());

            container.RegisterType<IContextProvider, ContextProvider>();
            container.RegisterType<IDatabaseLogger, DatabaseLogger>();

            container.RegisterType<IFtpFileMonitorFactory, FtpFileMonitorFactory>();
            container.RegisterType<IParserFactory, ParserFactory>();

            container.RegisterType<IPlugin, ChatMonitorPlugin>();
            container.RegisterType<IPlugin, PlayerInfoPlugin>();
            container.RegisterType<IPlugin, FuckYouPlugin>();
            container.RegisterType<IPlugin, LogProxyPlugin>();


            HostFactory.Run(x =>
            {
                x.UseUnityContainer(container);
                x.UseSerilog();

                x.Service<FileMonitorService>(s =>
                {
                    s.ConstructUsingUnityContainer();
                    s.WhenStarted(service => service.Start());
                    s.WhenStopped(service => service.Stop());
                    s.WhenShutdown(service => service.Shutdown());
                });

                x.RunAsLocalSystem();
                x.UseAssemblyInfoForServiceInfo();

                x.OnException(ex =>
                {
                    logger.Error(ex, "Top-level exception");

#if DEBUG
                    Console.ReadKey();
#endif
                });
            });
        }
    }
}