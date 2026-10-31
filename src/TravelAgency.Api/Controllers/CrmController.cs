using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TravelAgency.Api.Auth;
using TravelAgency.Api.Dtos;
using TravelAgency.Core.Enums;
using TravelAgency.Core.Interfaces;
using TravelAgency.Core.Models;

namespace TravelAgency.Api.Controllers;

/// <summary>Кабінет менеджера (CRM): заявки та клієнти. Доступно лише ролям Manager і Admin.
/// Зміна статусів — у BookingsController (confirm, cancel).</summary>
[ApiController]
[Route("api/crm")]
[Authorize(Roles = "Manager,Admin")]
public class CrmController : ControllerBase
{
    private readonly ICrmService _crm;
    private readonly IEncryptionService _encryption;

    public CrmController(ICrmService crm, IEncryptionService encryption)
    {
        _crm = crm;
        _encryption = encryption;
    }

    /// <summary>UC-13: список заявок усіх клієнтів із фільтром за статусом і клієнтом.</summary>
    [HttpGet("bookings")]
    public async Task<ActionResult<PagedResponse<BookingSummaryResponse>>> SearchBookings(
        [FromQuery] BookingStatus? status,
        [FromQuery] string? customer,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var criteria = new BookingSearchCriteria
        {
            Status = status,
            Customer = customer,
            Page = page,
            PageSize = pageSize
        };

        var result = await _crm.SearchBookingsAsync(criteria);
        return Ok(PagedResponse<BookingSummaryResponse>.From(result, BookingSummaryResponse.From));
    }

    /// <summary>UC-13, UC-14: деталі заявки з клієнтом, туристами (паспорт замаскований) та оплатами.</summary>
    [HttpGet("bookings/{id:int}")]
    public async Task<ActionResult<CrmBookingDetailsResponse>> GetBooking(int id)
    {
        var booking = await _crm.GetBookingAsync(id);

        return Ok(CrmBookingDetailsResponse.From(
            booking,
            cipher => PassportMask.FromEncrypted(_encryption, cipher)));
    }

    /// <summary>UC-11: клієнти з пошуком за іменем або поштою.</summary>
    [HttpGet("customers")]
    public async Task<ActionResult<PagedResponse<CustomerResponse>>> GetCustomers(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _crm.GetCustomersAsync(search, page, pageSize);
        return Ok(PagedResponse<CustomerResponse>.From(result, CustomerResponse.From));
    }
}
