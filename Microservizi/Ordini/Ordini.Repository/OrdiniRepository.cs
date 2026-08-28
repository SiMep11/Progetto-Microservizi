using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ordini.Repository.Abstractions;
using Ordini.Repository.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace Ordini.Repository
{
    // Costruttore Primario C# 12
    public class OrdiniRepository(OrdiniDbContext context) : IOrdiniRepository
    {
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<Ordine>> GetAllOrdiniAsync(CancellationToken cancellationToken = default)
        {
            return await context.Ordini
                .Include(o => o.Prodotti)
                .ToListAsync(cancellationToken);
        }

        public async Task<Ordine?> GetOrdineByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await context.Ordini
                .Include(o => o.Prodotti)
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        }

        public async Task CreaOrdineAsync(Ordine ordine, CancellationToken cancellationToken = default)
        {
            await context.Ordini.AddAsync(ordine, cancellationToken);
        }

        public async Task AggiornaStatoOrdineAsync(int id, string nuovoStato, CancellationToken cancellationToken = default)
        {
            var ordine = await context.Ordini.FindAsync(new object[] { id }, cancellationToken);
            if (ordine != null)
            {
                ordine.Stato = nuovoStato;
            }
        }

        public async Task BeginTransactionAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken = default)
        {
            if (context.Database.CurrentTransaction != null)
            {
                // Siamo già dentro una transazione: non ne apriamo un'altra
                await action(cancellationToken);
                return;
            }

            // Con EnableRetryOnFailure attivo, EF pretende che le transazioni esplicite
            // passino dalla execution strategy, così l'intero blocco è ritentabile.
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
    }
}