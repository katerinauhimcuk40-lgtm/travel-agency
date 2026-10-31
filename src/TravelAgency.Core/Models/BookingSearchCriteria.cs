using TravelAgency.Core.Enums;

namespace TravelAgency.Core.Models;

/// <summary>Критерії пошуку заявок у кабінеті менеджера (CRM): статус і частина імені або пошти клієнта.</summary>
public class BookingSearchCriteria
{
    public BookingStatus? Status { get; set; }
    public string? Customer { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
