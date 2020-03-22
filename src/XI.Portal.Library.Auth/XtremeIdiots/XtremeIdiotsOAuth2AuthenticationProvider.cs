using System;
using System.Threading.Tasks;

namespace XI.Portal.Library.Auth.XtremeIdiots
{
    public class XtremeIdiotsOAuth2AuthenticationProvider : IXtremeIdiotsOAuth2AuthenticationProvider
    {
        public XtremeIdiotsOAuth2AuthenticationProvider()
        {
            OnAuthenticated = context => Task.FromResult((object) null);
            OnReturnEndpoint = context => Task.FromResult((object) null);
            OnApplyRedirect = context => context.Response.Redirect(context.RedirectUri);
        }

        public Func<XtremeIdiotsOAuth2AuthenticatedContext, Task> OnAuthenticated { get; set; }
        public Func<XtremeIdiotsOAuth2ReturnEndpointContext, Task> OnReturnEndpoint { get; set; }
        public Action<XtremeIdiotsOAuth2ApplyRedirectContext> OnApplyRedirect { get; set; }

        public virtual Task Authenticated(XtremeIdiotsOAuth2AuthenticatedContext context)
        {
            return OnAuthenticated(context);
        }

        public virtual Task ReturnEndpoint(XtremeIdiotsOAuth2ReturnEndpointContext context)
        {
            return OnReturnEndpoint(context);
        }

        public virtual void ApplyRedirect(XtremeIdiotsOAuth2ApplyRedirectContext context)
        {
            OnApplyRedirect(context);
        }
    }
}