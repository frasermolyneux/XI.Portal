using System;
using System.Configuration;
using System.Data.Entity;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using XI.Portal.Data.Core.Context;
using XI.Portal.Data.Core.Models;
using XI.Portal.Library.Auth;
using XI.Portal.Library.Forums;
using XI.Portal.Library.GameTracker;
using XI.Portal.Library.GeoLocation.Repository;
using XI.Portal.Library.Logging;
using XI.Portal.Library.MapRedirect;
using XI.Portal.Web.Portal.Navigation;

namespace XI.Portal.Web
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
            var connectionString = ConfigurationManager.ConnectionStrings["PortalContext"];

            if (connectionString == null || connectionString.ConnectionString == "__ConnectionString__")
            {
                throw new Exception("Connection string has not been configured correctly in app settings");
            }

            container.RegisterType<ContextOptions>(new ContainerControlledLifetimeManager(), new InjectionFactory(
                (ctr, type, name) => new ContextOptions
                {
                    ConnectionString = connectionString.Name
                }));

            container.RegisterType<DbContext, PortalContext>(new InjectionConstructor(connectionString.Name));

            container.RegisterType<IContextProvider, ContextProvider>();
            container.RegisterType<INavigationMenu, NavigationMenu>();
            container.RegisterType<IMapImageRepository, MapImageRepository>();
            container.RegisterType<IDatabaseLogger, DatabaseLogger>();
            container.RegisterType<IGeoLocationApiRepository, GeoLocationApiRepository>();
            container.RegisterType<IMapRedirectRepository, MapRedirectRepository>();
            container.RegisterType<IManageTopics, ManageTopics>();

            #region AuthContext
            var contextOptions = container.Resolve<ContextOptions>();
            var context = new PortalContext(contextOptions.ConnectionString);
            #endregion

            #region Auth
            container.RegisterType<ApplicationSignInManager>();
            container.RegisterType<ApplicationUserManager>();
            container.RegisterType<IUserStore<ApplicationUser>, UserStore<ApplicationUser>>();
            container.RegisterType<UserManager<ApplicationUser>>(new HierarchicalLifetimeManager());
            container.RegisterType<RoleManager<IdentityRole>>(new InjectionFactory(x => new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context))));
            container.RegisterType<IAuthenticationManager>(new InjectionFactory(x => HttpContext.Current.GetOwinContext().Authentication));
            #endregion
        }
    }
}