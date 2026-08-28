using Ordini.Repository.Models;
using Ordini.Shared.Events;
using System.Text.Json;

namespace Ordini.Business.Factory
{
    public static class TransactionalOutboxFactory
    {
        public static TransactionalOutbox Create(OrdineConfermatoEvent evento) => new TransactionalOutbox
        {
            Tabella = nameof(OrdineConfermatoEvent),
            Messaggio = JsonSerializer.Serialize(evento)
        };
    }
}