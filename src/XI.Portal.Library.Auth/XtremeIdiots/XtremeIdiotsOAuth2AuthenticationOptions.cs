using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Owin;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security;

namespace XI.Portal.Library.Auth.XtremeIdiots
{
    public class XtremeIdiotsOAuth2AuthenticationOptions : AuthenticationOptions
    {
        public XtremeIdiotsOAuth2AuthenticationOptions() : base("XtremeIdiots")
        {
            Caption = "XtremeIdiots";
            CallbackPath = new PathString("/xtremeidiots-callback");
            AuthenticationMode = AuthenticationMode.Passive;
            Scope = new List<string>();
            BackchannelTimeout = TimeSpan.FromSeconds(60.0);
            CookieManager = new CookieManager();
            AuthorizationEndpoint = "https://www.xtremeidiots.com/oauth/authorize";
            TokenEndpoint = "https://www.xtremeidiots.com/oauth/token/";
            UserInformationEndpoint = "https://www.xtremeidiots.com/api/core/me";
        }

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string AuthorizationEndpoint { get; set; }

        public string TokenEndpoint { get; set; }

        public string UserInformationEndpoint { get; set; }
        public ICertificateValidator BackchannelCertificateValidator { get; set; }
        public TimeSpan BackchannelTimeout { get; set; }
        public HttpMessageHandler BackchannelHttpHandler { get; set; }

        public string Caption
        {
            get => Description.Caption;
            set => Description.Caption = value;
        }

        public PathString CallbackPath { get; set; }
        public string SignInAsAuthenticationType { get; set; }
        public IXtremeIdiotsOAuth2AuthenticationProvider Provider { get; set; }
        public ISecureDataFormat<AuthenticationProperties> StateDataFormat { get; set; }
        public IList<string> Scope { get; }
        public string AccessType { get; set; }
        public ICookieManager CookieManager { get; set; }
    }
}