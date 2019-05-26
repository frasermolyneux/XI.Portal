using System;
using Owin;

namespace XI.Portal.Library.Auth.XtremeIdiots
{
    public static class XtremeIdiotsAuthenticationExtensions
    {
        public static IAppBuilder UseXtremeIdiotsAuthentication(this IAppBuilder app,
            XtremeIdiotsOAuth2AuthenticationOptions options)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            app.Use(typeof(XtremeIdiotsOAuth2AuthenticationMiddleware), app, options);
            return app;
        }
    }
}