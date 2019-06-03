namespace XI.Portal.Library.Configuration
{
    public static class ConfigurationKeys
    {
        public static string AwsRegion { get; } = "AwsRegion";
        public static string AwsAccessKey { get; } = "AwsAccessKey";
        public static string AwsSecretKey { get; } = "AwsSecretKey";
        public static string AwsPortalSecretName { get; } = "AwsPortalSecretName";

        public static string DbConnectionString { get; } = "DbConnectionString";

        public static string XtremeIdiotsForumsApiKey { get; } = "XtremeIdiotsForumsApiKey";
        public static string XtremeIdiotsOAuthClientId { get; } = "XtremeIdiotsOAuthClientId";
        public static string XtremeIdiotsOAuthClientSecret { get; } = "XtremeIdiotsOAuthClientSecret";

        public static string DemoManagerBucketName { get; } = "DemoManagerBucketName";
    }
}