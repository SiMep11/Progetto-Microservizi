namespace Magazzino.Shared.Events
{
    /// <summary>
    /// Ricevuto da Ordini quando un ordine è stato pagato per l'importo corretto: è il via libera per scaricare la giacenza.
    /// </summary>
    public class OrdineConfermatoEvent
    {
        public int OrdineId { get; set; }
        public List<DettaglioOrdineEvent> Prodotti { get; set; } = new();
    }

    public class DettaglioOrdineEvent
    {
        public int ProdottoId { get; set; }
        public int Quantita { get; set; }
    }
}
