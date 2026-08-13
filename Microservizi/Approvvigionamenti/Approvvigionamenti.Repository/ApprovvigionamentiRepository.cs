using Approvvigionamenti.Repository.Abstraction;
using Approvvigionamenti.Repository.Models;
using Approvvigionamenti.Shared;
using Microsoft.EntityFrameworkCore;

namespace Approvvigionamenti.Repository
{
    public class ApprovvigionamentiRepository(ApprovvigionamentiDbContext context) : IApprovvigionamentiRepository
    {
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await context.SaveChangesAsync(cancellationToken);
        }

        #region Fornitori

        public async Task<IEnumerable<Fornitore>> GetAllFornitoriAsync(CancellationToken cancellationToken = default)
        {
            return await context.Fornitori
                .Include(f => f.ProdottiForniti)
                .ToListAsync(cancellationToken);
        }

        public async Task<Fornitore?> GetFornitoreByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await context.Fornitori
                .Include(f => f.ProdottiForniti)
                .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
        }

        public async Task CreaFornitoreAsync(Fornitore fornitore, CancellationToken cancellationToken = default)
        {
            await context.Fornitori.AddAsync(fornitore, cancellationToken);
        }

        #endregion

        #region ProdottiForniti

        public async Task AggiungiProdottoFornitoAsync(ProdottoFornito prodottoFornito, CancellationToken cancellationToken = default)
        {
            await context.ProdottiForniti.AddAsync(prodottoFornito, cancellationToken);
        }

        public async Task<ProdottoFornito?> GetProdottoFornitoAsync(int prodottoId, CancellationToken cancellationToken = default)
        {
            return await context.ProdottiForniti
                .FirstOrDefaultAsync(p => p.ProdottoId == prodottoId, cancellationToken);
        }

        #endregion

        #region OrdiniFornitore

        public async Task<IEnumerable<OrdineFornitore>> GetAllOrdiniFornitoreAsync(CancellationToken cancellationToken = default)
        {
            return await context.OrdiniFornitore.ToListAsync(cancellationToken);
        }

        public async Task<OrdineFornitore?> GetOrdineFornitoreByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await context.OrdiniFornitore.FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        }

        public async Task CreaOrdineFornitoreAsync(OrdineFornitore ordine, CancellationToken cancellationToken = default)
        {
            await context.OrdiniFornitore.AddAsync(ordine, cancellationToken);
        }

        public async Task<bool> EsisteOrdineApertoAsync(int prodottoId, CancellationToken cancellationToken = default)
        {
            return await context.OrdiniFornitore
                .AnyAsync(o => o.ProdottoId == prodottoId && o.Stato == StatiOrdineFornitore.Richiesto, cancellationToken);
        }

        #endregion
    }
}