using TravelAgency.Core.Interfaces;

namespace TravelAgency.Infrastructure.Payments;

/// <summary>Тестовий шлюз: завжди успішно проводить платіж.</summary>
public class FakePaymentGateway : IPaymentGateway
{
    public string Name => "Fake";

    public Task<PaymentGatewayResult> ChargeAsync(decimal amount, string currency) =>
        Task.FromResult(new PaymentGatewayResult(true, Guid.NewGuid().ToString()));
}