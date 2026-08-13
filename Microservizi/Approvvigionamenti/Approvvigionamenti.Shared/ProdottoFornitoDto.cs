namespace Approvvigionamenti.Shared
{
    public class ProdottoFornitoDto
    {
        public int Id { get; set; }
        public int FornitoreId { get; set; }

        // Riferimento al Magazzino (NON è una chiave esterna nel nostro DB)
        public int ProdottoId { get; set; }

        // A che prezzo questo fornitore ce lo vende
        public decimal PrezzoAcquisto { get; set; }

        // Quanti pezzi ordinare quando scatta il riordino automatico
        public int QuantitaRiordino { get; set; }
    }
}