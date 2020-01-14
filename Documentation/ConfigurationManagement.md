# [<](Index.md) Configuration Management

There is a project `XI.Portal.Configuration` in the solution that provides strongly typed configuration classes for the rest of the projects.

As part of this there is the option for using AWS Secrets Manager to store secrets. At present this requires the applications to be configured with Aws credentials through either app settings or environment variables. As such this needs to be in place for the applications to run locally.

## Secrets Manager

The values required in the app settings or environment variables are:

* AwsAccessKey
* AwsSecretKey
* AwsRegion
* AwsPortalSecretName

e.g:

```xml
    <add key="AwsRegion" value="us-east-2" />
    <add key="AwsAccessKey" value="AKIA3XXXXXXXXXXXXXXX" />
    <add key="AwsSecretKey" value="xxxxxxxxxxxxxxxxxxxx" />
    <add key="AwsPortalSecretName" value="xi-portal-environmentName" />
```

### Secret Values

Below is a list of secret values that needs to be in the `Secret Manager`:

* DbConnectionString
* XtremeIdiotsForumsApiKey
* XtremeIdiotsOAuthClientId
* XtremeIdiotsOAuthClientSecret
* MapRedirectKey

As the names suggest; these are very specific to the `XtremeIdiots` community making it near impossible for external input. This is something that can be remedied by introducing a stub of some form.
