using TravelAgency.Core.Entities;
using TravelAgency.Core.Interfaces;

namespace TravelAgency.Infrastructure.Services;

public class PaymentService : IPaymentService
{
    private readonly IUnitOfWork _unitOfWork;

    public PaymentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task ProcessPaymentAsync(int bookingId, decimal amount)
    {
        var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId)
            ?? throw new KeyNotFoundException("Booking not found");

        if (amount != booking.TotalPrice)
            throw new InvalidOperationException("Payment amount must be equal to the booking total price.");

        // Статус бронювання (Confirmed) перевіряє Booking.MarkPaid()
        var payment = new Payment
        {
            BookingId = booking.Id,
            Amount = booking.TotalPrice,
            Provider = "Fake",
            ExternalTransactionId = Guid.NewGuid().ToString()
        };

        booking.MarkPaid();
        payment.MarkSucceeded();

        await _unitOfWork.Payments.AddAsync(payment);
        _unitOfWork.Bookings.Update(booking);
        await _unitOfWork.SaveChangesAsync();
    }
}
