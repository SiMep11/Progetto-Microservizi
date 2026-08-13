using Approvvigionamenti.Shared;

namespace Approvvigionamenti.ClientHttp.Abstraction;

public interface IApprovvigionamentiClient
{
    Task<IEnumerable<FornitoreDto>> GetAllFornitoriAsync(CancellationToken cancellationToken = default);

    Task<FornitoreDto> CreaFornitoreAsync(FornitoreDto fornitore, CancellationToken cancellationToken = default);

    Task<IEnumerable<OrdineFornitoreDto>> GetAllOrdiniFornitoreAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Segna la merce come arrivata: chiude l'ordine e fa ricaricare la giacenza.
    /// </summary>
    Task ConfermaRicezioneAsync(int ordineFornitoreId, CancellationToken cancellationToken = default);
}