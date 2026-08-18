using Pagamenti.ClientHttp;
using Pagamenti.ClientHttp.Abstraction;
using Pagamenti.ClientHttp.DependencyInjection; // Serve per leggere le Options
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class PagamentiClientExtensions
{
    public static IServiceCollection AddPagamentiClient(this IServiceCollection services, IConfiguration configuration)
    {
        // Pescho la configurazione usando il SectionName
        IConfigurationSection confSection = configuration.GetSection(PagamentiClientOptions.SectionName);
        PagamentiClientOptions options = confSection.Get<PagamentiClientOptions>() ?? new();

        services.AddHttpClient<IPagamentiClient, PagamentiClient>(o => {
            o.BaseAddress = new Uri(options.BaseAddress);
        }).ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler
        {
            // Ignora gli errori dei certificati SSL in locale/Kubernetes
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
        });

        return services;
    }
}
