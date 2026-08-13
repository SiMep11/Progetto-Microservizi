namespace Approvvigionamenti.Repository.Models
{
    /// <summary>
    /// Dice quali prodotti del magazzino un fornitore è in grado di rifornire,
    /// a che prezzo e in che quantità conviene riordinare.
    /// </summary>
    public class ProdottoFornito
    {
        public int Id { get; set; }

        public int FornitoreId { get; set; }
        public Fornitore? Fornitore { get; set; }

        // Riferimento al Magazzino (NON è una vera chiave esterna nel nostro DB)
        public int ProdottoId { get; set; }

        public decimal PrezzoAcquisto { get; set; }
        public int QuantitaRiordino { get; set; }
    }
}