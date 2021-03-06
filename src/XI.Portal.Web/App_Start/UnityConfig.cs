using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Serilog;
using System;
using System.Data.Entity;
using System.Web;
using FM.GeoLocation.Client;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using XI.Portal.BLL.Web;
using XI.Portal.BLL.Web.Interfaces;
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
using XI.Portal.Data.Core.Models;
using XI.Portal.Data.Repositories;
using XI.Portal.Library.Analytics.Interfaces;
using XI.Portal.Library.Analytics.Providers;
using XI.Portal.Library.Auth;
using XI.Portal.Library.Forums;
using XI.Portal.Library.Ftp.Helpers;
using XI.Portal.Library.Ftp.Interfaces;
using XI.Portal.Library.Logging;
using XI.Portal.Library.Rcon.Factories;
using XI.Portal.Library.Rcon.Interfaces;
using XI.Portal.Services.MapRedirect.Interfaces;
using XI.Portal.Services.MapRedirect.Repositories;
using XI.Portal.Services.GameTracker.Interfaces;
using XI.Portal.Services.GameTracker.Repositories;
using XI.Portal.Web.Navigation;

namespace XI.Portal.Web
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public static class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container =
          new Lazy<IUnityContainer>(() =>
          {
              var container = new UnityContainer();
              RegisterTypes(container);
              return container;
          });

        /// <summary>
        /// Configured Unity Container.
        /// </summary>
        public static IUnityContainer Container => container.Value;
        #endregion

        /// <summary>
        /// Registers the type mappings with the Unity container.
        /// </summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>
        /// There is no need to register concrete types such as controllers or
        /// API controllers (unless you want to change the defaults), as Unity
        /// allows resolving a concrete type even if it was not previously
        /// registered.
        /// </remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            var logger = new LoggerConfiguration()
                .ReadFrom.AppSettings()
                .WriteTo.ColoredConsole()
                .CreateLogger();

            Log.Logger = logger;

            container.RegisterFactory<ILogger>((ctr, type, name) => logger, new ContainerControlledLifetimeManager());

            // Libraries
            container.RegisterType<IFtpHelper, FtpHelper>();
            container.RegisterType<IAdminActionsAnalytics, AdminActionsAnalytics>();
            container.RegisterType<IPlayersAnalytics, PlayersAnalytics>();

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
            container.RegisterType<ILivePlayersRepository, LivePlayersRepository>();
            container.RegisterType<IPlayersRepository, PlayersRepository>();
            container.RegisterType<IMapsRepository, MapsRepository>();

            // BLL
            container.RegisterType<IAdminActionsList, AdminActionsList>();
            container.RegisterType<IPlayersList, PlayersList>();
            container.RegisterType<IPortalIndex, PortalIndex>();
            container.RegisterType<IMapsList, MapsList>();

            // Other
            container.RegisterType<IContextProvider, ContextProvider>();
            container.RegisterType<INavigationMenu, NavigationMenu>();
            container.RegisterType<IMapImageRepository, MapImageRepository>();
            container.RegisterType<IDatabaseLogger, DatabaseLogger>();
            container.RegisterType<IMapRedirectRepository, MapRedirectRepository>();
            container.RegisterType<IManageTopics, ManageTopics>();
            container.RegisterType<IRconClientFactory, RconClientFactory>();

            // FM.GeoLocation
            container.RegisterType<IGeoLocationClientConfiguration, GeoLocationClientConfig>();
            container.RegisterType<IGeoLocationClient, GeoLocationClient>();

            var contextProvider = container.Resolve<IDatabaseConfiguration>();
            container.RegisterType<DbContext, PortalContext>(new InjectionConstructor(contextProvider.PortalDbConnectionString));
            var context = container.Resolve<DbContext>();

            container.RegisterType<ApplicationSignInManager>();
            container.RegisterType<ApplicationUserManager>();
            container.RegisterType<IUserStore<ApplicationUser>, UserStore<ApplicationUser>>();
            container.RegisterType<UserManager<ApplicationUser>>(new HierarchicalLifetimeManager());

            container.RegisterFactory<ApplicationRoleManager>(x => new ApplicationRoleManager(new RoleStore<IdentityRole>(context)));

            container.RegisterFactory<IdentityRole>(x => new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context)));
            container.RegisterFactory<IAuthenticationManager>(x => HttpContext.Current.GetOwinContext().Authentication);
        }
    }
}