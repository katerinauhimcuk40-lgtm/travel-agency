using TravelAgency.Core.Entities;
using TravelAgency.Core.Models;
using TravelAgency.Infrastructure.Repositories;
using TravelAgency.Infrastructure.Services;

namespace TravelAgency.Tests;

public class CrmServiceTests
{
    
    [Fact]
    public async Task GetBookingAsync_WithValidId_ShouldReturnBooking()
    {
        // Arrange
        using var context = TestDb.CreateContext();
        var unitOfWork = new UnitOfWork(context);
        var group = await TestDb.SeedGroupAsync(context);
        var bookingService = new BookingService(unitOfWork, TestDb.Encryption());

        var booking = await bookingService.CreateBookingAsync(1, group.Id, TestDb.Tourists(1));

        var crmService = new CrmService(unitOfWork);

        // Act
        var result = await crmService.GetBookingAsync(booking.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(booking.Id, result.Id);
    }

    [Fact]
    public async Task GetBookingAsync_WithNonExistentId_ShouldThrowException()
    {
        // Arrange
        using var context = TestDb.CreateContext();
        var unitOfWork = new UnitOfWork(context);
        var crmService = new CrmService(unitOfWork);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => crmService.GetBookingAsync(9999));
    }
    [Fact]
    public async Task SearchBookingsAsync_ShouldReturnPagedResult()
    {
        // Arrange
        using var context = TestDb.CreateContext();
        var unitOfWork = new UnitOfWork(context);
        var group = await TestDb.SeedGroupAsync(context);
        var bookingService = new BookingService(unitOfWork, TestDb.Encryption());

        await bookingService.CreateBookingAsync(1, group.Id, TestDb.Tourists(1));

        var crmService = new CrmService(unitOfWork);
        var criteria = new BookingSearchCriteria
        {
            Page = 1,
            PageSize = 10
        };

        // Act
        var result = await crmService.SearchBookingsAsync(criteria);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Items);
    }
}