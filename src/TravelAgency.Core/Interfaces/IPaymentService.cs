namespace TravelAgency.Core.Interfaces;

public interface IPaymentService
{
    Task ProcessPaymentAsync(int bookingId, decimal amount);
}