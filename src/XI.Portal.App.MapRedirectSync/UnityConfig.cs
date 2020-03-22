using Serilog;
using System;
using Unity;
using Unity.Lifetime;
using XI.Portal.Configuration.AwsSecrets;
using XI.Portal.Configuration.Database;
using XI.Portal.Configuration.Forums;
using XI.Portal.Configuration.Interfaces;
using XI.Portal.Configuration.Maps;
using XI.Portal.Configuration.Providers;
using XI.Portal.Data.Core.Context;
using XI.Portal.Services.MapRedirect.Interfaces;
using XI.Portal.Services.MapRedirect.Repositories;

namespace XI.Portal.App.MapRedirectSync
{
    public static class UnityConfig
    {
        private static Lazy<IUnityContainer> container =
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
            container.RegisterType<IForumsConfiguration, ForumsConfiguration>();
            container.RegisterType<IMapsConfiguration, MapsConfiguration>();

            // Database
            container.RegisterType<IContextProvider, ContextProvider>();

            // Map Redirect
            container.RegisterType<IMapRedirectRepository, MapRedirectRepository>();
        }
    }
}
