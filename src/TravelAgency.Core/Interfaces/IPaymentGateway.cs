namespace TravelAgency.Core.Interfaces;

/// <summary>Результат спроби списання коштів платіжним шлюзом.</summary>
public record PaymentGatewayResult(bool Success, string? TransactionId);

/// <summary>Платіжний шлюз (продукт у патерні Factory Method).</summary>
public interface IPaymentGateway
{
    string Name { get; }
    Task<PaymentGatewayResult> ChargeAsync(decimal amount, string currency);
}