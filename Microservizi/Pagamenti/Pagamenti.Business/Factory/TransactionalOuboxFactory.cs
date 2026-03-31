using Azure;
using Pagamenti.Repository.Model;
using Pagamenti.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using UtilityKafka.Constants;
using UtilityKafka.Messages:

namespace Pagamenti.Business.Factory
{
    public class TransactionalOuboxFactory
    {
        #region Versamenti

        public static TransactionalOutbox CreateInsert(VersamentoReadDto dto) => Create(dto, Operations.Insert);

        private static TransactionalOutbox Create(VersamentoReadDto dto, string operation) => Create(nameof(Versamento), dto, operation);

        #endregion

        private static TransactionalOutbox Create<TDTO>(string table, TDTO dto, string operation) where TDTO : class, new()
        {
            OperationMessage<TDTO> opMsg = new OperationMessage<TDTO>()
            {
                Dto = dto,
                Operation = operation
            };

            opMsg.CheckMessage();

            return new TransactionalOutbox()
            {
                Tabella = table,
                Messaggio = JsonSerializer.Serialize(opMsg)
            };
        }
    }
}
