using TravelAgency.Core.Entities;
using TravelAgency.Core.Enums;

using Xunit;

namespace TravelAgency.Tests;

public class PaymentTests
{
    [Fact]
    public void NewPayment_HasDefaultValues()
    {
        var before = DateTime.UtcNow;

        var payment = new Payment();

        Assert.Equal(PaymentStatus.Pending, payment.Status);
        Assert.Equal("UAH", payment.Currency);
        Assert.Equal(string.Empty, payment.Provider);
        Assert.Null(payment.PaidAt);
        Assert.Null(payment.ExternalTransactionId);
        Assert.Null(payment.Booking);
        Assert.InRange(payment.CreatedAt, before, DateTime.UtcNow);
    }

    [Fact]
    public void MarkSucceeded_SetsStatusAndPaidAt()
    {
        var payment = new Payment();
        var before = DateTime.UtcNow;

        payment.MarkSucceeded();

        Assert.Equal(PaymentStatus.Succeeded, payment.Status);
        Assert.NotNull(payment.PaidAt);
        Assert.InRange(payment.PaidAt!.Value, before, DateTime.UtcNow);
    }

    [Fact]
    public void MarkFailed_SetsStatusFailed_AndLeavesPaidAtEmpty()
    {
        var payment = new Payment();

        payment.MarkFailed();

        Assert.Equal(PaymentStatus.Failed, payment.Status);
        Assert.Null(payment.PaidAt);
    }

    [Fact]
    public void Properties_CanBeAssigned()
    {
        var booking = new Booking();

        var payment = new Payment
        {
            Id = 5,
            BookingId = 10,
            Booking = booking,
            Amount = 1500.50m,
            Currency = "USD",
            Provider = "TestProvider",
            ExternalTransactionId = "tx-123"
        };

        Assert.Equal(5, payment.Id);
        Assert.Equal(10, payment.BookingId);
        Assert.Same(booking, payment.Booking);
        Assert.Equal(1500.50m, payment.Amount);
        Assert.Equal("USD", payment.Currency);
        Assert.Equal("TestProvider", payment.Provider);
        Assert.Equal("tx-123", payment.ExternalTransactionId);
    }
}