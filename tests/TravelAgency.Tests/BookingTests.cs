using TravelAgency.Core.Entities;
using TravelAgency.Core.Enums;

namespace TravelAgency.Tests;

public class BookingTests
{
    [Fact]
    public void NewBooking_HasStatusNew()
    {
        var booking = new Booking();

        Assert.Equal(BookingStatus.New, booking.Status);
    }

    [Fact]
    public void Confirm_FromNew_SetsConfirmedAndDate()
    {
        var booking = new Booking();

        booking.Confirm();

        Assert.Equal(BookingStatus.Confirmed, booking.Status);
        Assert.NotNull(booking.ConfirmedAt);
    }

    [Fact]
    public void Confirm_FromCancelled_Throws()
    {
        var booking = new Booking();
        booking.Cancel();

        Assert.Throws<InvalidOperationException>(() => booking.Confirm());
    }

    [Fact]
    public void MarkPaid_FromNew_Throws()
    {
        var booking = new Booking();

        Assert.Throws<InvalidOperationException>(() => booking.MarkPaid());
    }

    [Fact]
    public void MarkPaid_FromConfirmed_SetsPaid()
    {
        var booking = new Booking();
        booking.Confirm();

        booking.MarkPaid();

        Assert.Equal(BookingStatus.Paid, booking.Status);
    }

    [Fact]
    public void Cancel_FromNew_SetsCancelledAndDate()
    {
        var booking = new Booking();

        booking.Cancel();

        Assert.Equal(BookingStatus.Cancelled, booking.Status);
        Assert.NotNull(booking.CancelledAt);
    }

    [Fact]
    public void Cancel_FromPaid_Throws()
    {
        var booking = new Booking();
        booking.Confirm();
        booking.MarkPaid();

        Assert.Throws<InvalidOperationException>(() => booking.Cancel());
    }

    [Fact]
    public void Complete_FromPaid_SetsCompleted()
    {
        var booking = new Booking();
        booking.Confirm();
        booking.MarkPaid();

        booking.Complete();

        Assert.Equal(BookingStatus.Completed, booking.Status);
    }

    [Fact]
    public void Complete_FromConfirmed_Throws()
    {
        var booking = new Booking();
        booking.Confirm();

        Assert.Throws<InvalidOperationException>(() => booking.Complete());
    }
}
