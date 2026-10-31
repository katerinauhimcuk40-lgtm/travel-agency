using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TravelAgency.Api.Auth;
using TravelAgency.Api.Dtos;
using TravelAgency.Core.Interfaces;
using TravelAgency.Core.Models;

namespace TravelAgency.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly IPaymentService _paymentService;

    public BookingsController(IBookingService bookingService, IPaymentService paymentService)
    {
        _bookingService = bookingService;
        _paymentService = paymentService;
    }

    /// <summary>UC-06 / UC-12: забронювати тур (виліт групи) для списку мандрівників.
    /// Клієнт бронює для себе, менеджер може вказати UserId клієнта.</summary>
    [HttpPost]
    [ProducesResponseType<BookingResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<BookingResponse>> Create([FromBody] CreateBookingRequest request)
    {
        var ownerId = User.IsStaff() && request.UserId.HasValue
            ? request.UserId.Value
            : User.GetUserId();

        var tourists = request.Tourists
            .Select(t => new TouristInfo(t.FullName, t.Passport, t.Gender, t.Age, t.ChildrenInfo))
            .ToList();

        var booking = await _bookingService.CreateBookingAsync(ownerId, request.TourGroupId, tourists);

        return Ok(BookingResponse.From(booking));
    }

    /// <summary>UC-08: мої бронювання.</summary>
    [HttpGet("my")]
    public async Task<ActionResult<List<BookingDetailsResponse>>> GetMy()
    {
        var bookings = await _bookingService.GetUserBookingsAsync(User.GetUserId());
        return Ok(bookings.Select(BookingDetailsResponse.From).ToList());
    }

    /// <summary>Деталі бронювання (власник або співробітник).</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookingDetailsResponse>> GetById(int id)
    {
        var booking = await _bookingService.GetBookingAsync(id);

        if (!CanAccess(booking.UserId))
            return Forbid();

        return Ok(BookingDetailsResponse.From(booking));
    }

    /// <summary>UC-13: підтвердити бронювання.</summary>
    [HttpPost("{id:int}/confirm")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> Confirm(int id)
    {
        await _bookingService.ConfirmBookingAsync(id);
        return NoContent();
    }

    /// <summary>UC-09: скасувати бронювання (своє або будь-яке — для співробітника).</summary>
    [HttpPost("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        var booking = await _bookingService.GetBookingAsync(id);

        if (!CanAccess(booking.UserId))
            return Forbid();

        await _bookingService.CancelBookingAsync(id);
        return NoContent();
    }

    /// <summary>UC-10: оплатити бронювання (сума має дорівнювати вартості бронювання).</summary>
    [HttpPost("{id:int}/pay")]
    public async Task<IActionResult> Pay(int id, [FromBody] PayRequest request)
    {
        var booking = await _bookingService.GetBookingAsync(id);

        if (!CanAccess(booking.UserId))
            return Forbid();

        await _paymentService.ProcessPaymentAsync(id, request.Amount);
        return NoContent();
    }

    private bool CanAccess(int ownerId) => User.IsStaff() || ownerId == User.GetUserId();
}
