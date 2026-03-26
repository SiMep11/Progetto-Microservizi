using Ordini.ClientHttp.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Ordini.ClientHttp.DependencyInjection
{
    public static class MagazzinoClientExtensions
    {
        // Questo è un "Extension Method". Aggiunge una funzione a IServiceCollection.
        public static IServiceCollection AddMagazzinoClient(
            this IServiceCollection services,
            Action<MagazzinoClientOptions> configureOptions)
        {
            // 1. Creiamo un "contenitore" per le opzioni vuoto e lo facciamo riempire
            var options = new MagazzinoClientOptions();
            configureOptions(options);

            // 2. Registriamo il vero e proprio HttpClient, leggendo l'URL dalle opzioni!
            services.AddHttpClient<IMagazzinoClient, MagazzinoClient>(client =>
            {
                client.BaseAddress = new Uri(options.BaseUrl);
            });

            return services;
        }
    }
}
