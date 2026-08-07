using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Ordini.Shared; // DTO degli Ordini
using Ordini.ClientHttp.Abstraction;

namespace Ordini.ClientHttp
{
    // Costruttore primario che inietta automaticamente l'HttpClient
    public class OrdiniClient(HttpClient httpClient) : IOrdiniClient
    {
        public async Task<IEnumerable<OrdineDto>> GetAllOrdiniAsync(CancellationToken cancellationToken = default)
        {
            // Esegue una GET a /api/ordini e deserializza il JSON
            var ordini = await httpClient.GetFromJsonAsync<IEnumerable<OrdineDto>>("api/ordini", cancellationToken);
            return ordini ?? new List<OrdineDto>();
        }

        public async Task<OrdineDto?> GetOrdineByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            // Esegue una GET a /api/ordini/{id}
            return await httpClient.GetFromJsonAsync<OrdineDto>($"api/ordini/{id}", cancellationToken);
        }

        public async Task<OrdineDto> CreaOrdineAsync(OrdineDto ordine, CancellationToken cancellationToken = default)
        {
            // Esegue una POST a /api/ordini inviando il DTO come JSON
            var response = await httpClient.PostAsJsonAsync("api/ordini", ordine, cancellationToken);

            // Sicurezza: esplode se il server risponde con un errore (es. 400 Bad Request o 500)
            response.EnsureSuccessStatusCode();

            // Legge la risposta del server (che conterrà l'ordine con l'ID assegnato)
            var ordineCreato = await response.Content.ReadFromJsonAsync<OrdineDto>(cancellationToken: cancellationToken);
            return ordineCreato!;
        }
    }
}