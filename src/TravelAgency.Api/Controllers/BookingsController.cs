using Microsoft.AspNetCore.Mvc;

using TravelAgency.Api.Dtos;
using TravelAgency.Core.Interfaces;
using TravelAgency.Core.Models;

namespace TravelAgency.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly IPaymentService _paymentService;

    public BookingsController(IBookingService bookingService, IPaymentService paymentService)
    {
        _bookingService = bookingService;
        _paymentService = paymentService;
    }

    /// <summary>UC-06: забронювати тур (виліт групи) для списку мандрівників.</summary>
    [HttpPost]
    [ProducesResponseType<BookingResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<BookingResponse>> Create([FromBody] CreateBookingRequest request)
    {
        var tourists = request.Tourists
            .Select(t => new TouristInfo(t.FullName, t.Passport, t.Gender, t.Age, t.ChildrenInfo))
            .ToList();

        var booking = await _bookingService.CreateBookingAsync(request.UserId, request.TourGroupId, tourists);

        return Ok(BookingResponse.From(booking));
    }

    /// <summary>UC-13: підтвердити бронювання.</summary>
    [HttpPost("{id:int}/confirm")]
    public async Task<IActionResult> Confirm(int id)
    {
        await _bookingService.ConfirmBookingAsync(id);
        return NoContent();
    }

    /// <summary>UC-09: скасувати бронювання.</summary>
    [HttpPost("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        await _bookingService.CancelBookingAsync(id);
        return NoContent();
    }

    /// <summary>UC-10: оплатити бронювання (сума має дорівнювати вартості бронювання).</summary>
    [HttpPost("{id:int}/pay")]
    public async Task<IActionResult> Pay(int id, [FromBody] PayRequest request)
    {
        await _paymentService.ProcessPaymentAsync(id, request.Amount);
        return NoContent();
    }
}
