using TravelAgency.Core.Enums;

namespace TravelAgency.Core.Entities;

/// <summary>Туристичний маршрут (таблиця Tours).</summary>
public class Tour
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Country { get; set; } = string.Empty;
    public string Cities { get; set; } = string.Empty;
    public TransportType TransportType { get; set; } = TransportType.Plane;
    public MealPlan MealPlan { get; set; } = MealPlan.AI;
    public string AccommodationType { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public bool IsActive { get; set; } = true;
    public List<TourGroup> TourGroups { get; set; } = [];
}
