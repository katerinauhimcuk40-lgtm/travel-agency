using TravelAgency.Core.Enums;
using TravelAgency.Core.States;

namespace TravelAgency.Tests;

public class BookingStateTests
{
    [Theory]
    [InlineData(BookingStatus.New, typeof(NewState))]
    [InlineData(BookingStatus.Confirmed, typeof(ConfirmedState))]
    [InlineData(BookingStatus.Paid, typeof(PaidState))]
    [InlineData(BookingStatus.Completed, typeof(CompletedState))]
    [InlineData(BookingStatus.Cancelled, typeof(CancelledState))]
    public void For_ReturnsStateMatchingStatus(BookingStatus status, Type expected)
    {
        Assert.IsType(expected, BookingStates.For(status));
    }

    [Fact]
    public void For_UnknownStatus_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => BookingStates.For((BookingStatus)99));
    }

    [Fact]
    public void NewState_Confirm_ReturnsConfirmed()
    {
        Assert.Equal(BookingStatus.Confirmed, new NewState().Confirm());
    }

    [Fact]
    public void NewState_Pay_Throws()
    {
        Assert.Throws<InvalidOperationException>(() => new NewState().Pay());
    }

    [Fact]
    public void ConfirmedState_Pay_ReturnsPaid()
    {
        Assert.Equal(BookingStatus.Paid, new ConfirmedState().Pay());
    }

    [Fact]
    public void PaidState_Cancel_Throws()
    {
        Assert.Throws<InvalidOperationException>(() => new PaidState().Cancel());
    }

    [Fact]
    public void CancelledState_AnyTransition_Throws()
    {
        var state = new CancelledState();

        Assert.Throws<InvalidOperationException>(() => state.Confirm());
        Assert.Throws<InvalidOperationException>(() => state.Pay());
        Assert.Throws<InvalidOperationException>(() => state.Cancel());
        Assert.Throws<InvalidOperationException>(() => state.Complete());
    }
}