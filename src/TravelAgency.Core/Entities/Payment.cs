using TravelAgency.Core.Enums;

namespace TravelAgency.Core.Entities;

public class Payment
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public Booking? Booking { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "UAH";
    public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;
    public string Provider { get; set; } = string.Empty;
    public string? ExternalTransactionId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PaidAt { get; private set; }

    public void MarkSucceeded()
    {
        Status = PaymentStatus.Succeeded;
        PaidAt = DateTime.UtcNow;
    }

    public void MarkFailed()
    {
        Status = PaymentStatus.Failed;
    }
}
