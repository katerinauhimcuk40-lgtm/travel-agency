using TravelAgency.Core.Enums;

namespace TravelAgency.Core.Models;

/// <summary>Критерії пошуку туру (UC-05): країна, дати вильоту, готель, ціна, харчування.</summary>
public class TourSearchCriteria
{
    public string? Country { get; set; }
    public DateTime? DepartureFrom { get; set; }
    public DateTime? DepartureTo { get; set; }
    public string? Hotel { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public MealPlan? MealPlan { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
