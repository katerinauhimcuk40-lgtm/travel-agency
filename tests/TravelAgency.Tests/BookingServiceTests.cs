using TravelAgency.Core.Enums;
using TravelAgency.Infrastructure.Repositories;
using TravelAgency.Infrastructure.Services;

namespace TravelAgency.Tests;

public class BookingServiceTests
{
    [Fact]
    public async Task CreateBooking_ReservesSeatsAndCalculatesTotal()
    {
        using var context = TestDb.CreateContext();
        var group = await TestDb.SeedGroupAsync(context, seats: 10);
        var service = new BookingService(new UnitOfWork(context), TestDb.Encryption());

        var booking = await service.CreateBookingAsync(1, group.Id, TestDb.Tourists(2));

        Assert.Equal(BookingStatus.New, booking.Status);
        Assert.Equal(2, booking.PersonsCount);
        Assert.Equal(1000, booking.TotalPrice);
        Assert.All(booking.Tourists, t => Assert.Equal(500, t.TicketPrice));
        Assert.Equal(8, group.SeatsAvailable);
    }

    [Fact]
    public async Task CreateBooking_EncryptsPassportData()
    {
        using var context = TestDb.CreateContext();
        var group = await TestDb.SeedGroupAsync(context);
        var service = new BookingService(new UnitOfWork(context), TestDb.Encryption());

        var booking = await service.CreateBookingAsync(1, group.Id, TestDb.Tourists(2));

        Assert.All(booking.Tourists, t =>
        {
            Assert.StartsWith("enc:", t.PassportEncrypted);
            Assert.DoesNotMatch("^AB", t.PassportEncrypted);
        });
        Assert.Equal(2, context.Tourists.Count());
    }

    [Fact]
    public async Task CreateBooking_NotEnoughSeats_Throws()
    {
        using var context = TestDb.CreateContext();
        var group = await TestDb.SeedGroupAsync(context, seats: 1);
        var service = new BookingService(new UnitOfWork(context), TestDb.Encryption());

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CreateBookingAsync(1, group.Id, TestDb.Tourists(2)));

        Assert.Empty(context.Bookings);
        Assert.Equal(1, group.SeatsAvailable);
    }

    [Fact]
    public async Task CreateBooking_GroupNotFound_Throws()
    {
        using var context = TestDb.CreateContext();
        await TestDb.SeedGroupAsync(context);
        var service = new BookingService(new UnitOfWork(context), TestDb.Encryption());

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => service.CreateBookingAsync(1, 999, TestDb.Tourists(1)));
    }

    [Fact]
    public async Task CreateBooking_UserNotFound_Throws()
    {
        using var context = TestDb.CreateContext();
        var group = await TestDb.SeedGroupAsync(context);
        var service = new BookingService(new UnitOfWork(context), TestDb.Encryption());

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => service.CreateBookingAsync(999, group.Id, TestDb.Tourists(1)));
    }

    [Fact]
    public async Task CreateBooking_NoTourists_Throws()
    {
        using var context = TestDb.CreateContext();
        var group = await TestDb.SeedGroupAsync(context);
        var service = new BookingService(new UnitOfWork(context), TestDb.Encryption());

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateBookingAsync(1, group.Id, []));
    }

    [Fact]
    public async Task CreateBooking_GroupAlreadyDeparted_Throws()
    {
        using var context = TestDb.CreateContext();
        var group = await TestDb.SeedGroupAsync(context, daysToDeparture: -1);
        var service = new BookingService(new UnitOfWork(context), TestDb.Encryption());

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CreateBookingAsync(1, group.Id, TestDb.Tourists(1)));
    }

    [Fact]
    public async Task ConfirmBooking_SetsConfirmed()
    {
        using var context = TestDb.CreateContext();
        var group = await TestDb.SeedGroupAsync(context);
        var service = new BookingService(new UnitOfWork(context), TestDb.Encryption());
        var booking = await service.CreateBookingAsync(1, group.Id, TestDb.Tourists(1));

        await service.ConfirmBookingAsync(booking.Id);

        Assert.Equal(BookingStatus.Confirmed, booking.Status);
    }

    [Fact]
    public async Task CancelBooking_ReleasesSeats()
    {
        using var context = TestDb.CreateContext();
        var group = await TestDb.SeedGroupAsync(context, seats: 10);
        var service = new BookingService(new UnitOfWork(context), TestDb.Encryption());
        var booking = await service.CreateBookingAsync(1, group.Id, TestDb.Tourists(3));

        await service.CancelBookingAsync(booking.Id);

        Assert.Equal(BookingStatus.Cancelled, booking.Status);
        Assert.Equal(10, group.SeatsAvailable);
    }

    [Fact]
    public async Task CancelBooking_AlreadyCancelled_Throws()
    {
        using var context = TestDb.CreateContext();
        var group = await TestDb.SeedGroupAsync(context);
        var service = new BookingService(new UnitOfWork(context), TestDb.Encryption());
        var booking = await service.CreateBookingAsync(1, group.Id, TestDb.Tourists(1));
        await service.CancelBookingAsync(booking.Id);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CancelBookingAsync(booking.Id));

        Assert.Equal(10, group.SeatsAvailable);
    }

    [Fact]
    public async Task CancelBooking_NotFound_Throws()
    {
        using var context = TestDb.CreateContext();
        await TestDb.SeedGroupAsync(context);
        var service = new BookingService(new UnitOfWork(context), TestDb.Encryption());

        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.CancelBookingAsync(999));
    }

    [Fact]
    public async Task CreateBooking_TourOrGroupDoesNotExist_ThrowsNotFound()
    {
        using var context = TestDb.CreateContext();
        var service = new BookingService(new UnitOfWork(context), TestDb.Encryption());

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => service.CreateBookingAsync(1, 999, TestDb.Tourists(1)));
    }

    [Fact]
    public async Task CancelBooking_AlreadyPaidOrCompleted_ThrowsInvalidOperation()
    {
        using var context = TestDb.CreateContext();
        var group = await TestDb.SeedGroupAsync(context);
        var service = new BookingService(new UnitOfWork(context), TestDb.Encryption());
        var booking = await service.CreateBookingAsync(1, group.Id, TestDb.Tourists(1));

        await service.CancelBookingAsync(booking.Id);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CancelBookingAsync(booking.Id));
    }
}