using Magazzino.Repository.Abstraction;
using Magazzino.Repository.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Magazzino.Repository
{
    public class MagazzinoRepository(MagazzinoDbContext context) : IMagazzinoRepository
    {
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<Prodotto>> GetAllProdottiAsync(CancellationToken cancellationToken = default)
        {
            return await context.Prodotti.ToListAsync(cancellationToken);
        }

        public async Task<Prodotto?> GetProdottoByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await context.Prodotti.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task CreaProdottoAsync(Prodotto prodotto, CancellationToken cancellationToken = default)
        {
            await context.Prodotti.AddAsync(prodotto, cancellationToken);
        }

        public async Task<Prodotto?> ScaricaGiacenzaAsync(int id, int quantita, CancellationToken cancellationToken = default)
        {
            // 1. Cerco il prodotto nel database
            var prodotto = await context.Prodotti.FindAsync(new object[] { id }, cancellationToken);

            if (prodotto != null)
            {
                // 2. Scalo la quantità
                prodotto.QuantitaInMagazzino -= quantita;

                // 3. Sicurezza: evito giacenze negative
                if (prodotto.QuantitaInMagazzino < 0)
                {
                    prodotto.QuantitaInMagazzino = 0;
                }
            }

            return prodotto;
        }

        public async Task<Prodotto?> CaricaGiacenzaAsync(int id, int quantita, CancellationToken cancellationToken = default)
        {
            var prodotto = await context.Prodotti.FindAsync(new object[] { id }, cancellationToken);

            if (prodotto != null)
            {
                prodotto.QuantitaInMagazzino += quantita;
            }

            return prodotto;
        }

        public async Task BeginTransactionAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken = default)
        {
            if (context.Database.CurrentTransaction != null)
            {
                // Siamo già dentro una transazione: non ne apriamo un'altra
                await action(cancellationToken);
                return;
            }

            // Magazzino ha EnableRetryOnFailure, che EF non permette di combinare con
            // transazioni esplicite: passando dalla execution strategy l'intero blocco
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
    }
}
