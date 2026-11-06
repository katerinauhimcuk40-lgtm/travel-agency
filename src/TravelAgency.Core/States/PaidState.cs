using TravelAgency.Core.Enums;

namespace TravelAgency.Core.States;

public sealed class PaidState : BookingStateBase
{
    protected override BookingStatus Current => BookingStatus.Paid;

    public override BookingStatus Complete() => BookingStatus.Completed;
}