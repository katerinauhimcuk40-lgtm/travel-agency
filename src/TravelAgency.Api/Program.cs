using System.Text.Json.Serialization;

using Microsoft.EntityFrameworkCore;

using TravelAgency.Api.Middleware;
using TravelAgency.Core.Interfaces;
using TravelAgency.Infrastructure.Data;
using TravelAgency.Infrastructure.Repositories;
using TravelAgency.Infrastructure.Security;
using TravelAgency.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Підключення DbContext
builder.Services.AddDbContext<TravelAgencyDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("TravelAgency.Infrastructure")
    ));

// Ключ шифрування паспортних даних (AES-256, Base64). Для розробки задається в appsettings.Development.json,
// у робочому середовищі — змінною середовища Encryption__Key.
var encryptionKey = builder.Configuration["Encryption:Key"]
    ?? throw new InvalidOperationException("Configuration value 'Encryption:Key' is missing.");

// Реєстрація репозиторіїв та бізнес-сервісів
builder.Services.AddSingleton<IEncryptionService>(_ => new AesEncryptionService(encryptionKey));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<ITourCatalogService, TourCatalogService>();

builder.Services.AddHealthChecks();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/core/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Глобальна обробка помилок має бути першою в конвеєрі
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // У розробці: застосувати міграції та заповнити БД початковими даними
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<TravelAgencyDbContext>();
    await db.Database.MigrateAsync();
    await DbSeeder.SeedAsync(db);
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
