using System.Net.Http.Json;
using Magazzino.ClientHttp.Abstraction;
using Magazzino.Shared;
using Microsoft.AspNetCore.Http; // Necessario per usare la classe QueryString
using System.Globalization;
using System.Net;

namespace Magazzino.ClientHttp;

public class MagazzinoClient(HttpClient httpClient) : IMagazzinoClient
{
    public async Task<ProdottoDto?> GetProdottoByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        // Passaggio ID tramite QueryString
        var queryString = QueryString.Create(new Dictionary<string, string?>() {
            { "id", id.ToString(CultureInfo.InvariantCulture) }
        });

        var response = await httpClient.GetAsync($"/Prodotti/GetProdottoById{queryString.Value}", cancellationToken);
        // Un prodotto inesistente è una risposta legittima(il controller torna NotFound),
        // non un errore: la traduciamo in null invece di sollevare eccezione.
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        return await response.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<ProdottoDto?>(cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<ProdottoDto>> GetAllProdottiAsync(CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync("/Prodotti/GetProdotti", cancellationToken);

        var result = await response.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<IEnumerable<ProdottoDto>>(cancellationToken: cancellationToken);
        return result ?? new List<ProdottoDto>();
    }

    public async Task<ProdottoDto?> CreaProdottoAsync(ProdottoDto prodotto, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("/Prodotti/AggiungiProdotto", prodotto, cancellationToken);
        return await response.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<ProdottoDto?>(cancellationToken: cancellationToken);
    }
}