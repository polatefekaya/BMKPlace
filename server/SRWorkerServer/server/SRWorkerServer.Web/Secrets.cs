using System;
using Azure.Identity;

namespace SRWorkerServer.Web;

public static class Secrets
{
    public static IConfigurationBuilder ConfigureKeyVault(this IConfigurationBuilder configuration)
    {
        Uri keyValUri = new Uri("https://bmkplace-keys.vault.azure.net/");
        configuration.AddAzureKeyVault(keyValUri, new DefaultAzureCredential());

        return configuration;
    }
}
