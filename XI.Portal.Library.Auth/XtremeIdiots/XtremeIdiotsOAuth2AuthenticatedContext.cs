using System;
using System.Globalization;
using System.Security.Claims;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Provider;
using Newtonsoft.Json.Linq;

namespace XI.Portal.Library.Auth.XtremeIdiots
{
    public class XtremeIdiotsOAuth2AuthenticatedContext : BaseContext
    {
        public XtremeIdiotsOAuth2AuthenticatedContext(IOwinContext context, JObject user, JObject tokenResponse)
            : base(context)
        {
            User = user;
            TokenResponse = tokenResponse;

            if (tokenResponse != null)
            {
                AccessToken = tokenResponse.Value<string>("access_token");
                RefreshToken = tokenResponse.Value<string>("refresh_token");

                if (int.TryParse(tokenResponse.Value<string>("expires_in"), NumberStyles.Integer,
                    CultureInfo.InvariantCulture, out var result))
                    ExpiresIn = TimeSpan.FromSeconds(result);
            }

            Id = TryGetValue(user, "id");
            Name = TryGetValue(user, "name");
            Title = TryGetValue(user, "title");
            FormattedName = TryGetValue(user, "formattedName");
            PrimaryGroupId = TryGetValue(user, "primaryGroup", "id");
            PrimaryGroupName = TryGetValue(user, "primaryGroup", "name");
            PrimaryGroupIdFormattedName = TryGetValue(user, "primaryGroup", "formattedName");
            Email = TryGetValue(user, "email");
            PhotoUrl = TryGetValue(user, "photoUrl");
            PhotoUrlIsDefault = TryGetValue(user, "photoUrlIsDefault");
        }

        public JObject User { get; }
        public string AccessToken { get; }
        public string RefreshToken { get; }
        public TimeSpan? ExpiresIn { get; set; }
        public string Id { get; }
        public string Name { get; }
        public string Title { get; set; }
        public string FormattedName { get; set; }
        public string PrimaryGroupId { get; set; }
        public string PrimaryGroupName { get; set; }
        public string PrimaryGroupIdFormattedName { get; set; }
        public string PhotoUrl { get; set; }
        public string PhotoUrlIsDefault { get; set; }
        public string Email { get; }
        public ClaimsIdentity Identity { get; set; }
        public JObject TokenResponse { get; }

        public AuthenticationProperties Properties { get; set; }

        private static string TryGetValue(JObject user, string propertyName)
        {
            return !user.TryGetValue(propertyName, out var jtoken) ? null : jtoken.ToString();
        }

        private static string TryGetValue(JObject user, string propertyName, string subProperty)
        {
            if (!user.TryGetValue(propertyName, out var jtoken)) return null;

            var jobject = JObject.Parse(jtoken.ToString());
            if (jobject != null && jobject.TryGetValue(subProperty, out jtoken))
                return jtoken.ToString();

            return null;
        }
    }
}