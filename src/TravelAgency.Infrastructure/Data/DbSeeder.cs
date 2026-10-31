using Microsoft.EntityFrameworkCore;

using TravelAgency.Core.Entities;
using TravelAgency.Core.Enums;

namespace TravelAgency.Infrastructure.Data;

/// <summary>Початкові дані для розробки. Повторний виклик нічого не дублює.</summary>
public static class DbSeeder
{
    public static async Task SeedAsync(TravelAgencyDbContext context)
    {
        await SeedUsersAsync(context);
        await SeedCatalogAsync(context);
    }

    private static async Task SeedUsersAsync(TravelAgencyDbContext context)
    {
        if (await context.Users.AnyAsync())
        {
            await UpgradePlaceholderPasswordsAsync(context);
            return;
        }

        // Демо-акаунти для розробки (пароль див. DemoPasswords)
        context.Users.AddRange(
            new User { Email = "client@travel.local", FullName = "Тестовий Клієнт", Role = Role.Client, PasswordHash = HashPassword(DemoPasswords["client@travel.local"]) },
            new User { Email = "manager@travel.local", FullName = "Тестовий Менеджер", Role = Role.Manager, PasswordHash = HashPassword(DemoPasswords["manager@travel.local"]) },
            new User { Email = "admin@travel.local", FullName = "Тестовий Адміністратор", Role = Role.Admin, PasswordHash = HashPassword(DemoPasswords["admin@travel.local"]) });

        await context.SaveChangesAsync();
    }

    /// <summary>Демо-паролі лише для середовища розробки.</summary>
    private static readonly Dictionary<string, string> DemoPasswords = new()
    {
        ["client@travel.local"] = "Client123!",
        ["manager@travel.local"] = "Manager123!",
        ["admin@travel.local"] = "Admin123!"
    };

    /// <summary>У раніше заповненій базі демо-користувачі мають заглушку "!" замість хешу — замінюємо її.</summary>
    private static async Task UpgradePlaceholderPasswordsAsync(TravelAgencyDbContext context)
    {
        var placeholders = await context.Users.Where(u => u.PasswordHash == "!").ToListAsync();

        foreach (var user in placeholders)
        {
            if (DemoPasswords.TryGetValue(user.Email, out var password))
                user.PasswordHash = HashPassword(password);
        }

        if (placeholders.Count > 0)
            await context.SaveChangesAsync();
    }

    private static string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);

    private static async Task SeedCatalogAsync(TravelAgencyDbContext context)
    {
        if (await context.Tours.AnyAsync() || await context.Hotels.AnyAsync())
            return;

        var hotels = new[]
        {
            new Hotel { Name = "Hotel Lumière", Country = "France", City = "Paris", Address = "12 Rue de Rivoli", Capacity = 120, HotelType = "4*" },
            new Hotel { Name = "Sunrise Resort", Country = "Turkey", City = "Antalya", Address = "Lara Beach 5", Capacity = 400, HotelType = "5*" },
            new Hotel { Name = "Kemer Beach", Country = "Turkey", City = "Kemer", Address = "Sahil Cd. 21", Capacity = 250, HotelType = "5*" },
            new Hotel { Name = "Карпатська оселя", Country = "Ukraine", City = "Яремче", Address = "вул. Свободи, 8", Capacity = 60, HotelType = "3*" },
            new Hotel { Name = "Roma Centrale", Country = "Italy", City = "Rome", Address = "Via Nazionale 40", Capacity = 150, HotelType = "4*" }
        };
        context.Hotels.AddRange(hotels);

        var tours = new[]
        {
            new Tour { Title = "Романтичний Париж", Description = "Екскурсії Парижем і Версалем, вільний час для прогулянок.", Country = "France", Cities = "Париж, Версаль", TransportType = TransportType.Plane, MealPlan = MealPlan.BB, AccommodationType = "Готель 4*", BasePrice = 850 },
            new Tour { Title = "Відпочинок в Анталії", Description = "Пляжний відпочинок за системою «все включено».", Country = "Turkey", Cities = "Анталія, Кемер", TransportType = TransportType.Plane, MealPlan = MealPlan.AI, AccommodationType = "Готель 5*", BasePrice = 620 },
            new Tour { Title = "Карпатські канікули", Description = "Гірські маршрути, термальні басейни, місцева кухня.", Country = "Ukraine", Cities = "Буковель, Яремче", TransportType = TransportType.Bus, MealPlan = MealPlan.HB, AccommodationType = "Гірський готель", BasePrice = 210 },
            new Tour { Title = "Італійська мозаїка", Description = "Рим, Флоренція та Венеція за один тур.", Country = "Italy", Cities = "Рим, Флоренція, Венеція", TransportType = TransportType.Bus, MealPlan = MealPlan.BB, AccommodationType = "Готель 3-4*", BasePrice = 740 }
        };
        context.Tours.AddRange(tours);

        await context.SaveChangesAsync();

        var today = DateTime.UtcNow.Date;

        context.TourGroups.AddRange(
            Group("PAR-01", tours[0], hotels[0], today.AddDays(30), 7, 20),
            Group("PAR-02", tours[0], hotels[0], today.AddDays(75), 7, 20),
            Group("ANT-01", tours[1], hotels[1], today.AddDays(20), 10, 40),
            Group("ANT-02", tours[1], hotels[2], today.AddDays(50), 10, 30),
            Group("KAR-01", tours[2], hotels[3], today.AddDays(15), 5, 25),
            Group("ITA-01", tours[3], hotels[4], today.AddDays(45), 9, 35),
            Group("ITA-02", tours[3], hotels[4], today.AddDays(90), 9, 35));

        await context.SaveChangesAsync();
    }

    private static TourGroup Group(string name, Tour tour, Hotel hotel, DateTime departure, int days, int seats) => new()
    {
        Name = name,
        TourId = tour.Id,
        HotelId = hotel.Id,
        DepartureDate = departure,
        ArrivalDate = departure.AddDays(days),
        SeatsTotal = seats,
        SeatsAvailable = seats
    };
}
