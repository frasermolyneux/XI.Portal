using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System;
using System.Data.Entity;
using System.Web;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using XI.Portal.Data.Core.Context;
using XI.Portal.Data.Core.Models;
using XI.Portal.Library.Auth;
using XI.Portal.Library.Configuration;
using XI.Portal.Library.Configuration.Providers;
using XI.Portal.Library.Forums;
using XI.Portal.Library.GameTracker;
using XI.Portal.Library.GeoLocation.Repository;
using XI.Portal.Library.Logging;
using XI.Portal.Library.MapRedirect;
using XI.Portal.Web.Navigation;

namespace XI.Portal.Web.Portal
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
            container.RegisterType<AppSettingConfigurationProvider>();
            container.RegisterType<AwsSecretConfigurationProvider>();
            container.RegisterType<AwsConfiguration>();
            container.RegisterType<DatabaseConfiguration>();
            container.RegisterType<XtremeIdiotsForumsConfiguration>();
            container.RegisterType<DemoManagerConfiguration>();
            container.RegisterType<GeoLocationConfiguration>();
            container.RegisterType<MapRedirectConfiguration>();

            container.RegisterType<IContextProvider, ContextProvider>();
            container.RegisterType<INavigationMenu, NavigationMenu>();
            container.RegisterType<IMapImageRepository, MapImageRepository>();
            container.RegisterType<IDatabaseLogger, DatabaseLogger>();
            container.RegisterType<IGeoLocationApiRepository, GeoLocationApiRepository>();
            container.RegisterType<IMapRedirectRepository, MapRedirectRepository>();
            container.RegisterType<IManageTopics, ManageTopics>();

            var contextProvider = container.Resolve<DatabaseConfiguration>();
            container.RegisterType<DbContext, PortalContext>(new InjectionConstructor(contextProvider.DbConnectionString));
            var context = container.Resolve<DbContext>();

            container.RegisterType<ApplicationSignInManager>();
            container.RegisterType<ApplicationUserManager>();
            container.RegisterType<IUserStore<ApplicationUser>, UserStore<ApplicationUser>>();
            container.RegisterType<UserManager<ApplicationUser>>(new HierarchicalLifetimeManager());

            container.RegisterFactory<IdentityRole>(x => new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context)));
            container.RegisterFactory<IAuthenticationManager>(x => HttpContext.Current.GetOwinContext().Authentication);
        }
    }
}