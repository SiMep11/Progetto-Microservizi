using System;
using System.Collections.Generic;
using System.Text;

namespace Ordini.ClientHttp.DependencyInjection
{
    public class OrdiniClientOptions
    {
        // Qui salveremo l'URL del magazzino
        public string BaseUrl { get; set; } = string.Empty;
    }
}
