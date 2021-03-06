﻿using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using Unity;
using XI.Portal.Configuration.Interfaces;
using XI.Portal.Data.Core.Models;
using XI.Portal.Library.Auth;
using XI.Portal.Library.Auth.Extensions;
using XI.Portal.Library.Auth.XtremeIdiots;
using XI.Portal.Web;

[assembly: OwinStartup(typeof(Startup))]

namespace XI.Portal.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Landing/Index"),
                Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity =
                        SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                            TimeSpan.FromMinutes(30),
                            (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);


            var xtremeIdiotsForumsConfiguration = UnityConfig.Container.Resolve<IForumsConfiguration>();

            app.UseXtremeIdiotsAuthentication(new XtremeIdiotsOAuth2AuthenticationOptions
            {
                ClientId = xtremeIdiotsForumsConfiguration.OAuthClientId,
                ClientSecret = xtremeIdiotsForumsConfiguration.OAuthClientSecret
            });
        }
    }
}