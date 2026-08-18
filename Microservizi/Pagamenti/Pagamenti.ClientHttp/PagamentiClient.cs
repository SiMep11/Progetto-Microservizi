using System.Net.Http.Json;
using Pagamenti.ClientHttp.Abstraction;
using Pagamenti.Shared;

namespace Pagamenti.ClientHttp;

// NB: PagamentiController usa [Route("[controller]/[action]")], quindi la rotta è /Pagamenti/NomeAction.
public class PagamentiClient(HttpClient httpClient) : IPagamentiClient
{
    public async Task CreateVersamentoAsync(VersamentoInsertDto versamento, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("/Pagamenti/CreateVersamento", versamento, cancellationToken);

        // L'action risponde Ok() senza corpo: basta verificare l'esito
        response.EnsureSuccessStatusCode();
    }
}
