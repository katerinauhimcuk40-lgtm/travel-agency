using Microsoft.EntityFrameworkCore;

using TravelAgency.Core.Interfaces;
using TravelAgency.Infrastructure.Data;
using TravelAgency.Infrastructure.Repositories;
using TravelAgency.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Підключення DbContext
builder.Services.AddDbContext<TravelAgencyDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("TravelAgency.Infrastructure")
    ));

// Реєстрація репозиторіїв та бізнес-сервісів
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/core/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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