using Magazzino.Repository.Models;
using Magazzino.Shared.Events;
using System.Text.Json;

namespace Magazzino.Business.Factory
{
    public static class TransactionalOutboxFactory
    {
        public static TransactionalOutbox Create(ScortaSottoSogliaEvent evento) => new TransactionalOutbox
        {
            Tabella = nameof(ScortaSottoSogliaEvent),
            Messaggio = JsonSerializer.Serialize(evento)
        };
    }
}