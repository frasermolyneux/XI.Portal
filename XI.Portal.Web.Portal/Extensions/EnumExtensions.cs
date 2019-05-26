using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace XI.Portal.Web.Portal.Extensions
{
    public static class EnumExtensions
    {
        public static string DisplayName(this Enum item)
        {
            var type = item.GetType();
            var member = type.GetMember(item.ToString());
            var displayName =
                (DisplayAttribute) member[0].GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault();

            return displayName != null ? displayName.Name : item.ToString();
        }
    }
}