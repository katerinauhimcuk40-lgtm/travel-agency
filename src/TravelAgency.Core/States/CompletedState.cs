using TravelAgency.Core.Enums;

namespace TravelAgency.Core.States;

public sealed class CompletedState : BookingStateBase
{
    protected override BookingStatus Current => BookingStatus.Completed;
}