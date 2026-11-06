using TravelAgency.Core.Entities;
using TravelAgency.Core.Models;

namespace TravelAgency.Core.Interfaces;

/// <summary>Розраховує ціну квитка для одного туриста (компонент патерну Decorator).</summary>
public interface ITicketPriceCalculator
{
    decimal Calculate(TouristInfo tourist, Tour tour, TourGroup group);
}