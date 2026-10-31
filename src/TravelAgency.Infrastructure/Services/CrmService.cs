using TravelAgency.Core.Entities;
using TravelAgency.Core.Interfaces;
using TravelAgency.Core.Models;

namespace TravelAgency.Infrastructure.Services;

public class CrmService : ICrmService
{
    private const int MaxPageSize = 100;

    private readonly IUnitOfWork _unitOfWork;

    public CrmService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<Booking>> SearchBookingsAsync(BookingSearchCriteria criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria);

        (criteria.Page, criteria.PageSize) = NormalizePaging(criteria.Page, criteria.PageSize);

        return await _unitOfWork.Bookings.SearchAsync(criteria);
    }

    public async Task<Booking> GetBookingAsync(int bookingId)
    {
        return await _unitOfWork.Bookings.GetForStaffAsync(bookingId)
            ?? throw new KeyNotFoundException("Booking not found");
    }

    public async Task<PagedResult<CustomerSummary>> GetCustomersAsync(string? search, int page, int pageSize)
    {
        var (safePage, safePageSize) = NormalizePaging(page, pageSize);
        return await _unitOfWork.Users.SearchCustomersAsync(search, safePage, safePageSize);
    }

    private static (int Page, int PageSize) NormalizePaging(int page, int pageSize) =>
        (Math.Max(1, page), Math.Clamp(pageSize, 1, MaxPageSize));
}
