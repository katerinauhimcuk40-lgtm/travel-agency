using TravelAgency.Core.Interfaces;

namespace TravelAgency.Infrastructure.Payments;

public class PaymentGatewayFactory : IPaymentGatewayFactory
{
    private readonly Dictionary<string, Func<IPaymentGateway>> _creators =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["Fake"] = () => new FakePaymentGateway(),
            ["Card"] = () => new CardPaymentGateway()
        };

    public IPaymentGateway Create(string provider)
    {
        if (provider is not null && _creators.TryGetValue(provider, out var create))
            return create();

        throw new ArgumentException($"Unknown payment provider '{provider}'.", nameof(provider));
    }
}