using Approvvigionamenti.Business;
using Approvvigionamenti.Business.Abstraction;
using Approvvigionamenti.Business.Kafka;
using Approvvigionamenti.Business.Kafka.MessageHandlers;
using Approvvigionamenti.Repository;
using Approvvigionamenti.Repository.Abstraction;
using Microsoft.EntityFrameworkCore;
using Utility.Kafka.Abstractions.Clients;
using Utility.Kafka.Clients;

var builder = WebApplication.CreateBuilder(args);

// 1. Configura il DbContext per collegarsi a SQL Server
builder.Services.AddDbContext<ApprovvigionamentiDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("ApprovvigionamentiDb"),
        b =>
        {
            b.MigrationsAssembly("Approvvigionamenti.WebApi");
            b.EnableRetryOnFailure();
        }
    ));

// 2. Registra Repository e livello Business
builder.Services.AddScoped<IApprovvigionamentiRepository, ApprovvigionamentiRepository>();
builder.Services.AddScoped<IApprovvigionamentiManager, ApprovvigionamentiManager>();

// 3. Comunicazione sincrona verso Magazzino (verifica che i prodotti esistano)
builder.Services.AddMagazzinoClient(builder.Configuration);

// 4. Kafka: produciamo "merce-ricevuta" e consumiamo "scorta-sotto-soglia"
builder.Services.Configure<KafkaProducerClientOptions>(builder.Configuration.GetSection("Kafka"));
builder.Services.AddSingleton<IProducerClient<string, string>, ProducerClient>();
builder.Services.AddKafkaConsumerService<KafkaTopicsInput, ApprovvigionamentiMessageHandlerFactory>(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

var app = builder.Build();

// --- AUTO-MIGRATION ---
// Quando l'app parte, controlla se il DB esiste e applica le migrazioni mancanti
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApprovvigionamentiDbContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Errore durante la migrazione del database: {ex.Message}");
    }
}
// ----------------------

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();