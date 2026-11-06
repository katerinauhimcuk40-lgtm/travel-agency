using TravelAgency.Core.Enums;

namespace TravelAgency.Core.States;

public sealed class ConfirmedState : BookingStateBase
{
    protected override BookingStatus Current => BookingStatus.Confirmed;

    public override BookingStatus Pay() => BookingStatus.Paid;
    public override BookingStatus Cancel() => BookingStatus.Cancelled;
}