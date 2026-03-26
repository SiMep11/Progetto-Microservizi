using Confluent.Kafka;
using Magazzino.Business.Abstraction;
using Magazzino.Shared.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

namespace Magazzino.Business.BackgroundServices
{
    public class ConsumerOrdineCreato : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConsumer<Ignore, string> _consumer;

        public ConsumerOrdineCreato(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            var bootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092";

            var config = new ConsumerConfig
            {
                BootstrapServers = bootstrapServers, 
                GroupId = "magazzino-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.Subscribe("ordine-creato");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Spostiamo tutto dentro il try!
                    var consumeResult = _consumer.Consume(stoppingToken);
                    var evento = JsonSerializer.Deserialize<OrdineCreatoEvent>(consumeResult.Message.Value);

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var manager = scope.ServiceProvider.GetRequiredService<IProdottiManager>();

                        foreach (var p in evento.Prodotti)
                        {
                            await manager.ScaricaGiacenzaAsync(p.ProdottoId, p.Quantita);
                            Console.WriteLine($"[MAGAZZINO] Scalati {p.Quantita} pezzi per il prodotto {p.ProdottoId}!");
                        }
                    }
                }
                catch (ConsumeException ex)
                {
                    // Se il topic non esiste, Kafka si lamenta. Noi lo ignoriamo e aspettiamo!
                    Console.WriteLine($"[KAFKA] Canale non ancora pronto, in attesa... ({ex.Error.Reason})");
                    await Task.Delay(2000, stoppingToken); // Aspetta 2 secondi e riprova
                }
                catch (Exception ex)
                {
                    // Qualsiasi altro errore strano non farà crashare il server
                    Console.WriteLine($"[KAFKA] Errore di lettura: {ex.Message}");
                    await Task.Delay(2000, stoppingToken);
                }
            }
        }
    }
}