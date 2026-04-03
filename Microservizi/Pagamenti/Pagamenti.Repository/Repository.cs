using Microsoft.EntityFrameworkCore.Storage;
using Pagamenti.Repository.Abstraction;
using Pagamenti.Repository.Model;
using Pagamenti.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Pagamenti.Repository
{
    public class Repository(PagamentiDbContext pagamentiDbContext) : IRepository
    {
        public async Task BeginTransactionAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken = default)
        {
            if (pagamentiDbContext.Database.CurrentTransaction != null)
            {
                // La connessione è già in una transazione
                await action(cancellationToken);
            }
            else
            {
                // Viene avviata una transazione
                await using IDbContextTransaction transaction = await pagamentiDbContext.Database.BeginTransactionAsync(cancellationToken);
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
            }
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await pagamentiDbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<Versamento> CreateVersamentoAsync(VersamentoInsertDto versamentoInsertDto, CancellationToken cancellationToken = default)
        {
            Versamento versamento = new Versamento();
            versamento.DataVersamento = versamentoInsertDto.DataVersamento;
            versamento.DataContabile = versamentoInsertDto.DataContabile;
            versamento.Importo = versamentoInsertDto.Importo;
            versamento.Informazioni = versamentoInsertDto.Informazioni;

            await pagamentiDbContext.Versamenti.AddAsync(versamento, cancellationToken);

            return versamento;
        }

        #region TransactionalOutbox

        public async Task<IEnumerable<TransactionalOutbox>> GetAllTransactionalOutboxAsync(CancellationToken cancellationToken = default) => await pagamentiDbContext.TransactionalOutboxList.ToListAsync(cancellationToken);

        public async Task<TransactionalOutbox?> GetTransactionalOutboxByKeyAsync(long id, CancellationToken cancellationToken = default)
        {
            return await pagamentiDbContext.TransactionalOutboxList.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task DeleteTransactionalOutboxAsync(long id, CancellationToken cancellationToken = default)
        {
            pagamentiDbContext.TransactionalOutboxList.Remove(
                (await GetTransactionalOutboxByKeyAsync(id, cancellationToken)) ??
                throw new ArgumentException($"TransactionalOutbox con id {id} non trovato", nameof(id)));
        }

        public async Task InsertTransactionalOutboxAsync(TransactionalOutbox transactionalOutbox, CancellationToken cancellationToken = default)
        {
            await pagamentiDbContext.TransactionalOutboxList.AddAsync(transactionalOutbox);
        }

        #endregion
    }
}
