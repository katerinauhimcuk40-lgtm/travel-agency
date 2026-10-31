using TravelAgency.Core.Entities;
using TravelAgency.Core.Models;

namespace TravelAgency.Core.Interfaces;

public interface IUserRepository : IRepository<User>
{
    /// <summary>Клієнти (роль Client) з пошуком за іменем або поштою, відсортовані за іменем, з кількістю бронювань.</summary>
    Task<PagedResult<CustomerSummary>> SearchCustomersAsync(string? search, int page, int pageSize);
}
