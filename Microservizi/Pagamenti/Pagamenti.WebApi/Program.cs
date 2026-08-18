using Microsoft.EntityFrameworkCore;
using Pagamenti.Business;
using Pagamenti.Business.Abstraction;
using Pagamenti.Business.Profiles;
using Pagamenti.Repository;
using Pagamenti.Repository.Abstraction;
using Pagamenti.Business.Kafka;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

builder.Services.AddDbContext<PagamentiDbContext>(options =>
{
    // Usa la stringa di connessione dall'appsettings.json
    options.UseSqlServer(builder.Configuration.GetConnectionString("PagamentiDb"), b => b.MigrationsAssembly("Pagamenti.WebApi"));
});
builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(AssemblyMarker).Assembly));

// Registrazione Repository e Logica di Business
builder.Services.AddScoped<IRepository, Repository>(); 
builder.Services.AddScoped<IBusiness, Business>();

// Registrazione del sistema di Eventi (Observer Pattern per Kafka)
builder.Services.AddSingleton(p => ActivatorUtilities.CreateInstance<Subject>(p));
builder.Services.AddSingleton<IPagamentiObservable>(p => p.GetRequiredService<Subject>());
builder.Services.AddSingleton<IPagamentiObserver>(p => p.GetRequiredService<Subject>());
builder.Services.AddKafkaProducerServiceWithSubscription<KafkaTopicsOutput, ProducerServiceWithSubscription>(builder.Configuration);

// Comunicazione sincrona verso Ordini: serve a validare l'ordine prima di incassare
builder.Services.AddOrdiniClient(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<PagamentiDbContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Errore durante la migrazione del database: {ex.Message}");
    }
}

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