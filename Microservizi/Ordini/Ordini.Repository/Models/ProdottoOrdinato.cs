using System;
using System.Collections.Generic;
using System.Text;

namespace Ordini.Repository.Models
{
    public class ProdottoOrdinato
    {
        public int Id { get; set; }
        public int OrdineId { get; set; }
        public Ordine Ordine { get; set; } = null!; // Relazione di navigazione

        public int ProdottoId { get; set; }
        public int Quantita { get; set; }
        public decimal PrezzoUnitario { get; set; }
    }
}
