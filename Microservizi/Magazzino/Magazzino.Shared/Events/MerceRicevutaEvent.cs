namespace Magazzino.Shared.Events
{
    /// <summary>
    /// Ricevuto da Approvvigionamenti quando la merce ordinata al fornitore è arrivata: è il via libera per ricaricare la giacenza.
    /// </summary>
    public class MerceRicevutaEvent
    {
        public int OrdineFornitoreId { get; set; }
        public int ProdottoId { get; set; }
        public int Quantita { get; set; }
    }
}