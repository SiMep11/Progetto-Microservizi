using Magazzino.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Magazzino.Repository.Models;

namespace Magazzino.Business
{
    public  class ProdottiManager
    {
        private readonly MagazzinoDbContext _context;

        // Tramite Dependency Injection, si fa passare il DbContext
        public ProdottiManager(MagazzinoDbContext context)
        {
            _context = context;
        }

        public async Task<List<Prodotto>> RecuperaTuttiAsync()
        {
            return await _context.Prodotti.ToListAsync();
        }

        public async Task<Prodotto> CreaProdottoAsync(Prodotto prodotto)
        {
            _context.Prodotti.Add(prodotto);
            await _context.SaveChangesAsync();
            return prodotto;
        }
    }
}
