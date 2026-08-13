using Approvvigionamenti.Shared;

namespace Approvvigionamenti.Repository.Models
{
    public class OrdineFornitore
    {
        public int Id { get; set; }

        public int FornitoreId { get; set; }
        public Fornitore? Fornitore { get; set; }

        public int ProdottoId { get; set; }
        public int Quantita { get; set; }
        public decimal PrezzoUnitario { get; set; }

        public DateTime DataOrdine { get; set; } = DateTime.UtcNow;
        public DateTime? DataRicezione { get; set; }
        public string Stato { get; set; } = StatiOrdineFornitore.Richiesto;
    }
}