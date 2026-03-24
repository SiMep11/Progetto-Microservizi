using Microsoft.EntityFrameworkCore;
using Ordini.Repository;
using Ordini.Repository.Abstractions;
using Ordini.Business;
using Ordini.Business.Abstractions;
using Ordini.ClientHttp.Abstractions;
using Ordini.ClientHttp;
using Ordini.Business.Kafka;
using Ordini.ClientHttp.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// 1. Configura il DbContext per collegarsi a SQL Server
builder.Services.AddDbContext<OrdiniDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("Ordini.WebApi") 
    ));

// 2. Registra il Repository (Dependency Injection)
builder.Services.AddScoped<IOrdiniRepository, OrdiniRepository>();
// Registra il livello Business
builder.Services.AddScoped<IOrdiniService, OrdiniService>();
builder.Services.AddScoped<IEventProducer, KafkaEventProducer>();
builder.Services.AddMagazzinoClient(options =>
{
    options.BaseUrl = "http://localhost:5024"; // Metti qui l'URL locale del tuo Magazzino
});
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // <-- Questo genera il file JSON di Swagger
builder.Services.AddOpenApi();

var app = builder.Build();

// --- AUTO-MIGRATION ---
// Quando l'app parte, controlla se il DB esiste e applica le migrazioni mancanti
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<OrdiniDbContext>();
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
