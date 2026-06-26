using System;
using Ordini.ClientHttp; 
using Ordini.ClientHttp.Abstraction;
using Ordini.ClientHttp.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class OrdiniClientExtensions
    {
        public static IServiceCollection AddOrdiniClient(
            this IServiceCollection services,
            Action<OrdiniClientOptions> configureOptions)
        {
            // 1. Creiamo un "contenitore" per le opzioni vuoto e lo facciamo riempire
            var options = new OrdiniClientOptions();
            configureOptions(options);

            // 2. Registriamo il vero e proprio HttpClient, leggendo l'URL dalle opzioni!
            services.AddHttpClient<IOrdiniClient, OrdiniClient>(client =>
            {
                client.BaseAddress = new Uri(options.BaseUrl);
            });

            return services;
        }
    }
}
