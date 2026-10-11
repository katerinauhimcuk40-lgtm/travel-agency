using TravelAgency.Core.Enums;

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
        if (Status != BookingStatus.New)
            throw new InvalidOperationException($"Cannot confirm a booking in status {Status}.");

        Status = BookingStatus.Confirmed;
        ConfirmedAt = DateTime.UtcNow;
    }

    public void MarkPaid()
    {
        if (Status != BookingStatus.Confirmed)
            throw new InvalidOperationException($"Cannot pay a booking in status {Status}. Booking must be confirmed first.");

        Status = BookingStatus.Paid;
    }

    public void Cancel()
    {
        if (Status != BookingStatus.New && Status != BookingStatus.Confirmed)
            throw new InvalidOperationException($"Cannot cancel a booking in status {Status}.");

        Status = BookingStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        if (Status != BookingStatus.Paid)
            throw new InvalidOperationException($"Cannot complete a booking in status {Status}.");

        Status = BookingStatus.Completed;
    }
}
