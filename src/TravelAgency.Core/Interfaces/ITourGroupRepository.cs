using TravelAgency.Core.Entities;
using TravelAgency.Core.Models;

namespace TravelAgency.Core.Interfaces;

public interface ITourGroupRepository : IRepository<TourGroup>
{
    /// <summary>Пошук майбутніх груп активних турів за критеріями (з турами та готелями).</summary>
    Task<PagedResult<TourGroup>> SearchAsync(TourSearchCriteria criteria, DateTime now);
}
