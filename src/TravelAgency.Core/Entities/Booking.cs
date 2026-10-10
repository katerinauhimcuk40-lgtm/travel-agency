using TravelAgency.Core.Enums;

namespace TravelAgency.Core.Entities;

public class Booking
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public int TourGroupId { get; set; }
    public TourGroup? TourGroup { get; set; }
    public BookingStatus Status { get; private set; } = BookingStatus.Pending;
    public decimal TotalAmount { get; set; }
    public List<Tourist> Tourists { get; set; } = [];
    public Payment? Payment { get; set; }

    public void Reserve()
    {
        if (Status == BookingStatus.Pending)
        {
            Status = BookingStatus.Reserved;
        }
    }

    public void Confirm()
    {
        if (Status == BookingStatus.Reserved || Status == BookingStatus.Pending)
        {
            Status = BookingStatus.Confirmed;
        }
    }

    public void Cancel()
    {
        if (Status != BookingStatus.Confirmed)
        {
            Status = BookingStatus.Cancelled;
        }
    }
}