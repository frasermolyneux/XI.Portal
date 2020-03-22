using System;
using Serilog;
using Topshelf;
using Topshelf.Unity;
using Unity;

namespace XI.Portal.App.ServerMonitorService
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

                x.Service<ServerMonitorService>(s =>
                {
                    s.ConstructUsingUnityContainer();
                    s.WhenStarted(service => service.Start());
                    s.WhenStopped(service => service.Stop());
                    s.WhenShutdown(service => service.Shutdown());
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