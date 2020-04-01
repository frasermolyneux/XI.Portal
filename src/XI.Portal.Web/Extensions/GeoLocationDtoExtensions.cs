using System.Web;
using FM.GeoLocation.Contract.Models;

namespace XI.Portal.Web.Extensions
{
    public static class GeoLocationDtoExtensions
    {
        public static HtmlString FlagImage(this GeoLocationDto geoLocationDto)
        {
            return !string.IsNullOrWhiteSpace(geoLocationDto.CountryCode)
                ? new HtmlString($"<img src=\"/content/flags/{geoLocationDto.CountryCode.ToLower()}.png\" />")
                : new HtmlString("<img src=\"/content/flags/unknown.png\" />");
        }

        public static HtmlString LocationSummary(this GeoLocationDto geoLocationDto)
        {
            if (!string.IsNullOrWhiteSpace(geoLocationDto.CityName) &&
                !string.IsNullOrWhiteSpace(geoLocationDto.CountryName))
                return new HtmlString($"{geoLocationDto.CityName}, {geoLocationDto.CountryName}");

            if (!string.IsNullOrWhiteSpace(geoLocationDto.CountryCode))
                return new HtmlString($"{geoLocationDto.CountryCode}");

            if (!string.IsNullOrWhiteSpace(geoLocationDto.RegisteredCountry))
                return new HtmlString($"{geoLocationDto.RegisteredCountry}");

            return new HtmlString("Unknown");
        }
    }
}