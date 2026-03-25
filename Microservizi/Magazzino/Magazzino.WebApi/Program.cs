using Magazzino.Business;
using Magazzino.Business.Abstraction;
using Magazzino.Business.BackgroundServices;
using Magazzino.Repository;
using Magazzino.Repository.Abstraction;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddDbContext<MagazzinoDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("MagazzinoDb"),
        b => b.MigrationsAssembly("Magazzino.WebApi") 
    ));
builder.Services.AddScoped<ProdottiManager>();
// Registriamo il nuovo Repository
builder.Services.AddScoped<IMagazzinoRepository, MagazzinoRepository>();

// Registriamo il Manager aggiornato
builder.Services.AddScoped<IProdottiManager, ProdottiManager>();

builder.Services.AddHostedService<ConsumerOrdineCreato>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Chiediamo a .NET di darci un'istanza del nostro DbContext
        var context = services.GetRequiredService<MagazzinoDbContext>();

        // Questo comando magico crea il DB e le tabelle se non esistono!
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Errore durante la creazione del database.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
