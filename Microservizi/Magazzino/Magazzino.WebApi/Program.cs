using Magazzino.Business;
using Magazzino.Business.Abstraction;
using Magazzino.Business.Kafka;
using Magazzino.Business.Kafka.MessageHandlers;
using Magazzino.Repository;
using Magazzino.Repository.Abstraction;
using Microsoft.EntityFrameworkCore;
using Utility.Kafka.Abstractions.Clients;
using Utility.Kafka.Clients;
using Magazzino.Business.Abstraction;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MagazzinoDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("MagazzinoDb"),
        b => {
            b.MigrationsAssembly("Magazzino.WebApi");
            b.EnableRetryOnFailure();
        }
    ));

// Registro il Repository
builder.Services.AddScoped<IMagazzinoRepository, MagazzinoRepository>();

// Registro il Manager
builder.Services.AddScoped<IProdottiManager, ProdottiManager>();

// Observer pattern per svegliare il producer dell'outbox
builder.Services.AddSingleton(p => ActivatorUtilities.CreateInstance<Subject>(p));
builder.Services.AddSingleton<IMagazzinoObservable>(p => p.GetRequiredService<Subject>());
builder.Services.AddSingleton<IMagazzinoObserver>(p => p.GetRequiredService<Subject>());

builder.Services.AddKafkaProducerServiceWithSubscription<KafkaTopicsOutput, ProducerServiceWithSubscription>(builder.Configuration);
builder.Services.AddKafkaConsumerService<KafkaTopicsInput, MagazzinoMessageHandlerFactory>(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<MagazzinoDbContext>();
    context.Database.Migrate();
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
