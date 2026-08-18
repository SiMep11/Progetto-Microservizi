using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ordini.Repository.Abstractions;
using Ordini.Repository.Models;

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
    }
}