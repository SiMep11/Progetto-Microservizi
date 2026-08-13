using Magazzino.Repository.Abstraction;
using Magazzino.Repository.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

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
            // 1. Cerchiamo il prodotto nel database
            var prodotto = await context.Prodotti.FindAsync(new object[] { id }, cancellationToken);

            if (prodotto != null)
            {
                // 2. Scaliamo la quantità
                prodotto.QuantitaInMagazzino -= quantita;

                // 3. Sicurezza: evitiamo giacenze negative
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
    }
}
