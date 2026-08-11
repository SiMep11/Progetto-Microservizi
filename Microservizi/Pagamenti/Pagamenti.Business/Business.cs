using AutoMapper;
using Microsoft.Extensions.Logging;
using Pagamenti.Business.Abstraction;
using Pagamenti.Repository;
using Pagamenti.Repository.Abstraction;
using Pagamenti.Shared;
using Pagamenti.Business.Factory;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pagamenti.Business
{
    public class Business(IRepository repository, ILogger<Business> logger, IMapper map, IPagamentiObserver observer) : IBusiness
    {
        public async Task CreateVersamentoAsync(VersamentoInsertDto versamentoInsertDto, CancellationToken cancellationToken = default)
        {
            if (versamentoInsertDto.ordineId <= 0)
                throw new ArgumentException("Il versamento deve essere collegato a un ordine valido", nameof(versamentoInsertDto.ordineId));

            if (versamentoInsertDto.Importo <= 0)
                throw new ArgumentException("L'importo del versamento deve essere positivo", nameof(versamentoInsertDto.Importo));

            int idVersamentoCreato = 0;
            await repository.BeginTransactionAsync(async (CancellationToken cancellation) =>
            {
                var versamento = await repository.CreateVersamentoAsync(versamentoInsertDto, cancellation);

                await repository.SaveChangesAsync(cancellation); // Save intermedia per recuperare l'Id: è autogenerato dal DB e al termine di questa chiamata viene compilato automaticamente

                var newVersamentoRecord = map.Map<VersamentoReadDto>(versamento); // travasiamo il record su un dto (newVersamentoRecord conterrà anche l'Id)

                await repository.InsertTransactionalOutboxAsync(TransactionalOutboxFactory.CreateInsert(newVersamentoRecord), cancellation);

                await repository.SaveChangesAsync(cancellation);

                idVersamentoCreato = newVersamentoRecord.Id;
            }, cancellationToken);

            if(idVersamentoCreato > 0)
                observer.NuovoVersamento.OnNext(idVersamentoCreato);
        }
    }
}
