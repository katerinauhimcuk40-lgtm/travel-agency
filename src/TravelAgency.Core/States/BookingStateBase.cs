using TravelAgency.Core.Enums;

namespace TravelAgency.Core.States;

/// <summary>За замовчуванням усі переходи заборонені; конкретні стани перевизначають дозволені.</summary>
public abstract class BookingStateBase : IBookingState
{
    protected abstract BookingStatus Current { get; }

    public virtual BookingStatus Confirm() => throw Invalid("confirm");
    public virtual BookingStatus Pay() => throw Invalid("pay");
    public virtual BookingStatus Cancel() => throw Invalid("cancel");
    public virtual BookingStatus Complete() => throw Invalid("complete");

    private InvalidOperationException Invalid(string action) =>
        new($"Cannot {action} a booking in status {Current}.");
}