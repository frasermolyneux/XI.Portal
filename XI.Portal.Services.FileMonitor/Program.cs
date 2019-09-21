using System;
using System.Data.Entity;
using System.Threading;
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
using XI.Portal.Plugins.Events;
using XI.Portal.Plugins.FuckYouPlugin;
using XI.Portal.Plugins.Interfaces;
using XI.Portal.Plugins.LogProxyPlugin;
using XI.Portal.Plugins.PlayerInfoPlugin;
using XI.Portal.Services.FileMonitor.Factories;
using XI.Portal.Services.FileMonitor.Interfaces;

namespace XI.Portal.Services.FileMonitor
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

    internal class FileMonitorService
    {
        private readonly IContextProvider contextProvider;
        private readonly IDatabaseLogger databaseLogger;
        private readonly IFtpFileMonitorFactory ftpFileMonitorFactory;
        private readonly ILogger logger;
        private readonly IParserFactory parserFactory;

        private CancellationTokenSource cts;

        public FileMonitorService(ILogger logger, IContextProvider contextProvider, IDatabaseLogger databaseLogger, IFtpFileMonitorFactory ftpFileMonitorFactory, IParserFactory parserFactory)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
            this.databaseLogger = databaseLogger ?? throw new ArgumentNullException(nameof(databaseLogger));
            this.ftpFileMonitorFactory = ftpFileMonitorFactory ?? throw new ArgumentNullException(nameof(ftpFileMonitorFactory));
            this.parserFactory = parserFactory ?? throw new ArgumentNullException(nameof(parserFactory));
        }

        public void Start()
        {
            logger.Information("Starting new File Monitor instance");
            databaseLogger.CreateSystemLogAsync("Info", "Starting new File Monitor instance");

            cts = new CancellationTokenSource();

            using (var context = contextProvider.GetContext())
            {
                foreach (var fileMonitor in context.FileMonitors.Include(fm => fm.GameServer))
                {
                    logger.Information($"Creating FtpFileMonitor for ftp://{fileMonitor.FilePath}");

                    var ftpFileMonitor = ftpFileMonitorFactory.CreateInstance($"ftp://{fileMonitor.GameServer.FtpHostname}/{fileMonitor.FilePath}",
                        fileMonitor.GameServer.FtpUsername,
                        fileMonitor.GameServer.FtpPassword,
                        fileMonitor.GameServer.ServerId,
                        fileMonitor.GameServer.GameType,
                        cts);

                    ftpFileMonitor.LineRead += FtpFileMonitor_LineRead;
                }
            }
        }

        public void Stop()
        {
            logger.Information("Stopping current File Monitor instance");
            databaseLogger.CreateSystemLogAsync("Info", "Stopping current File Monitor instance");

            cts?.Cancel();
        }

        public void Shutdown()
        {
            logger.Information("Shutting down current FileMonitor instance");
            databaseLogger.CreateSystemLogAsync("Info", "Shutting down current FileMonitor instance");

            cts?.Cancel();
        }

        private void FtpFileMonitor_LineRead(object sender, EventArgs e)
        {
            var eventData = (LineReadEventArgs) e;

            var parser = parserFactory.GetParserForGameType(eventData.GameType);
            parser.ParseLine(eventData.LineData, eventData.ServerId);
        }
    }
}