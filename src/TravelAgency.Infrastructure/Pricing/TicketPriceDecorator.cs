using TravelAgency.Core.Entities;
using TravelAgency.Core.Interfaces;
using TravelAgency.Core.Models;

namespace TravelAgency.Infrastructure.Pricing;

/// <summary>Базовий декоратор: обгортає інший калькулятор і змінює його результат.</summary>
public abstract class TicketPriceDecorator : ITicketPriceCalculator
{
    protected ITicketPriceCalculator Inner { get; }

    protected TicketPriceDecorator(ITicketPriceCalculator inner)
    {
        Inner = inner ?? throw new ArgumentNullException(nameof(inner));
    }

    public abstract decimal Calculate(TouristInfo tourist, Tour tour, TourGroup group);
}