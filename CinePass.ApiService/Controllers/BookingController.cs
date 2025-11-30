using CinePass.Core.Services;
using CinePass.Shared.DTOs.Booking;
using CinePass.Shared.DTOs.QRCode;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinePass.ApiService.Controllers;

[Route("api/[controller]")]
[ApiController]
// [Authorize]
public class BookingController : ControllerBase
{
    private readonly BookingService _bookingService;

    public BookingController(BookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpPost]
    public async Task<ActionResult<BookingResponse>> Create([FromBody] BookingRequest dto)
    {
        try
        {
            var booking = await _bookingService.CreateBookingAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = booking.BookingID }, booking);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookingResponse>> GetById(int id)
    {
        var booking = await _bookingService.GetBookingByIdAsync(id);
        if (booking == null)
            return NotFound(new { message = "Booking not found" });
        return Ok(booking);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<BookingResponse>>> GetUserBookings(int userId)
    {
        var bookings = await _bookingService.GetUserBookingsAsync(userId);
        return Ok(bookings);
    }

    [HttpGet("{id}/qrcode")]
    public async Task<ActionResult<QRCodeDto>> GetQRCode(int id)
    {
        var qrCode = await _bookingService.GetQRCodeAsync(id);
        if (qrCode == null)
            return NotFound(new { message = "Booking not found" });
        return Ok(qrCode);
    }

    [HttpPost("{id}/cancel")]
    public async Task<ActionResult> Cancel(int id)
    {
        var result = await _bookingService.CancelBookingAsync(id);
        if (!result)
            return NotFound(new { message = "Booking not found or already cancelled" });
        return Ok(new { message = "Booking cancelled successfully" });
    }

    // [Authorize(Roles = "Admin")]
    [HttpPost("{id}/confirm")]
    public async Task<ActionResult> Confirm(int id)
    {
        var result = await _bookingService.ConfirmBookingAsync(id);
        if (!result)
            return NotFound(new { message = "Booking not found" });
        return Ok(new { message = "Booking confirmed successfully" });
    }

    [HttpPost("checkin")]
    public async Task<ActionResult<BookingResponse>> CheckIn([FromBody] CheckInDto dto)
    {
        try
        {
            var booking = await _bookingService.CheckInAsync(dto.QRToken);
            return Ok(new { message = "Check-in successful", booking });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}