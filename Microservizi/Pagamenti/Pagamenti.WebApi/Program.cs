using Microsoft.EntityFrameworkCore;
using Pagamenti.Business;
using Pagamenti.Business.Abstraction;
using Pagamenti.Business.Profiles;
using Pagamenti.Repository;
using Pagamenti.Repository.Abstraction;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<PagamentiDbContext>(options =>
{
    // Usa la stringa di connessione dall'appsettings.json
    options.UseSqlServer(builder.Configuration.GetConnectionString("PagamentiDb"), b => b.MigrationsAssembly("Pagamenti.WebApi"));
});
builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(AssemblyMarker).Assembly));

// Registrazione Repository e Logica di Business
builder.Services.AddScoped<IRepository, Repository>(); // Assicurati di avere l'using per il tuo Repository
builder.Services.AddScoped<IBusiness, Business>();

// Registrazione del sistema di Eventi (Observer Pattern per Kafka)
builder.Services.AddSingleton(p => ActivatorUtilities.CreateInstance<Subject>(p));
builder.Services.AddSingleton<IPagamentiObservable>(p => p.GetRequiredService<Subject>());
builder.Services.AddSingleton<IPagamentiObserver>(p => p.GetRequiredService<Subject>());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddAuthorization();

var app = builder.Build();

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