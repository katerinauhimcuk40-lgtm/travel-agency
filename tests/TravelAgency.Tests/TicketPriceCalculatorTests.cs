using TravelAgency.Core.Entities;
using TravelAgency.Core.Enums;
using TravelAgency.Core.Models;
using TravelAgency.Infrastructure.Pricing;
using TravelAgency.Infrastructure.Repositories;
using TravelAgency.Infrastructure.Services;

namespace TravelAgency.Tests;

public class TicketPriceCalculatorTests
{
    private static readonly Tour ParisTour = new() { Id = 1, BasePrice = 500 };

    private static TourGroup GroupIn(int days) =>
        new() { DepartureDate = DateTime.UtcNow.AddDays(days) };

    private static TouristInfo TouristOfAge(int age) =>
        new("Test Tourist", "AB123456", Gender.Male, age, null);

    [Fact]
    public void Base_ReturnsTourBasePrice()
    {
        var calc = new BaseTicketPriceCalculator();

        Assert.Equal(500m, calc.Calculate(TouristOfAge(30), ParisTour, GroupIn(10)));
    }

    [Fact]
    public void ChildDiscount_Child_GetsHalfPrice()
    {
        var calc = new ChildDiscountDecorator(new BaseTicketPriceCalculator());

        Assert.Equal(250m, calc.Calculate(TouristOfAge(5), ParisTour, GroupIn(10)));
    }

    [Fact]
    public void ChildDiscount_Adult_PaysFullPrice()
    {
        var calc = new ChildDiscountDecorator(new BaseTicketPriceCalculator());

        Assert.Equal(500m, calc.Calculate(TouristOfAge(30), ParisTour, GroupIn(10)));
    }

    [Fact]
    public void ChildDiscount_AgeTwelve_IsNotChild()
    {
        var calc = new ChildDiscountDecorator(new BaseTicketPriceCalculator());

        Assert.Equal(500m, calc.Calculate(TouristOfAge(12), ParisTour, GroupIn(10)));
    }

    [Fact]
    public void EarlyBooking_FarDeparture_GetsFivePercentOff()
    {
        var calc = new EarlyBookingDecorator(new BaseTicketPriceCalculator());

        Assert.Equal(475m, calc.Calculate(TouristOfAge(30), ParisTour, GroupIn(90)));
    }

    [Fact]
    public void EarlyBooking_NearDeparture_PaysFullPrice()
    {
        var calc = new EarlyBookingDecorator(new BaseTicketPriceCalculator());

        Assert.Equal(500m, calc.Calculate(TouristOfAge(30), ParisTour, GroupIn(30)));
    }

    [Fact]
    public void Decorators_CanBeCombined()
    {
        var calc = new EarlyBookingDecorator(new ChildDiscountDecorator(new BaseTicketPriceCalculator()));

        // 500 → 250 (дитина) → 237.5 (раннє бронювання)
        Assert.Equal(237.5m, calc.Calculate(TouristOfAge(5), ParisTour, GroupIn(90)));
    }

    [Fact]
    public void Decorator_NullInner_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new ChildDiscountDecorator(null!));
    }

    [Fact]
    public async Task CreateBooking_AppliesDiscountsToTicketsAndTotal()
    {
        using var context = TestDb.CreateContext();
        var group = await TestDb.SeedGroupAsync(context, daysToDeparture: 90);
        var calculator = new EarlyBookingDecorator(new ChildDiscountDecorator(new BaseTicketPriceCalculator()));
        var service = new BookingService(new UnitOfWork(context), TestDb.Encryption(), calculator);
        var tourists = new List<TouristInfo> { TouristOfAge(30), TouristOfAge(5) };

        var booking = await service.CreateBookingAsync(1, group.Id, tourists);

        Assert.Equal(475m, booking.Tourists[0].TicketPrice);
        Assert.Equal(237.5m, booking.Tourists[1].TicketPrice);
        Assert.Equal(712.5m, booking.TotalPrice);
    }
}