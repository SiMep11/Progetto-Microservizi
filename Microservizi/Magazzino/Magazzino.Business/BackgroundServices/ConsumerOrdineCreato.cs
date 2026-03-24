using Confluent.Kafka;
using Magazzino.Business.Abstraction;
using Magazzino.Shared.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

namespace Magazzino.Business.BackgroundServices
{
    public class ConsumerOrdineCreato : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConsumer<Ignore, string> _consumer;

        public ConsumerOrdineCreato(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092", // Assicurati che Docker con Kafka sia acceso!
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
                var consumeResult = _consumer.Consume(stoppingToken);
                var evento = JsonSerializer.Deserialize<OrdineCreatoEvent>(consumeResult.Message.Value);

                using (var scope = _serviceProvider.CreateScope())
                {
                    var manager = scope.ServiceProvider.GetRequiredService<IProdottiManager>();

                    foreach (var p in evento.Prodotti)
                    {
                        // Qui chiameremo un nuovo metodo del manager per scalare la giacenza
                        await manager.ScaricaGiacenzaAsync(p.ProdottoId, p.Quantita);
                    }
                }
            }
        }
    }
}