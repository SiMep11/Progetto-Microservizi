using Magazzino.Business;
using Magazzino.Business.Abstraction;
using Magazzino.Business.Kafka;
using Magazzino.Business.Kafka.MessageHandlers;
using Magazzino.Repository;
using Magazzino.Repository.Abstraction;
using Microsoft.EntityFrameworkCore;
using Utility.Kafka.Abstractions.Clients;
using Utility.Kafka.Clients;

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

// Kafka: produciamo "scorta-sotto-soglia", consumiamo "ordine-confermato" e "merce-ricevuta"
builder.Services.Configure<KafkaProducerClientOptions>(builder.Configuration.GetSection("Kafka"));
builder.Services.AddSingleton<IProducerClient<string, string>, ProducerClient>();
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
