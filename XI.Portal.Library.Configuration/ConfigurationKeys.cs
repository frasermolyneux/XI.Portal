﻿namespace XI.Portal.Library.Configuration
{
    public static class ConfigurationKeys
    {
        public static string AwsRegion { get; } = "AwsRegion";
        public static string AwsAccessKey { get; } = "AwsAccessKey";
        public static string AwsSecretKey { get; } = "AwsSecretKey";
        public static string AwsPortalSecretName { get; } = "AwsPortalSecretName";

        public static string DbConnectionString { get; } = "DbConnectionString";
    }
}