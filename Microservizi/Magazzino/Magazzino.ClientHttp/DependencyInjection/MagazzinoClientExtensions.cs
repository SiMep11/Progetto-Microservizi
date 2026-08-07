using Magazzino.ClientHttp;
using Magazzino.ClientHttp.Abstraction;
using Magazzino.ClientHttp.DependencyInjection; // Serve per leggere le Options
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class MagazzinoClientExtensions
{
    public static IServiceCollection AddMagazzinoClient(this IServiceCollection services, IConfiguration configuration)
    {
        // Peschiamo la configurazione usando il SectionName
        IConfigurationSection confSection = configuration.GetSection(MagazzinoClientOptions.SectionName);
        MagazzinoClientOptions options = confSection.Get<MagazzinoClientOptions>() ?? new();

        services.AddHttpClient<IMagazzinoClient, MagazzinoClient>(o => {
            o.BaseAddress = new Uri(options.BaseAddress);
        }).ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler
        {
            // Ignora gli errori dei certificati SSL in locale/Kubernetes
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
        });

        return services;
    }
}