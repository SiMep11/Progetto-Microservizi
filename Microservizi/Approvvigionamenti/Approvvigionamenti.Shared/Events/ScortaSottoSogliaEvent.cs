namespace Approvvigionamenti.Shared.Events
{
    /// <summary>
    /// Ricevuto da Magazzino quando la giacenza di un prodotto scende sotto la scorta
    /// ScortaMinima: è il segnale che fa scattare il riordino automatico.
    /// </summary>
    public class ScortaSottoSogliaEvent
    {
        public int ProdottoId { get; set; }
        public int QuantitaAttuale { get; set; }
        public int ScortaMinima { get; set; }
    }
}