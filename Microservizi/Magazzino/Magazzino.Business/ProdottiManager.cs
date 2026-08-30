using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Magazzino.Business.Abstraction;
using Magazzino.Business.Kafka;
using Magazzino.Repository.Abstraction;
using Magazzino.Repository.Models;
using Magazzino.Shared;
using Magazzino.Shared.Events;
using Microsoft.Extensions.Logging;
using Magazzino.Business.Factory;

namespace Magazzino.Business
{
    public class ProdottiManager(
        IMagazzinoRepository repository,
        IMagazzinoObserver observer,
        ILogger<ProdottiManager> logger) : IProdottiManager
    {
        public async Task<IEnumerable<ProdottoDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var prodottiDb = await repository.GetAllProdottiAsync(cancellationToken);
            return prodottiDb.Select(Map);
        }

        public async Task<ProdottoDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var prodottoDb = await repository.GetProdottoByIdAsync(id, cancellationToken);
            return prodottoDb == null ? null : Map(prodottoDb);
        }

        public async Task<ProdottoDto> CreaProdottoAsync(ProdottoDto prodottoDto, CancellationToken cancellationToken = default)
        {
            var nuovoProdotto = new Prodotto
            {
                Nome = prodottoDto.Nome,
                Descrizione = prodottoDto.Descrizione,
                Prezzo = prodottoDto.Prezzo,
                QuantitaInMagazzino = prodottoDto.QuantitaDisponibile,
                ScortaMinima = prodottoDto.ScortaMinima
            };

            await repository.CreaProdottoAsync(nuovoProdotto, cancellationToken);
            
            await repository.SaveChangesAsync(cancellationToken);

            prodottoDto.Id = nuovoProdotto.Id;
            return prodottoDto;
        }

        public async Task ScaricaGiacenzaAsync(int id, int quantita, CancellationToken cancellationToken = default)
        {
            long idOutbox = 0;

            await repository.BeginTransactionAsync(async (CancellationToken ct) =>
            {
                var prodotto = await repository.ScaricaGiacenzaAsync(id, quantita, ct);

                if (prodotto is null)
                {
                    logger.LogWarning("Scarico giacenza: prodotto {ProdottoId} non trovato", id);
                    return;
                }

                TransactionalOutbox? outbox = null;

                // Se siamo scesi sotto la scorta minima prepariamo la richiesta di riordino
                if (prodotto.QuantitaInMagazzino < prodotto.ScortaMinima)
                {
                    var evento = new ScortaSottoSogliaEvent
                    {
                        ProdottoId = prodotto.Id,
                        QuantitaAttuale = prodotto.QuantitaInMagazzino,
                        ScortaMinima = prodotto.ScortaMinima
                    };

                    outbox = TransactionalOutboxFactory.Create(evento);
                    await repository.InsertTransactionalOutboxAsync(outbox, ct);

                    logger.LogInformation("Prodotto {ProdottoId} sceso a {Quantita}, sotto la soglia {Soglia}: richiesto riordino",
                        prodotto.Id, prodotto.QuantitaInMagazzino, prodotto.ScortaMinima);
                }

                // UN SOLO salvataggio: giacenza aggiornata ed eventuale messaggio
                // vengono committati insieme, o non viene committato nulla
                await repository.SaveChangesAsync(ct);

                if (outbox is not null)
                    idOutbox = outbox.Id;

            }, cancellationToken);

            // Svegliamo il producer: pubblicherà subito invece di attendere il giro successivo
            if (idOutbox > 0)
                observer.NuovoMessaggioOutbox.OnNext(idOutbox);
        }

        public async Task CaricaGiacenzaAsync(int id, int quantita, CancellationToken cancellationToken = default)
        {
            var prodotto = await repository.CaricaGiacenzaAsync(id, quantita, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            if (prodotto is null)
            {
                logger.LogWarning("Carico giacenza: prodotto {ProdottoId} non trovato", id);
                return;
            }

            logger.LogInformation("Prodotto {ProdottoId} ricaricato di {Quantita} pezzi, giacenza ora {Giacenza}",
                prodotto.Id, quantita, prodotto.QuantitaInMagazzino);
        }

        private static ProdottoDto Map(Prodotto p) => new ProdottoDto
        {
            Id = p.Id,
            Nome = p.Nome,
            Descrizione = p.Descrizione,
            Prezzo = p.Prezzo,
            QuantitaDisponibile = p.QuantitaInMagazzino,
            ScortaMinima = p.ScortaMinima
        };
    }
}