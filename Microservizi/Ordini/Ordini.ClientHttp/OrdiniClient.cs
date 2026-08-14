using System.Collections.Generic;
using System.Net;
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
            var response = await httpClient.GetAsync("/Ordini/GetAll", cancellationToken);

            var result = await response.EnsureSuccessStatusCode()
                .Content.ReadFromJsonAsync<IEnumerable<OrdineDto>>(cancellationToken: cancellationToken);

            return result ?? new List<OrdineDto>();
        }

        public async Task<OrdineDto?> GetOrdineByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            // L'action GetById ha [HttpGet("{id}")], quindi l'id va nel path
            var response = await httpClient.GetAsync($"/Ordini/GetById/{id}", cancellationToken);

            // Un ordine inesistente è una risposta legittima (il controller torna NotFound)
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            return await response.EnsureSuccessStatusCode()
                .Content.ReadFromJsonAsync<OrdineDto?>(cancellationToken: cancellationToken);
        }

        public async Task<OrdineDto> CreaOrdineAsync(OrdineInsertDto ordine, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostAsJsonAsync("/Ordini/Create", ordine, cancellationToken);

            // Sicurezza: esplode se il server risponde con un errore (es. 400 Bad Request o 500)
            var ordineCreato = await response.EnsureSuccessStatusCode()
                .Content.ReadFromJsonAsync<OrdineDto>(cancellationToken: cancellationToken);

            return ordineCreato!;
        }
    }
}
