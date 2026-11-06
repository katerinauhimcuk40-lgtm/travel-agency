using TravelAgency.Core.Enums;

namespace TravelAgency.Core.States;

public sealed class CancelledState : BookingStateBase
{
    protected override BookingStatus Current => BookingStatus.Cancelled;
}