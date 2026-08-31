using Approvvigionamenti.Repository.Abstraction;
using Approvvigionamenti.Repository.Models;
using Approvvigionamenti.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Approvvigionamenti.Repository
{
    public class ApprovvigionamentiRepository(ApprovvigionamentiDbContext context) : IApprovvigionamentiRepository
    {
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await context.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken = default)
        {
            if (context.Database.CurrentTransaction != null)
            {
                // Siamo già dentro una transazione: non ne apriamo un'altra
                await action(cancellationToken);
                return;
            }

            // Approvvigionamenti ha EnableRetryOnFailure, che EF non permette di combinare
            // con transazioni esplicite: passando dalla execution strategy l'intero blocco
            // diventa ritentabile e le due cose convivono.
            var strategy = context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                await using IDbContextTransaction transaction = await context.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    await action(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);
                }
                catch
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            });
        }

        #region TransactionalOutbox

        public async Task<IEnumerable<TransactionalOutbox>> GetAllTransactionalOutboxAsync(CancellationToken cancellationToken = default)
            => await context.TransactionalOutboxList.ToListAsync(cancellationToken);

        public async Task<TransactionalOutbox?> GetTransactionalOutboxByKeyAsync(long id, CancellationToken cancellationToken = default)
            => await context.TransactionalOutboxList.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public async Task InsertTransactionalOutboxAsync(TransactionalOutbox transactionalOutbox, CancellationToken cancellationToken = default)
        {
            await context.TransactionalOutboxList.AddAsync(transactionalOutbox, cancellationToken);
        }

        public async Task DeleteTransactionalOutboxAsync(long id, CancellationToken cancellationToken = default)
        {
            context.TransactionalOutboxList.Remove(
                (await GetTransactionalOutboxByKeyAsync(id, cancellationToken))
                ?? throw new ArgumentException($"TransactionalOutbox con id {id} non trovato", nameof(id)));
        }

        #endregion

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