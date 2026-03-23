using Microsoft.EntityFrameworkCore;
using Ordini.Repository;
using Ordini.Repository.Abstractions;
using Ordini.Business;
using Ordini.Business.Abstractions;
using Ordini.ClientHttp.Abstractions;
using Ordini.ClientHttp;


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
builder.Services.AddHttpClient<IMagazzinoClient, MagazzinoClient>(client =>
{
    // ATTENZIONE: Metti qui l'URL su cui gira il tuo Magazzino in locale! 
    // Di solito è qualcosa tipo https://localhost:5001 o http://localhost:5000
    // (Puoi controllarlo nel file launchSettings.json del Magazzino)
    client.BaseAddress = new Uri("https://localhost:5024");
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
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
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
