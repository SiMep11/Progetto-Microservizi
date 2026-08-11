using AutoMapper;
using Microsoft.Extensions.Logging;
using Pagamenti.Business.Abstraction;
using Pagamenti.Repository;
using Pagamenti.Repository.Abstraction;
using Pagamenti.Shared;
using Pagamenti.Business.Factory;
using Ordini.ClientHttp.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pagamenti.Business
{
    public class Business(IRepository repository, ILogger<Business> logger, IMapper map, IPagamentiObserver observer, IOrdiniClient ordiniClient) : IBusiness
    {
        public async Task CreateVersamentoAsync(VersamentoInsertDto versamentoInsertDto, CancellationToken cancellationToken = default)
        {
            if (versamentoInsertDto.ordineId <= 0)
                throw new ArgumentException("Il versamento deve essere collegato a un ordine valido", nameof(versamentoInsertDto.ordineId));

            if (versamentoInsertDto.Importo <= 0)
                throw new ArgumentException("L'importo del versamento deve essere positivo", nameof(versamentoInsertDto.Importo));

            // Validazione SINCRONA (http client) verso Ordini: scartiamo subito un versamento
            // che non potrebbe mai confermare nulla, invece di farlo sparire dentro Kafka.
            // NB: la decisione autoritativa resta di Ordini, che la riprende sul messaggio Kafka.
            var ordine = await ordiniClient.GetOrdineByIdAsync(versamentoInsertDto.ordineId, cancellationToken);

            if (ordine is null)
            {
                logger.LogWarning("Versamento rifiutato: l'ordine {OrdineId} non esiste", versamentoInsertDto.ordineId);
                throw new ArgumentException($"L'ordine {versamentoInsertDto.ordineId} non esiste", nameof(versamentoInsertDto.ordineId));
            }

            if (versamentoInsertDto.Importo != ordine.Totale)
            {
                logger.LogWarning("Versamento rifiutato: importo {Importo} diverso dal totale {Totale} dell'ordine {OrdineId}",
                    versamentoInsertDto.Importo, ordine.Totale, versamentoInsertDto.ordineId);
                throw new ArgumentException($"L'importo versato ({versamentoInsertDto.Importo}) non corrisponde al totale dell'ordine ({ordine.Totale})", nameof(versamentoInsertDto.Importo));
            }

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
