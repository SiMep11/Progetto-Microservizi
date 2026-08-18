using Approvvigionamenti.Repository.Models;

namespace Approvvigionamenti.Repository.Abstraction
{
    public interface IApprovvigionamentiRepository
    {
        // Il salvataggio lo comanda il Business, come negli altri microservizi
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        #region Fornitori

        Task<IEnumerable<Fornitore>> GetAllFornitoriAsync(CancellationToken cancellationToken = default);
        Task<Fornitore?> GetFornitoreByIdAsync(int id, CancellationToken cancellationToken = default);
        Task CreaFornitoreAsync(Fornitore fornitore, CancellationToken cancellationToken = default);

        #endregion

        #region ProdottiForniti

        Task AggiungiProdottoFornitoAsync(ProdottoFornito prodottoFornito, CancellationToken cancellationToken = default);

        /// <summary>
        /// Restituisce il fornitore associato a un prodotto, con prezzo e quantità di riordino. È quello che il riordino automatico usa per sapere a chi ordinare e quanto. 
        /// Null se per quel prodotto non è stato configurato un fornitore.
        /// </summary>
        Task<ProdottoFornito?> GetProdottoFornitoAsync(int prodottoId, CancellationToken cancellationToken = default);

        #endregion

        #region OrdiniFornitore

        Task<IEnumerable<OrdineFornitore>> GetAllOrdiniFornitoreAsync(CancellationToken cancellationToken = default);
        Task<OrdineFornitore?> GetOrdineFornitoreByIdAsync(int id, CancellationToken cancellationToken = default);
        Task CreaOrdineFornitoreAsync(OrdineFornitore ordine, CancellationToken cancellationToken = default);

        /// <summary>
        /// Serve a non riordinare lo stesso prodotto a ogni evento di scorta bassa: se c'è già un ordine in attesa di consegna, non se ne crea un altro.
        /// </summary>
        Task<bool> EsisteOrdineApertoAsync(int prodottoId, CancellationToken cancellationToken = default);

        #endregion
    }
}