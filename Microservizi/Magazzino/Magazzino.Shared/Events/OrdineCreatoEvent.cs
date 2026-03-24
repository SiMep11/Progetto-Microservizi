namespace Magazzino.Shared.Events
{
    public class OrdineCreatoEvent
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