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
        var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId);
        if (booking == null) throw new Exception("Booking not found");

        var payment = new Payment
        {
            BookingId = bookingId,
            Amount = amount
        };

        payment.Complete();

        await _unitOfWork.Payments.AddAsync(payment);
        _unitOfWork.Bookings.Update(booking);
        await _unitOfWork.SaveChangesAsync();
    }
}