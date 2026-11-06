using TravelAgency.Core.Enums;

namespace TravelAgency.Core.States;

public sealed class NewState : BookingStateBase
{
    protected override BookingStatus Current => BookingStatus.New;

    public override BookingStatus Confirm() => BookingStatus.Confirmed;
    public override BookingStatus Cancel() => BookingStatus.Cancelled;
}