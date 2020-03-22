namespace XI.Portal.Configuration.AwsSecrets
{
    internal static class AwsSecretsConfigurationKeys
    {
        internal static string AccessKey { get; } = "AwsAccessKey";
        internal static string SecretKey { get; } = "AwsSecretKey";
        internal static string Region { get; } = "AwsRegion";
        internal static string SecretName { get; } = "AwsPortalSecretName";
    }
}
