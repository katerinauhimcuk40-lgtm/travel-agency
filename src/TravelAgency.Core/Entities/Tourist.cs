using TravelAgency.Core.Enums;

namespace TravelAgency.Core.Entities;

/// <summary>Мандрівник у складі бронювання (таблиця Tourists).</summary>
public class Tourist
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public Booking? Booking { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PassportEncrypted { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public int Age { get; set; }
    public string? ChildrenInfo { get; set; }
    public decimal TicketPrice { get; set; }
}
