﻿using System;
using FM.GeoLocation.Client;
using Serilog;
using Unity;
using Unity.Lifetime;
using XI.Portal.App.FileMonitorService.Factories;
using XI.Portal.App.FileMonitorService.Interfaces;
using XI.Portal.Configuration.AwsSecrets;
using XI.Portal.Configuration.Database;
using XI.Portal.Configuration.Demos;
using XI.Portal.Configuration.Forums;
using XI.Portal.Configuration.GeoLocation;
using XI.Portal.Configuration.Interfaces;
using XI.Portal.Configuration.LogProxyPlugin;
using XI.Portal.Configuration.Maps;
using XI.Portal.Configuration.Providers;
using XI.Portal.Data.Core.Context;
using XI.Portal.Library.Logging;
using XI.Portal.Library.Rcon.Factories;
using XI.Portal.Library.Rcon.Interfaces;
using XI.Portal.Plugins.ChatMonitorPlugin;
using XI.Portal.Plugins.FuckYouPlugin;
using XI.Portal.Plugins.ImAlivePlugin;
using XI.Portal.Plugins.Interfaces;
using XI.Portal.Plugins.LogProxyPlugin;
using XI.Portal.Plugins.MapRotationPlugin;
using XI.Portal.Plugins.PlayerInfoPlugin;
using XI.Portal.Plugins.PlayerInfoPlugin.Interfaces;
using XI.Portal.Plugins.PlayerInfoPlugin.LocalCaching;

namespace XI.Portal.App.FileMonitorService
{
    public static class UnityConfig
    {
        private static readonly Lazy<IUnityContainer> container =
            new Lazy<IUnityContainer>(() =>
            {
                var container = new UnityContainer();
                RegisterTypes(container);
                return container;
            });

        public static IUnityContainer Container => container.Value;

        public static void RegisterTypes(IUnityContainer container)
        {
            var logger = new LoggerConfiguration()
                .ReadFrom.AppSettings()
                .WriteTo.ColoredConsole()
                .CreateLogger();
            Log.Logger = logger;

            container.RegisterFactory<ILogger>((ctr, type, name) => logger, new ContainerControlledLifetimeManager());

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

            // Other
            container.RegisterType<IContextProvider, ContextProvider>();
            container.RegisterType<IDatabaseLogger, DatabaseLogger>();

            container.RegisterType<IFtpFileMonitorFactory, FtpFileMonitorFactory>();
            container.RegisterType<IParserFactory, ParserFactory>();
            container.RegisterType<IRconClientFactory, RconClientFactory>();
            container.RegisterType<IPlayerCaching, PlayerCaching>();

            container.RegisterType<IPlugin, ChatMonitorPlugin>();
            container.RegisterType<IPlugin, PlayerInfoPlugin>();
            container.RegisterType<IPlugin, FuckYouPlugin>();
            container.RegisterType<IPlugin, LogProxyPlugin>();
            container.RegisterType<IPlugin, MapRotationPlugin>();
            container.RegisterType<IPlugin, ImAlivePlugin>();

            // FM.GeoLocation
            container.RegisterType<IGeoLocationClientConfiguration, GeoLocationClientConfig>();
            container.RegisterType<IGeoLocationClient, GeoLocationClient>();
        }
    }
}