using Magazzino.Repository.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Magazzino.Repository.Abstraction
{
    public interface IMagazzinoRepository
    {
       // Il metodo per salvare i dati esposto al Business
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<Prodotto>> GetAllProdottiAsync(CancellationToken cancellationToken = default);

        Task<Prodotto?> GetProdottoByIdAsync(int id, CancellationToken cancellationToken = default);

        // Cambiato a Task (void) perché il salvataggio effettivo avverrà dopo
        Task CreaProdottoAsync(Prodotto prodotto, CancellationToken cancellationToken = default);

        // Restituisce il prodotto aggiornato: al Business serve per sapere se siamo scesi sotto la scorta minima
        Task<Prodotto?> ScaricaGiacenzaAsync(int id, int quantita, CancellationToken cancellationToken = default);

        Task<Prodotto?> CaricaGiacenzaAsync(int id, int quantita, CancellationToken cancellationToken = default);
    }
}
