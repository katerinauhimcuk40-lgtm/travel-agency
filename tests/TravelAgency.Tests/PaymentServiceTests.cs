using TravelAgency.Core.Entities;
using TravelAgency.Core.Enums;
using TravelAgency.Infrastructure.Data;
using TravelAgency.Infrastructure.Repositories;
using TravelAgency.Infrastructure.Services;

namespace TravelAgency.Tests;

public class PaymentServiceTests
{
    private static async Task<(Booking booking, PaymentService payments, BookingService bookings)> ArrangeAsync(TravelAgencyDbContext context)
    {
        var group = await TestDb.SeedGroupAsync(context);
        var unitOfWork = new UnitOfWork(context);
        var bookings = new BookingService(unitOfWork);
        var payments = new PaymentService(unitOfWork);
        var booking = await bookings.CreateBookingAsync(1, group.Id, TestDb.Tourists(2));

        return (booking, payments, bookings);
    }

    [Fact]
    public async Task ProcessPayment_ConfirmedBooking_MarksPaidAndSavesPayment()
    {
        using var context = TestDb.CreateContext();
        var (booking, payments, bookings) = await ArrangeAsync(context);
        await bookings.ConfirmBookingAsync(booking.Id);

        await payments.ProcessPaymentAsync(booking.Id, booking.TotalPrice);

        Assert.Equal(BookingStatus.Paid, booking.Status);
        var payment = Assert.Single(context.Payments);
        Assert.Equal(PaymentStatus.Succeeded, payment.Status);
        Assert.Equal(booking.TotalPrice, payment.Amount);
        Assert.NotNull(payment.PaidAt);
    }

    [Fact]
    public async Task ProcessPayment_WrongAmount_Throws()
    {
        using var context = TestDb.CreateContext();
        var (booking, payments, bookings) = await ArrangeAsync(context);
        await bookings.ConfirmBookingAsync(booking.Id);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => payments.ProcessPaymentAsync(booking.Id, 1));

        Assert.Empty(context.Payments);
        Assert.Equal(BookingStatus.Confirmed, booking.Status);
    }

    [Fact]
    public async Task ProcessPayment_NotConfirmedBooking_Throws()
    {
        using var context = TestDb.CreateContext();
        var (booking, payments, _) = await ArrangeAsync(context);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => payments.ProcessPaymentAsync(booking.Id, booking.TotalPrice));

        Assert.Empty(context.Payments);
    }

    [Fact]
    public async Task ProcessPayment_PaidTwice_Throws()
    {
        using var context = TestDb.CreateContext();
        var (booking, payments, bookings) = await ArrangeAsync(context);
        await bookings.ConfirmBookingAsync(booking.Id);
        await payments.ProcessPaymentAsync(booking.Id, booking.TotalPrice);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => payments.ProcessPaymentAsync(booking.Id, booking.TotalPrice));

        Assert.Single(context.Payments);
    }

    [Fact]
    public async Task ProcessPayment_BookingNotFound_Throws()
    {
        using var context = TestDb.CreateContext();
        var (_, payments, _) = await ArrangeAsync(context);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => payments.ProcessPaymentAsync(999, 100));
    }
}
