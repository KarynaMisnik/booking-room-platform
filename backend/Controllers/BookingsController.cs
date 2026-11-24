using backend.Services;
using Microsoft.AspNetCore.Mvc;
using backend.DTOs;
using backend.Models;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly BookingService _service;

    public BookingsController(BookingService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<BookingResponse>> CreateBooking(
    CreateBookingRequest request)
    {
        var booking = new Booking
        {
            RoomId = request.RoomId,
            GuestName = request.GuestName,
            GuestEmail = request.GuestEmail,
            CheckIn = request.CheckIn,
            CheckOut = request.CheckOut
        };

        try
        {
            var createdBooking = await _service.CreateBookingAsync(booking);

            return StatusCode(201, createdBooking);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
    [HttpPost("cancel")]
    public async Task<IActionResult> CancelBooking(
    CancelBookingRequest request)
    {
        try
        {
            await _service.CancelBookingAsync(request.ManagementToken);

            return Ok(new { message = "Booking cancelled successfully." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}