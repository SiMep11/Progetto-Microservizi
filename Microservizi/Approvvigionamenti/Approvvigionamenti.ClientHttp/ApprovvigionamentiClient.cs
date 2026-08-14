using System.Net.Http.Json;
using Approvvigionamenti.ClientHttp.Abstraction;
using Approvvigionamenti.Shared;

namespace Approvvigionamenti.ClientHttp;

// NB: i controller usano [Route("[controller]/[action]")], quindi le rotte sono /NomeController/NomeAction.
public class ApprovvigionamentiClient(HttpClient httpClient) : IApprovvigionamentiClient
{
    public async Task<IEnumerable<FornitoreDto>> GetAllFornitoriAsync(CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync("/Fornitori/GetAll", cancellationToken);

        var result = await response.EnsureSuccessStatusCode()
            .Content.ReadFromJsonAsync<IEnumerable<FornitoreDto>>(cancellationToken: cancellationToken);

        return result ?? new List<FornitoreDto>();
    }

    public async Task<FornitoreDto> CreaFornitoreAsync(FornitoreInsertDto fornitore, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("/Fornitori/Create", fornitore, cancellationToken);

        var creato = await response.EnsureSuccessStatusCode()
            .Content.ReadFromJsonAsync<FornitoreDto>(cancellationToken: cancellationToken);

        return creato!;
    }

    public async Task<IEnumerable<OrdineFornitoreDto>> GetAllOrdiniFornitoreAsync(CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync("/OrdiniFornitore/GetAll", cancellationToken);

        var result = await response.EnsureSuccessStatusCode()
            .Content.ReadFromJsonAsync<IEnumerable<OrdineFornitoreDto>>(cancellationToken: cancellationToken);

        return result ?? new List<OrdineFornitoreDto>();
    }

    public async Task ConfermaRicezioneAsync(int ordineFornitoreId, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsync($"/OrdiniFornitore/ConfermaRicezione/{ordineFornitoreId}", null, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}