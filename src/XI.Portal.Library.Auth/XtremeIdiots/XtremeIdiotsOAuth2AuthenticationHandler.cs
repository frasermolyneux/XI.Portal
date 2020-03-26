using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using Newtonsoft.Json.Linq;

namespace XI.Portal.Library.Auth.XtremeIdiots
{
    internal class XtremeIdiotsOAuth2AuthenticationHandler : AuthenticationHandler<XtremeIdiotsOAuth2AuthenticationOptions>
    {
        private readonly HttpClient httpClient;
        private readonly ILogger logger;

        public XtremeIdiotsOAuth2AuthenticationHandler(HttpClient httpClient, ILogger logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;
        }

        protected override async Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            AuthenticationProperties properties = null;
            try
            {
                var code = (string) null;
                var state = (string) null;
                var query = Request.Query;
                var values = query.GetValues("code");

                if (values != null && values.Count == 1)
                    code = values[0];

                values = query.GetValues("state");

                if (values != null && values.Count == 1)
                    state = values[0];

                properties = Options.StateDataFormat.Unprotect(state);

                if (properties == null)
                    return null;

                //if (!ValidateCorrelationId(Options.CookieManager, properties, logger))
                    //return new AuthenticationTicket(null, properties);

                    var requestPrefix = "https://" + Request.Host;
                var redirectUri = requestPrefix + Options.CallbackPath;

                var tokenResponse = await httpClient.PostAsync(Options.TokenEndpoint, new FormUrlEncodedContent(
                    new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("grant_type", "authorization_code"),
                        new KeyValuePair<string, string>("code", code),
                        new KeyValuePair<string, string>("redirect_uri", redirectUri),
                        new KeyValuePair<string, string>("client_id", Options.ClientId),
                        new KeyValuePair<string, string>("client_secret", Options.ClientSecret)
                    }));

                tokenResponse.EnsureSuccessStatusCode();

                var text = await tokenResponse.Content.ReadAsStringAsync();
                var response = JObject.Parse(text);
                var accessToken = response.Value<string>("access_token");

                if (string.IsNullOrWhiteSpace(accessToken))
                {
                    logger.WriteWarning("Access token was not found");
                    return new AuthenticationTicket(null, properties);
                }

                var graphResponse = await httpClient.SendAsync(
                    new HttpRequestMessage(HttpMethod.Get, Options.UserInformationEndpoint)
                    {
                        Headers =
                        {
                            Authorization = new AuthenticationHeaderValue("Bearer", accessToken)
                        }
                    }, Request.CallCancelled);

                graphResponse.EnsureSuccessStatusCode();
                text = await graphResponse.Content.ReadAsStringAsync();

                var user = JObject.Parse(text);
                var context = new XtremeIdiotsOAuth2AuthenticatedContext(Context, user, response)
                {
                    Identity = new ClaimsIdentity(Options.AuthenticationType,
                        "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
                        "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
                };

                context.Identity.AddClaim(new Claim(XtremeIdiotsClaims.Id, context.Id, ClaimValueTypes.String,
                    Options.AuthenticationType));
                context.Identity.AddClaim(new Claim(XtremeIdiotsClaims.Name, context.Name, ClaimValueTypes.String,
                    Options.AuthenticationType));
                context.Identity.AddClaim(new Claim(XtremeIdiotsClaims.Title, context.Title, ClaimValueTypes.String,
                    Options.AuthenticationType));
                context.Identity.AddClaim(new Claim(XtremeIdiotsClaims.FormattedName, context.FormattedName,
                    ClaimValueTypes.String, Options.AuthenticationType));
                context.Identity.AddClaim(new Claim(XtremeIdiotsClaims.PrimaryGroupId, context.PrimaryGroupId,
                    ClaimValueTypes.String, Options.AuthenticationType));
                context.Identity.AddClaim(new Claim(XtremeIdiotsClaims.PrimaryGroupName, context.PrimaryGroupName,
                    ClaimValueTypes.String, Options.AuthenticationType));
                context.Identity.AddClaim(new Claim(XtremeIdiotsClaims.PrimaryGroupIdFormattedName,
                    context.PrimaryGroupIdFormattedName, ClaimValueTypes.String, Options.AuthenticationType));
                context.Identity.AddClaim(new Claim(XtremeIdiotsClaims.Email, context.Email, ClaimValueTypes.String,
                    Options.AuthenticationType));
                context.Identity.AddClaim(new Claim(XtremeIdiotsClaims.PhotoUrl, context.PhotoUrl,
                    ClaimValueTypes.String, Options.AuthenticationType));
                context.Identity.AddClaim(new Claim(XtremeIdiotsClaims.PhotoUrlIsDefault, context.PhotoUrlIsDefault,
                    ClaimValueTypes.Boolean, Options.AuthenticationType));

                context.Properties = properties;
                await Options.Provider.Authenticated(context);
                return new AuthenticationTicket(context.Identity, context.Properties);
            }
            catch (Exception ex)
            {
                logger.WriteError("Authentication failed", ex);
                return new AuthenticationTicket(null, properties);
            }
        }

        protected override Task ApplyResponseChallengeAsync()
        {
            if (Response.StatusCode != 401)
                return Task.FromResult((object) null);

            var responseChallenge = Helper.LookupChallenge(Options.AuthenticationType, Options.AuthenticationMode);

            if (responseChallenge == null)
                return Task.FromResult((object) null);

            var str1 = "https://" + Request.Host;
            var str2 = str1 + Request.Path + Request.QueryString;
            var str3 = str1 + Options.CallbackPath;
            var properties = responseChallenge.Properties;

            if (string.IsNullOrEmpty(properties.RedirectUri))
                properties.RedirectUri = str2;

            GenerateCorrelationId(Options.CookieManager, properties);

            var dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                {"response_type", "code"},
                {"client_id", Options.ClientId},
                {"redirect_uri", str3}
            };

            var defaultValue = string.Join(" ", Options.Scope);

            if (string.IsNullOrEmpty(defaultValue))
                defaultValue = "profile email";

            AddQueryString(dictionary, properties, "scope", defaultValue);
            AddQueryString(dictionary, properties, "access_type", Options.AccessType);
            AddQueryString(dictionary, properties, "approval_prompt");
            AddQueryString(dictionary, properties, "prompt");
            AddQueryString(dictionary, properties, "login_hint");
            AddQueryString(dictionary, properties, "include_granted_scopes");

            var str4 = Options.StateDataFormat.Protect(properties);
            dictionary.Add("state", str4);
            var redirectUri = WebUtilities.AddQueryString(Options.AuthorizationEndpoint, dictionary);
            Options.Provider.ApplyRedirect(
                new XtremeIdiotsOAuth2ApplyRedirectContext(Context, Options, properties, redirectUri));

            return Task.FromResult((object) null);
        }

        public override async Task<bool> InvokeAsync()
        {
            return await InvokeReplyPathAsync();
        }

        private async Task<bool> InvokeReplyPathAsync()
        {
            if (!Options.CallbackPath.HasValue || !(Options.CallbackPath == Request.Path))
                return false;

            var ticket = await AuthenticateAsync();

            if (ticket == null)
            {
                logger.WriteWarning("Invalid return state, unable to redirect.");
                Response.StatusCode = 500;
                return true;
            }

            var context = new XtremeIdiotsOAuth2ReturnEndpointContext(Context, ticket)
            {
                SignInAsAuthenticationType = Options.SignInAsAuthenticationType,
                RedirectUri = ticket.Properties.RedirectUri
            };

            await Options.Provider.ReturnEndpoint(context);

            if (context.SignInAsAuthenticationType != null && context.Identity != null)
            {
                var claimsIdentity = context.Identity;

                if (!string.Equals(claimsIdentity.AuthenticationType, context.SignInAsAuthenticationType,
                    StringComparison.Ordinal))
                    claimsIdentity = new ClaimsIdentity(claimsIdentity.Claims, context.SignInAsAuthenticationType,
                        claimsIdentity.NameClaimType, claimsIdentity.RoleClaimType);

                Context.Authentication.SignIn(context.Properties, claimsIdentity);
            }

            if (context.IsRequestCompleted || context.RedirectUri == null)
                return context.IsRequestCompleted;

            var str = context.RedirectUri;
            if (context.Identity == null)
                str = WebUtilities.AddQueryString(str, "error", "access_denied");

            Response.Redirect(str);
            context.RequestCompleted();

            return context.IsRequestCompleted;
        }

        private static void AddQueryString(IDictionary<string, string> queryStrings,
            AuthenticationProperties properties, string name, string defaultValue = null)
        {
            if (!properties.Dictionary.TryGetValue(name, out var str))
                str = defaultValue;
            else
                properties.Dictionary.Remove(name);

            if (str == null)
                return;

            queryStrings[name] = str;
        }
    }
}