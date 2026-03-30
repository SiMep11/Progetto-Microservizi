using System;
using System.Collections.Generic;
using System.Text;

namespace Pagamenti.Repository.Model
{
    public class TransactionalOutbox
    {
        public long Id { get; set; }
        public string Tabella { get; set; } = string.Empty; // Il nome dell'entità o del topic
        public string Messaggio { get; set; } = string.Empty; // Il JSON del messaggio
    }
}
