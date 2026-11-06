using TravelAgency.Core.Entities;
using TravelAgency.Core.Interfaces;
using TravelAgency.Core.Models;

namespace TravelAgency.Infrastructure.Pricing;

public class BaseTicketPriceCalculator : ITicketPriceCalculator
{
    public decimal Calculate(TouristInfo tourist, Tour tour, TourGroup group) => tour.BasePrice;
}