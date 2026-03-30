using Microsoft.EntityFrameworkCore;
using Pagamenti.Repository;
using Pagamenti.Business.Profiles;

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


