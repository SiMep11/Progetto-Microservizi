using System;
using System.Collections.Generic;
using System.Text;

namespace Ordini.Repository.Models
{
    public class Ordine
    {
        public int Id { get; set; }
        public DateTime DataOrdine { get; set; } = DateTime.UtcNow;
        public string Cliente { get; set; } = string.Empty;
        public decimal Totale { get; set; }
        public string Stato { get; set; } = "In elaborazione";

        // Usiamo ICollection per Entity Framework
        public ICollection<ProdottoOrdinato> Prodotti { get; set; } = new List<ProdottoOrdinato>();
    }
}
