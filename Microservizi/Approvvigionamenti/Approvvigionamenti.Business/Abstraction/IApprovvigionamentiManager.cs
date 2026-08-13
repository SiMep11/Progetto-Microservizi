using Approvvigionamenti.Shared;

namespace Approvvigionamenti.Business.Abstraction
{
    public interface IApprovvigionamentiManager
    {
        #region Fornitori

        Task<IEnumerable<FornitoreDto>> GetAllFornitoriAsync(CancellationToken cancellationToken = default);
        Task<FornitoreDto?> GetFornitoreByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<FornitoreDto> CreaFornitoreAsync(FornitoreDto fornitoreDto, CancellationToken cancellationToken = default);

        /// <summary>
        /// Associa un prodotto del magazzino a un fornitore, con prezzo e quantità di riordino.
        /// </summary>
        Task<ProdottoFornitoDto> AggiungiProdottoFornitoAsync(ProdottoFornitoDto prodottoFornitoDto, CancellationToken cancellationToken = default);

        #endregion

        #region OrdiniFornitore

        Task<IEnumerable<OrdineFornitoreDto>> GetAllOrdiniFornitoreAsync(CancellationToken cancellationToken = default);
        Task<OrdineFornitoreDto?> GetOrdineFornitoreByIdAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Ordine di acquisto creato a mano dall'operatore.
        /// </summary>
        Task<OrdineFornitoreDto> CreaOrdineFornitoreAsync(int prodottoId, int quantita, CancellationToken cancellationToken = default);

        /// <summary>
        /// La merce è arrivata: chiude l'ordine e avvisa il magazzino via Kafka.
        /// </summary>
        Task<bool> ConfermaRicezioneAsync(int ordineFornitoreId, CancellationToken cancellationToken = default);

        #endregion

        /// <summary>
        /// Riordino automatico, invocato dall'evento Kafka di scorta sotto soglia.
        /// </summary>
        Task RiordinaSeNecessarioAsync(int prodottoId, int quantitaAttuale, CancellationToken cancellationToken = default);
    }
}