using Pagamenti.Repository.Model;
using Pagamenti.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pagamenti.Repository.Abstraction
{
    public interface IRepository
    {
        Task BeginTransactionAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken = default);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        Task<Versamento> CreateVersamentoAsync(VersamentoInsertDto versamentoInsertDto, CancellationToken cancellationToken = default);

        #region TransactionalOutbox

        Task<IEnumerable<TransactionalOutbox>> GetAllTransactionalOutboxAsync(CancellationToken cancellationToken = default);

        Task<TransactionalOutbox?> GetTransactionalOutboxByKeyAsync(long id, CancellationToken cancellationToken = default);

        Task DeleteTransactionalOutboxAsync(long id, CancellationToken cancellationToken = default);

        Task InsertTransactionalOutboxAsync(TransactionalOutbox transactionalOutbox, CancellationToken cancellationToken = default);

        #endregion
    }
}
