using System.Net.Http.Json;
using Magazzino.ClientHttp.Abstraction;
using Magazzino.Shared;
using Microsoft.AspNetCore.Http; // Necessario per usare la classe QueryString
using System.Globalization;

namespace Magazzino.ClientHttp;

public class MagazzinoClient(HttpClient httpClient) : IMagazzinoClient
{
    public async Task<ProdottoDto?> GetProdottoByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        // PUNTO 2: Passaggio ID tramite QueryString
        var queryString = QueryString.Create(new Dictionary<string, string?>() {
            { "id", id.ToString(CultureInfo.InvariantCulture) }
        });

        // PUNTO 1: Gestione errori aggressiva
        var response = await httpClient.GetAsync($"/api/prodotti/GetProdottoById{queryString}", cancellationToken);
        return await response.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<ProdottoDto?>(cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<ProdottoDto>> GetAllProdottiAsync(CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync("/api/prodotti/GetAllProdotti", cancellationToken);

        // PUNTO 1: Gestione errori aggressiva
        var result = await response.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<IEnumerable<ProdottoDto>>(cancellationToken: cancellationToken);
        return result ?? new List<ProdottoDto>();
    }

    public async Task<ProdottoDto?> CreaProdottoAsync(ProdottoDto prodotto, CancellationToken cancellationToken = default)
    {
        // Niente Punto 3: Usiamo la versione moderna per il Json (PostAsJsonAsync)
        // PUNTO 1: Gestione errori aggressiva
        var response = await httpClient.PostAsJsonAsync("/api/prodotti/CreaProdotto", prodotto, cancellationToken);
        return await response.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<ProdottoDto?>(cancellationToken: cancellationToken);
    }
}