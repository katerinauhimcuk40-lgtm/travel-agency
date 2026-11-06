namespace TravelAgency.Core.Interfaces;

/// <summary>Фабрика платіжних шлюзів: створює шлюз за назвою провайдера.</summary>
public interface IPaymentGatewayFactory
{
    /// <exception cref="ArgumentException">Невідомий провайдер.</exception>
    IPaymentGateway Create(string provider);
}