using System;
using System.Collections.Generic;
using System.Text;
using Ordini.Repository.Models;

namespace Ordini.Repository.Abstractions
{
    public interface IOrdiniRepository
    {
        // Ottiene tutti gli ordini (con i relativi prodotti inclusi)
        Task<IEnumerable<Ordine>> GetAllOrdiniAsync();

        // Ottiene un ordine specifico tramite ID
        Task<Ordine?> GetOrdineByIdAsync(int id);

        // Salva un nuovo ordine nel database
        Task<Ordine> CreaOrdineAsync(Ordine ordine);

        // Aggiorna lo stato di un ordine (es: da "In elaborazione" a "Completato")
        Task AggiornaStatoOrdineAsync(int id, string nuovoStato);
    }
}
