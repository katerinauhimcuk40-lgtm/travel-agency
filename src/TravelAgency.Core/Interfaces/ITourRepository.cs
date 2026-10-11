using TravelAgency.Core.Entities;
using TravelAgency.Core.Models;

namespace TravelAgency.Core.Interfaces;

public interface ITourRepository : IRepository<Tour>
{
    /// <summary>Активні тури зі сторінковим виводом.</summary>
    Task<PagedResult<Tour>> GetPagedAsync(int page, int pageSize);

    /// <summary>Тур разом з майбутніми групами та їхніми готелями.</summary>
    Task<Tour?> GetWithUpcomingGroupsAsync(int id, DateTime now);
}
