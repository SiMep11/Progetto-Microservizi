using System;
using System.Collections.Generic;
using System.Text;

namespace Ordini.Shared
{
    public class OrdineDto
    {
        public int Id { get; set; }
        public DateTime DataOrdine { get; set; } = DateTime.UtcNow;
        public string Cliente { get; set; } = string.Empty; // Nome o ID dell'utente
        public decimal Totale { get; set; }
        public string Stato { get; set; } = string.Empty; // Es: In elaborazione, Completato, Rifiutato

        // Relazione 1 a N: un ordine contiene tanti prodotti
        public List<ProdottoOrdinatoDto> Prodotti { get; set; } = new List<ProdottoOrdinatoDto>();
    }
}
