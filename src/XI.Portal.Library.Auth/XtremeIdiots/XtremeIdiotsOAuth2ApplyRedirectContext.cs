using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Provider;

namespace XI.Portal.Library.Auth.XtremeIdiots
{
    public class XtremeIdiotsOAuth2ApplyRedirectContext : BaseContext<XtremeIdiotsOAuth2AuthenticationOptions>
    {
        public XtremeIdiotsOAuth2ApplyRedirectContext(IOwinContext context,
            XtremeIdiotsOAuth2AuthenticationOptions options, AuthenticationProperties properties, string redirectUri)
            : base(context, options)
        {
            RedirectUri = redirectUri;
            Properties = properties;
        }

        public string RedirectUri { get; }
        public AuthenticationProperties Properties { get; }
    }
}