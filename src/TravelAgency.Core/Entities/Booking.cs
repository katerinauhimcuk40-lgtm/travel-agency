using TravelAgency.Core.Enums;
using TravelAgency.Core.States;

namespace TravelAgency.Core.Entities;

/// <summary>Бронювання (заявка). Життєвий цикл: New → Confirmed → Paid → Completed; Cancelled з New/Confirmed.</summary>
public class Booking
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public int TourGroupId { get; set; }
    public TourGroup? TourGroup { get; set; }
    public int PersonsCount { get; set; }
    public decimal TotalPrice { get; set; }
    public BookingStatus Status { get; private set; } = BookingStatus.New;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ConfirmedAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public int? CreatedByUserId { get; set; }
    public List<Tourist> Tourists { get; set; } = [];
    public List<Payment> Payments { get; set; } = [];

    public void Confirm()
    {
        Status = BookingStates.For(Status).Confirm();
        ConfirmedAt = DateTime.UtcNow;
    }

    public void MarkPaid() => Status = BookingStates.For(Status).Pay();

    public void Cancel()
    {
        Status = BookingStates.For(Status).Cancel();
        CancelledAt = DateTime.UtcNow;
    }

    public void Complete() => Status = BookingStates.For(Status).Complete();
}
