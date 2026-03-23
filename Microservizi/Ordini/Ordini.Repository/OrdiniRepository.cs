using System;
using System.Collections.Generic;
using System.Text;
using Ordini.Repository.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Ordini.Repository.Models;

namespace Ordini.Repository
{
    public class OrdiniRepository : IOrdiniRepository
    {
        private readonly OrdiniDbContext _context;

        public OrdiniRepository(OrdiniDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Ordine>> GetAllOrdiniAsync()
        {
            // Include(o => o.Prodotti) è fondamentale per caricare anche le righe dell'ordine!
            return await _context.Ordini
                .Include(o => o.Prodotti)
                .ToListAsync();
        }

        public async Task<Ordine?> GetOrdineByIdAsync(int id)
        {
            return await _context.Ordini
                .Include(o => o.Prodotti)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Ordine> CreaOrdineAsync(Ordine ordine)
        {
            _context.Ordini.Add(ordine);
            await _context.SaveChangesAsync();
            return ordine;
        }

        public async Task AggiornaStatoOrdineAsync(int id, string nuovoStato)
        {
            var ordine = await _context.Ordini.FindAsync(id);
            if (ordine != null)
            {
                ordine.Stato = nuovoStato;
                await _context.SaveChangesAsync();
            }
        }
    }
}
