using Magazzino.Shared;
using Ordini.ClientHttp.Abstractions;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Ordini.ClientHttp
{
    public class MagazzinoClient : IMagazzinoClient
    {
        private readonly HttpClient _httpClient;

        public MagazzinoClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ProdottoDto?> GetProdottoByIdAsync(int id)
        {
            // Facciamo una richiesta GET all'API del Magazzino
            var response = await _httpClient.GetAsync($"/api/prodotti/{id}");

            if (response.IsSuccessStatusCode)
            {
                // Se il magazzino risponde 200 OK, "spacchettiamo" il JSON nel DTO
                return await response.Content.ReadFromJsonAsync<ProdottoDto>();
            }

            // Se il prodotto non esiste (404) o c'è un errore, restituiamo null
            return null;
        }
    }
}
