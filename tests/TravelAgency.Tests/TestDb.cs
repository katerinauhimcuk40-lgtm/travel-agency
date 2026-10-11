using Microsoft.EntityFrameworkCore;

using Moq;

using TravelAgency.Core.Entities;
using TravelAgency.Core.Enums;
using TravelAgency.Core.Interfaces;
using TravelAgency.Core.Models;
using TravelAgency.Infrastructure.Data;

namespace TravelAgency.Tests;

/// <summary>Допоміжний клас: окрема in-memory база для кожного тесту.</summary>
internal static class TestDb
{
    public static TravelAgencyDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<TravelAgencyDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TravelAgencyDbContext(options);
    }

    /// <summary>Імітація сервісу шифрування (Moq): додає до тексту префікс "enc:".</summary>
    public static IEncryptionService Encryption()
    {
        var mock = new Mock<IEncryptionService>();
        mock.Setup(e => e.Encrypt(It.IsAny<string>())).Returns<string>(s => "enc:" + s);
        mock.Setup(e => e.Decrypt(It.IsAny<string>())).Returns<string>(s => s.Replace("enc:", string.Empty));
        return mock.Object;
    }

    /// <summary>Додає користувача, тур, готель і групу (виліт через 30 днів) та повертає групу.</summary>
    public static async Task<TourGroup> SeedGroupAsync(TravelAgencyDbContext context, int seats = 10, int daysToDeparture = 30)
    {
        var user = new User { Id = 1, Email = "client@test.com", FullName = "Test Client" };
        var tour = new Tour { Id = 1, Title = "Paris Tour", Country = "France", Cities = "Paris", BasePrice = 500 };
        var hotel = new Hotel { Id = 1, Name = "Hotel Paris", Country = "France", City = "Paris", Address = "Rue 1", Capacity = 100, HotelType = "4*" };
        var group = new TourGroup
        {
            Id = 1,
            Name = "PAR-01",
            TourId = tour.Id,
            HotelId = hotel.Id,
            DepartureDate = DateTime.UtcNow.AddDays(daysToDeparture),
            ArrivalDate = DateTime.UtcNow.AddDays(daysToDeparture + 7),
            SeatsTotal = seats,
            SeatsAvailable = seats
        };

        context.Users.Add(user);
        context.Tours.Add(tour);
        context.Hotels.Add(hotel);
        context.TourGroups.Add(group);
        await context.SaveChangesAsync();

        return group;
    }

    public static List<TouristInfo> Tourists(int count) =>
        Enumerable.Range(1, count)
            .Select(i => new TouristInfo($"Tourist {i}", $"AB{i:D6}", Gender.Male, 30, null))
            .ToList();
}
