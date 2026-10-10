using TravelAgency.Core.Enums;

namespace TravelAgency.Core.Entities;

public class Payment
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public Booking? Booking { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;
    public DateTime PaymentDate { get; set; }

    public void Complete()
    {
        Status = PaymentStatus.Completed;
        PaymentDate = DateTime.UtcNow;
        Booking?.Confirm();
    }

    public void Fail()
    {
        Status = PaymentStatus.Failed;
    }
}