using TravelAgency.Core.Entities;
using TravelAgency.Core.Interfaces;
using TravelAgency.Core.Models;

namespace TravelAgency.Infrastructure.Pricing;

/// <summary>Знижка 50% для дітей до 12 років.</summary>
public class ChildDiscountDecorator : TicketPriceDecorator
{
    public const int MaxChildAge = 12;
    public const decimal Factor = 0.5m;

    public ChildDiscountDecorator(ITicketPriceCalculator inner) : base(inner) { }

    public override decimal Calculate(TouristInfo tourist, Tour tour, TourGroup group)
    {
        var price = Inner.Calculate(tourist, tour, group);
        return tourist.Age < MaxChildAge ? Math.Round(price * Factor, 2) : price;
    }
}