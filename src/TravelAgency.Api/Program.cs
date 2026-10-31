using System.Text;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using TravelAgency.Api.Auth;
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
builder.Services.AddScoped<IAuthService, AuthService>();

// JWT-автентифікація. Ключ для розробки — в appsettings.Development.json, у робочому середовищі — змінна Jwt__Key.
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
builder.Services.AddSingleton<ITokenService, JwtTokenService>();

var jwt = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
if (string.IsNullOrWhiteSpace(jwt.Key) || jwt.Key.Length < 32)
    throw new InvalidOperationException("Configuration value 'Jwt:Key' must be at least 32 characters long.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Claim-и в токені мають короткі імена (sub, name, role), тому автоматичне перейменування вимкнено
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,
            ValidateAudience = true,
            ValidAudience = jwt.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1),
            NameClaimType = "name",
            RoleClaimType = "role"
        };
    });
builder.Services.AddAuthorization();

// CORS для веб-клієнта (адреси задаються в Cors:AllowedOrigins)
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
    options.AddPolicy("Web", policy =>
        policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod()));

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
else
{
    // У розробці веб-клієнт звертається до API по http, тому переадресацію на https вмикаємо лише в робочому середовищі
    app.UseHttpsRedirection();
}

app.UseCors("Web");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
