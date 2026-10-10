using Microsoft.AspNetCore.Mvc;

using TravelAgency.Core.Entities;
using TravelAgency.Core.Interfaces;

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

    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
    {
        try
        {
            var booking = await _bookingService.CreateBookingAsync(dto.UserId, dto.TourGroupId, dto.Tourists);
            return Ok(booking);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/confirm")]
    public async Task<IActionResult> ConfirmBooking(int id)
    {
        try
        {
            await _bookingService.ConfirmBookingAsync(id);
            return Ok(new { message = "Booking confirmed successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/pay")]
    public async Task<IActionResult> PayBooking(int id, [FromBody] PayDto dto)
    {
        try
        {
            await _paymentService.ProcessPaymentAsync(id, dto.Amount);
            return Ok(new { message = "Payment processed successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

public record CreateBookingDto(int UserId, int TourGroupId, List<Tourist> Tourists);
public record PayDto(decimal Amount);