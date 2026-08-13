using Approvvigionamenti.ClientHttp;
using Approvvigionamenti.ClientHttp.Abstraction;
using Approvvigionamenti.ClientHttp.DependencyInjection; // Serve per leggere le Options
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class ApprovvigionamentiClientExtensions
{
    public static IServiceCollection AddApprovvigionamentiClient(this IServiceCollection services, IConfiguration configuration)
    {
        // Peschiamo la configurazione usando il SectionName
        IConfigurationSection confSection = configuration.GetSection(ApprovvigionamentiClientOptions.SectionName);
        ApprovvigionamentiClientOptions options = confSection.Get<ApprovvigionamentiClientOptions>() ?? new();

        services.AddHttpClient<IApprovvigionamentiClient, ApprovvigionamentiClient>(o => {
            o.BaseAddress = new Uri(options.BaseAddress);
        }).ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler
        {
            // Ignora gli errori dei certificati SSL in locale/Kubernetes
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
        });

        return services;
    }
}