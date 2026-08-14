namespace Ordini.Shared
{
    /// <summary>
    /// Riga d'ordine in ingresso. Niente PrezzoUnitario: il prezzo è un dato che appartiene al Magazzino e non può essere deciso dal chiamante.
    /// </summary>
    public class ProdottoOrdinatoInsertDto
    {
        public int ProdottoId { get; set; }
        public int Quantita { get; set; }
    }
}