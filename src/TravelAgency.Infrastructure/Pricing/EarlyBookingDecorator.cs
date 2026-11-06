using TravelAgency.Core.Entities;
using TravelAgency.Core.Interfaces;
using TravelAgency.Core.Models;

namespace TravelAgency.Infrastructure.Pricing;

/// <summary>Знижка 5%, якщо до вильоту лишається щонайменше 60 днів.</summary>
public class EarlyBookingDecorator : TicketPriceDecorator
{
    public const int MinDaysBeforeDeparture = 60;
    public const decimal Factor = 0.95m;

    public EarlyBookingDecorator(ITicketPriceCalculator inner) : base(inner) { }

    public override decimal Calculate(TouristInfo tourist, Tour tour, TourGroup group)
    {
        var price = Inner.Calculate(tourist, tour, group);
        var daysLeft = (group.DepartureDate - DateTime.UtcNow).TotalDays;
        return daysLeft >= MinDaysBeforeDeparture ? Math.Round(price * Factor, 2) : price;
    }
}