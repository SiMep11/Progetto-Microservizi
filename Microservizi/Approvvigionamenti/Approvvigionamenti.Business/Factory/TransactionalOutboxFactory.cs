using Approvvigionamenti.Repository.Models;
using Approvvigionamenti.Shared.Events;
using System.Text.Json;

namespace Approvvigionamenti.Business.Factory
{
    public static class TransactionalOutboxFactory
    {
        public static TransactionalOutbox Create(MerceRicevutaEvent evento) => new TransactionalOutbox
        {
            Tabella = nameof(MerceRicevutaEvent),
            Messaggio = JsonSerializer.Serialize(evento)
        };
    }
}