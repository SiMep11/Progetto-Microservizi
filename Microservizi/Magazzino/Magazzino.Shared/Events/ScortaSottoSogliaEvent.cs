namespace Magazzino.Shared.Events
{
    /// <summary>
    /// Pubblicato quando la giacenza di un prodotto scende sotto la scorta minima: è la richiesta di riordino verso Approvvigionamenti.
    /// </summary>
    public class ScortaSottoSogliaEvent
    {
        public int ProdottoId { get; set; }
        public int QuantitaAttuale { get; set; }
        public int ScortaMinima { get; set; }
    }
}