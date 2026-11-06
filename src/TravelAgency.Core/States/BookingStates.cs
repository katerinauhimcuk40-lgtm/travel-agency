using TravelAgency.Core.Enums;

namespace TravelAgency.Core.States;

public static class BookingStates
{
    public static IBookingState For(BookingStatus status) => status switch
    {
        BookingStatus.New => new NewState(),
        BookingStatus.Confirmed => new ConfirmedState(),
        BookingStatus.Paid => new PaidState(),
        BookingStatus.Completed => new CompletedState(),
        BookingStatus.Cancelled => new CancelledState(),
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Unknown booking status.")
    };
}