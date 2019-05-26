using System;
using System.Net.Http;
using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.Infrastructure;
using Owin;

namespace XI.Portal.Library.Auth.XtremeIdiots
{
    public class XtremeIdiotsOAuth2AuthenticationMiddleware : AuthenticationMiddleware<XtremeIdiotsOAuth2AuthenticationOptions>
    {
        private readonly HttpClient httpClient;
        private readonly ILogger logger;

        public XtremeIdiotsOAuth2AuthenticationMiddleware(OwinMiddleware next, IAppBuilder app,
            XtremeIdiotsOAuth2AuthenticationOptions options)
            : base(next, options)
        {
            if (string.IsNullOrWhiteSpace(Options.ClientId))
                throw new ArgumentException(nameof(Options.ClientId));

            if (string.IsNullOrWhiteSpace(Options.ClientSecret))
                throw new ArgumentException(nameof(Options.ClientSecret));

            logger = app.CreateLogger<XtremeIdiotsOAuth2AuthenticationMiddleware>();

            if (Options.Provider == null)
                Options.Provider =
                    new XtremeIdiotsOAuth2AuthenticationProvider();

            if (Options.StateDataFormat == null)
                Options.StateDataFormat = new PropertiesDataFormat(app.CreateDataProtector(
                    typeof(XtremeIdiotsOAuth2AuthenticationMiddleware).FullName, Options.AuthenticationType, "v1"));

            if (string.IsNullOrEmpty(Options.SignInAsAuthenticationType))
                Options.SignInAsAuthenticationType = app.GetDefaultSignInAsAuthenticationType();

            httpClient = new HttpClient(ResolveHttpMessageHandler(Options))
            {
                Timeout = Options.BackchannelTimeout,
                MaxResponseContentBufferSize = 10485760L
            };
        }

        protected override AuthenticationHandler<XtremeIdiotsOAuth2AuthenticationOptions> CreateHandler()
        {
            return new XtremeIdiotsOAuth2AuthenticationHandler(httpClient, logger);
        }

        private static HttpMessageHandler ResolveHttpMessageHandler(XtremeIdiotsOAuth2AuthenticationOptions options)
        {
            var httpMessageHandler = options.BackchannelHttpHandler ?? new WebRequestHandler();

            if (options.BackchannelCertificateValidator == null)
                return httpMessageHandler;

            if (!(httpMessageHandler is WebRequestHandler webRequestHandler))
                throw new InvalidOperationException("");

            webRequestHandler.ServerCertificateValidationCallback =
                options.BackchannelCertificateValidator.Validate;

            return httpMessageHandler;
        }
    }
}