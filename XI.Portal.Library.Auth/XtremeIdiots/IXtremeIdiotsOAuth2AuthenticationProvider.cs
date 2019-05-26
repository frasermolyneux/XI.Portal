using System.Threading.Tasks;

namespace XI.Portal.Library.Auth.XtremeIdiots
{
    public interface IXtremeIdiotsOAuth2AuthenticationProvider
    {
        Task Authenticated(XtremeIdiotsOAuth2AuthenticatedContext context);
        Task ReturnEndpoint(XtremeIdiotsOAuth2ReturnEndpointContext context);
        void ApplyRedirect(XtremeIdiotsOAuth2ApplyRedirectContext context);
    }
}