namespace TravelAgency.Core.Entities;

public class Tourist
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PassportNumber { get; set; } = string.Empty;
    public int BookingId { get; set; }
    public Booking? Booking { get; set; }
}