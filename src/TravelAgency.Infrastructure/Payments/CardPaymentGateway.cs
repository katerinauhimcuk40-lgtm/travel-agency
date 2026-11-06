using TravelAgency.Core.Interfaces;

namespace TravelAgency.Infrastructure.Payments;

/// <summary>Імітація карткового шлюзу: відхиляє платіж із нульовою або від'ємною сумою.</summary>
public class CardPaymentGateway : IPaymentGateway
{
    public string Name => "Card";

    public Task<PaymentGatewayResult> ChargeAsync(decimal amount, string currency)
    {
        if (amount <= 0)
            return Task.FromResult(new PaymentGatewayResult(false, null));

        return Task.FromResult(new PaymentGatewayResult(true, $"CARD-{Guid.NewGuid():N}"));
    }
}