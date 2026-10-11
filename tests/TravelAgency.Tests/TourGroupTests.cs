using TravelAgency.Core.Entities;

namespace TravelAgency.Tests;

public class TourGroupTests
{
    private static TourGroup CreateGroup(int total = 10, int available = 10) =>
        new() { SeatsTotal = total, SeatsAvailable = available };

    [Fact]
    public void Reserve_DecreasesAvailableSeats()
    {
        var group = CreateGroup();

        group.Reserve(3);

        Assert.Equal(7, group.SeatsAvailable);
        Assert.Equal(3, group.TouristsCount);
    }

    [Fact]
    public void Reserve_MoreThanAvailable_Throws()
    {
        var group = CreateGroup(total: 10, available: 2);

        Assert.Throws<InvalidOperationException>(() => group.Reserve(3));
        Assert.Equal(2, group.SeatsAvailable);
    }

    [Fact]
    public void Reserve_ZeroPersons_Throws()
    {
        var group = CreateGroup();

        Assert.Throws<ArgumentOutOfRangeException>(() => group.Reserve(0));
    }

    [Fact]
    public void Release_IncreasesAvailableSeats()
    {
        var group = CreateGroup(total: 10, available: 5);

        group.Release(2);

        Assert.Equal(7, group.SeatsAvailable);
    }

    [Fact]
    public void Release_AboveTotal_Throws()
    {
        var group = CreateGroup(total: 10, available: 9);

        Assert.Throws<InvalidOperationException>(() => group.Release(2));
    }
}
