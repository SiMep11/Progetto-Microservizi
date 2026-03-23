using Magazzino.Repository.Abstraction;
using Magazzino.Repository.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Magazzino.Repository
{
    public class MagazzinoRepository : IMagazzinoRepository
    {
        private readonly MagazzinoDbContext _context;

        public MagazzinoRepository(MagazzinoDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Prodotto>> GetAllProdottiAsync()
        {
            return await _context.Prodotti.ToListAsync();
        }

        public async Task<Prodotto?> GetProdottoByIdAsync(int id)
        {
            return await _context.Prodotti.FindAsync(id);
        }

        public async Task<Prodotto> CreaProdottoAsync(Prodotto prodotto)
        {
            _context.Prodotti.Add(prodotto);
            await _context.SaveChangesAsync();
            return prodotto;
        }
    }
}
