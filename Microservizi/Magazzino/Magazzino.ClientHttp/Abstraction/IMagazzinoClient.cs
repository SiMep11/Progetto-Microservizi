using Magazzino.Shared; // Per usare ProdottoDto

namespace Magazzino.ClientHttp.Abstraction;

public interface IMagazzinoClient
{
    Task<ProdottoDto?> GetProdottoByIdAsync(int id, CancellationToken cancellationToken = default);

    // Un extra utile per avere tutto il catalogo
    Task<IEnumerable<ProdottoDto>> GetAllProdottiAsync(CancellationToken cancellationToken = default);

    // extra  per aggiungere un prodotto
    Task<ProdottoDto?> CreaProdottoAsync(ProdottoDto prodotto, CancellationToken cancellationToken = default);
}