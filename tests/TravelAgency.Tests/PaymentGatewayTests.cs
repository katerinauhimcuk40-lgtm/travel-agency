using Moq;

using TravelAgency.Core.Entities;
using TravelAgency.Core.Enums;
using TravelAgency.Core.Interfaces;
using TravelAgency.Infrastructure.Data;
using TravelAgency.Infrastructure.Payments;
using TravelAgency.Infrastructure.Repositories;
using TravelAgency.Infrastructure.Services;

namespace TravelAgency.Tests;

public class PaymentGatewayTests
{
    private static async Task<(Booking booking, BookingService bookings, UnitOfWork unitOfWork)> ArrangeAsync(
        TravelAgencyDbContext context)
    {
        var group = await TestDb.SeedGroupAsync(context);
        var unitOfWork = new UnitOfWork(context);
        var bookings = new BookingService(unitOfWork, TestDb.Encryption());
        var booking = await bookings.CreateBookingAsync(1, group.Id, TestDb.Tourists(2));

        return (booking, bookings, unitOfWork);
    }

    private static Mock<IPaymentGatewayFactory> FactoryReturning(Mock<IPaymentGateway> gateway)
    {
        var factory = new Mock<IPaymentGatewayFactory>();
        factory.Setup(f => f.Create(It.IsAny<string>())).Returns(gateway.Object);
        return factory;
    }

    // ---------- Фабрика ----------

    [Theory]
    [InlineData("Fake", typeof(FakePaymentGateway))]
    [InlineData("Card", typeof(CardPaymentGateway))]
    [InlineData("card", typeof(CardPaymentGateway))]
    public void Factory_ReturnsGatewayForProvider(string provider, Type expected)
    {
        var gateway = new PaymentGatewayFactory().Create(provider);

        Assert.IsType(expected, gateway);
    }

    [Fact]
    public void Factory_UnknownProvider_Throws()
    {
        Assert.Throws<ArgumentException>(() => new PaymentGatewayFactory().Create("Bitcoin"));
    }

    // ---------- Шлюзи ----------

    [Fact]
    public async Task FakeGateway_Charge_Succeeds()
    {
        var result = await new FakePaymentGateway().ChargeAsync(100, "UAH");

        Assert.True(result.Success);
        Assert.False(string.IsNullOrEmpty(result.TransactionId));
    }

    [Fact]
    public async Task CardGateway_ZeroAmount_Fails()
    {
        var result = await new CardPaymentGateway().ChargeAsync(0, "UAH");

        Assert.False(result.Success);
        Assert.Null(result.TransactionId);
    }

    // ---------- PaymentService + фабрика ----------

    [Fact]
    public async Task ProcessPayment_CardProvider_SavesProviderName()
    {
        using var context = TestDb.CreateContext();
        var (booking, bookings, unitOfWork) = await ArrangeAsync(context);
        await bookings.ConfirmBookingAsync(booking.Id);
        var payments = new PaymentService(unitOfWork, new PaymentGatewayFactory());

        await payments.ProcessPaymentAsync(booking.Id, booking.TotalPrice, "Card");

        var payment = Assert.Single(context.Payments);
        Assert.Equal("Card", payment.Provider);
        Assert.Equal(PaymentStatus.Succeeded, payment.Status);
        Assert.StartsWith("CARD-", payment.ExternalTransactionId);
        Assert.Equal(BookingStatus.Paid, booking.Status);
    }

    [Fact]
    public async Task ProcessPayment_UnknownProvider_ThrowsAndSavesNothing()
    {
        using var context = TestDb.CreateContext();
        var (booking, bookings, unitOfWork) = await ArrangeAsync(context);
        await bookings.ConfirmBookingAsync(booking.Id);
        var payments = new PaymentService(unitOfWork, new PaymentGatewayFactory());

        await Assert.ThrowsAsync<ArgumentException>(
            () => payments.ProcessPaymentAsync(booking.Id, booking.TotalPrice, "Bitcoin"));

        Assert.Empty(context.Payments);
        Assert.Equal(BookingStatus.Confirmed, booking.Status);
    }

    [Fact]
    public async Task ProcessPayment_GatewayDeclines_SavesFailedPaymentAndKeepsConfirmed()
    {
        using var context = TestDb.CreateContext();
        var (booking, bookings, unitOfWork) = await ArrangeAsync(context);
        await bookings.ConfirmBookingAsync(booking.Id);

        var gateway = new Mock<IPaymentGateway>();
        gateway.SetupGet(g => g.Name).Returns("Mock");
        gateway.Setup(g => g.ChargeAsync(It.IsAny<decimal>(), It.IsAny<string>()))
               .ReturnsAsync(new PaymentGatewayResult(false, null));
        var payments = new PaymentService(unitOfWork, FactoryReturning(gateway).Object);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => payments.ProcessPaymentAsync(booking.Id, booking.TotalPrice));

        var payment = Assert.Single(context.Payments);
        Assert.Equal(PaymentStatus.Failed, payment.Status);
        Assert.Equal("Mock", payment.Provider);
        Assert.Null(payment.PaidAt);
        Assert.Equal(BookingStatus.Confirmed, booking.Status);
    }

    [Fact]
    public async Task ProcessPayment_NotConfirmedBooking_DoesNotCallGateway()
    {
        using var context = TestDb.CreateContext();
        var (booking, _, unitOfWork) = await ArrangeAsync(context);   // бронювання лишається New

        var gateway = new Mock<IPaymentGateway>();
        var payments = new PaymentService(unitOfWork, FactoryReturning(gateway).Object);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => payments.ProcessPaymentAsync(booking.Id, booking.TotalPrice));

        gateway.Verify(g => g.ChargeAsync(It.IsAny<decimal>(), It.IsAny<string>()), Times.Never);
        Assert.Empty(context.Payments);
    }
}