using Serilog;
using Topshelf;
using Topshelf.Unity;
using Unity;
using Unity.Lifetime;
using XI.Portal.Data.Core.Context;
using XI.Portal.Library.Configuration;
using XI.Portal.Library.Configuration.Providers;
using XI.Portal.Library.GeoLocation.Repository;
using XI.Portal.Library.Logging;
using XI.Portal.Library.Rcon.Factories;
using XI.Portal.Library.Rcon.Interfaces;
using XI.Portal.Services.RconMonitorService.Factories;
using XI.Portal.Services.RconMonitorService.Interfaces;

namespace XI.Portal.Services.RconMonitorService
{
    internal partial class Program
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

            container.RegisterFactory<ILogger>((ctr, type, name) => logger, new ContainerControlledLifetimeManager());

            container.RegisterType<IContextProvider, ContextProvider>();
            container.RegisterType<IDatabaseLogger, DatabaseLogger>();
            container.RegisterType<IRconClientFactory, RconClientFactory>();

            container.RegisterType<IGeoLocationApiRepository, GeoLocationApiRepository>();

            container.RegisterType<IRconMonitorFactory, RconMonitorFactory>();

            HostFactory.Run(x =>
            {
                x.UseUnityContainer(container);
                x.UseSerilog();

                x.Service<RconMonitorService>(s =>
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