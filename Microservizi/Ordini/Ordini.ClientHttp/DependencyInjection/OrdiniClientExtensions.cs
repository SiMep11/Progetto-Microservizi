using Ordini.ClientHttp;
using Ordini.ClientHttp.Abstraction;
using Ordini.ClientHttp.DependencyInjection; // Serve per leggere le Options
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class OrdiniClientExtensions
{
    public static IServiceCollection AddOrdiniClient(this IServiceCollection services, IConfiguration configuration)
    {
        // Pesco la configurazione usando il SectionName
        IConfigurationSection confSection = configuration.GetSection(OrdiniClientOptions.SectionName);
        OrdiniClientOptions options = confSection.Get<OrdiniClientOptions>() ?? new();

        services.AddHttpClient<IOrdiniClient, OrdiniClient>(o => {
            o.BaseAddress = new Uri(options.BaseAddress);
        }).ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler
        {
            // Ignora gli errori dei certificati SSL in locale/Kubernetes
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
        });

        return services;
    }
}
