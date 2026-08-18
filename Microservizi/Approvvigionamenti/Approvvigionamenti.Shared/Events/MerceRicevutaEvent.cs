namespace Approvvigionamenti.Shared.Events
{
    /// <summary>
    /// Pubblicato quando un ordine al fornitore viene segnato come ricevuto: è il via libera per Magazzino a ricaricare la giacenza.
    /// </summary>
    public class MerceRicevutaEvent
    {
        public int OrdineFornitoreId { get; set; }
        public int ProdottoId { get; set; }
        public int Quantita { get; set; }
    }
}