using System;
using System.Collections.Generic;

namespace XI.Portal.Configuration.Cache
{
    public static class AwsSecretsCache
    {
        public static Dictionary<string, string> CachedSecrets = new Dictionary<string, string>();
        public static DateTime Cached { get; set; } = DateTime.MinValue;
    }
}