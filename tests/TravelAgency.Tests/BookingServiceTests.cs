using Microsoft.EntityFrameworkCore;

using TravelAgency.Core.Entities;
using TravelAgency.Core.Enums;
using TravelAgency.Infrastructure.Data;
using TravelAgency.Infrastructure.Repositories;
using TravelAgency.Infrastructure.Services;

using Xunit;

namespace TravelAgency.Tests;

public class BookingServiceTests
{
    [Fact]
    public async Task CreateBooking_ShouldCreateAndReserveBooking()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TravelAgencyDbContext>()
            .UseInMemoryDatabase(databaseName: "TestTravelDb")
            .Options;

        using var context = new TravelAgencyDbContext(options);

        var tour = new Tour { Id = 1, Title = "Paris Tour", Price = 500 };
        var tourGroup = new TourGroup { Id = 1, TourId = 1, Tour = tour, MaxCapacity = 10 };

        context.Tours.Add(tour);
        context.TourGroups.Add(tourGroup);
        await context.SaveChangesAsync();

        var unitOfWork = new UnitOfWork(context);
        var bookingService = new BookingService(unitOfWork);

        var tourists = new List<Tourist>
        {
            new Tourist { FirstName = "John", LastName = "Doe", PassportNumber = "AB12345" }
        };

        // Act
        var booking = await bookingService.CreateBookingAsync(1, 1, tourists);

        // Assert
        Assert.NotNull(booking);
        Assert.Equal(BookingStatus.Reserved, booking.Status);
        Assert.Equal(500, booking.TotalAmount);
    }
}