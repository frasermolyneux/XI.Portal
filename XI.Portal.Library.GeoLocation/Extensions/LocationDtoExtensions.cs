// ReSharper disable CompareOfFloatsByEqualityOperator

using XI.Portal.Library.GeoLocation.Models;

namespace XI.Portal.Library.GeoLocation.Extensions
{
    public static class LocationDtoExtensions
    {
        public static bool IsDefault(this LocationDto locationDto)
        {
            return locationDto.Latitude == 0 && locationDto.Longitude == 0;
        }
    }
}