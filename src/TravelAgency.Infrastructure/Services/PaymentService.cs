using TravelAgency.Core.Entities;
using TravelAgency.Core.Entities;
using TravelAgency.Core.Enums;
using TravelAgency.Core.Interfaces;
using TravelAgency.Infrastructure.Payments;

namespace TravelAgency.Infrastructure.Services;

public class PaymentService : IPaymentService
{
    private const string Currency = "UAH";

    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymentGatewayFactory _gatewayFactory;

    public PaymentService(IUnitOfWork unitOfWork, IPaymentGatewayFactory? gatewayFactory = null)
    {
        _unitOfWork = unitOfWork;
        _gatewayFactory = gatewayFactory ?? new PaymentGatewayFactory();
    }

    public async Task ProcessPaymentAsync(int bookingId, decimal amount, string provider = "Fake")
    {
        var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId)
            ?? throw new KeyNotFoundException("Booking not found");

        if (amount != booking.TotalPrice)
            throw new InvalidOperationException("Payment amount must be equal to the booking total price.");

        // Перевіряємо статус ДО звернення до шлюзу, щоб не списати гроші за неоплачуване бронювання
        if (booking.Status != BookingStatus.Confirmed)
            throw new InvalidOperationException(
                $"Cannot pay a booking in status {booking.Status}. Booking must be confirmed first.");

        var gateway = _gatewayFactory.Create(provider);            // Factory Method
        var result = await gateway.ChargeAsync(booking.TotalPrice, Currency);

        var payment = new Payment
        {
            BookingId = booking.Id,
            Amount = booking.TotalPrice,
            Currency = Currency,
            Provider = gateway.Name,
            ExternalTransactionId = result.TransactionId
        };

        if (!result.Success)
        {
            // Невдала спроба фіксується в історії платежів, бронювання лишається Confirmed
            payment.MarkFailed();
            await _unitOfWork.Payments.AddAsync(payment);
            await _unitOfWork.SaveChangesAsync();
            throw new InvalidOperationException("Payment was declined by the payment provider.");
        }

        booking.MarkPaid();
        payment.MarkSucceeded();

        await _unitOfWork.Payments.AddAsync(payment);
        _unitOfWork.Bookings.Update(booking);
        await _unitOfWork.SaveChangesAsync();
    }
}