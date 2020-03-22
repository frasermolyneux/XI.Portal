using System;
using Serilog;
using Topshelf;
using Topshelf.Unity;
using Unity;
using XI.Portal.App.SyncService.Service;

namespace XI.Portal.App.SyncService
{
    internal class Program
    {
        private static void Main()
        {
            var container = (UnityContainer) UnityConfig.Container;
            var logger = container.Resolve<ILogger>();

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

                x.EnableServiceRecovery(service =>
                    service.RestartService(TimeSpan.FromSeconds(30))
                );

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