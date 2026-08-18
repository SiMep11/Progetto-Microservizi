using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ordini.Repository.Models;

namespace Ordini.Repository.Abstractions
{
    public interface IOrdiniRepository
    {
        // Il nuovo comando manuale per salvare!
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<Ordine>> GetAllOrdiniAsync(CancellationToken cancellationToken = default);

        Task<Ordine?> GetOrdineByIdAsync(int id, CancellationToken cancellationToken = default);

        Task CreaOrdineAsync(Ordine ordine, CancellationToken cancellationToken = default);

        Task AggiornaStatoOrdineAsync(int id, string nuovoStato, CancellationToken cancellationToken = default);
    }
}