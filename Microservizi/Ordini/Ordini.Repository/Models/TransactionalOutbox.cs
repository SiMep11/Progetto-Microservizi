namespace Ordini.Repository.Models
{
    /// <summary>
    /// il messaggio da pubblicare su Kafka viene scritto qui nella stessa transazione del dato di business, e spedito dopo da un processo separato.
    ///  Serve perché database e broker sono due sistemi diversi e non esiste una transazione che li copra entrambi.
    /// </summary>
    public class TransactionalOutbox
    {
        public long Id { get; set; }
        public string Tabella { get; set; } = string.Empty;   // discriminante per scegliere il topic
        public string Messaggio { get; set; } = string.Empty; // il JSON dell'evento
    }
}
