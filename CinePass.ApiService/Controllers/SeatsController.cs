using CinePass.Domain;
using CinePass.Domain.IRepository;
using CinePass.Shared.DTOs.Seat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinePass.ApiService.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize(Roles = "Admin")]
public class SeatsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public SeatsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Lấy danh sách ghế theo phòng chiếu
    /// </summary>
    [AllowAnonymous]
    [HttpGet("screen/{screenId}")]
    public async Task<ActionResult> GetSeatsByScreen(int screenId)
    {
        try
        {
            var seats = await _unitOfWork.Seats.GetSeatsByScreenAsync(screenId);
            
            var seatDtos = seats.Select(s => new
            {
                seatId = s.SeatID,
                seatNumber = s.SeatNumber,
                seatType = s.SeatType.ToString(),
                screenId = s.ScreenID
            });

            return Ok(new
            {
                success = true,
                data = seatDtos
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Cập nhật loại ghế (Admin only)
    /// </summary>
    [HttpPut("{id}/type")]
    public async Task<ActionResult> UpdateSeatType(int id, [FromBody] UpdateSeatTypeDto dto)
    {
        try
        {
            var seat = await _unitOfWork.Seats.GetByIdAsync(id);
            if (seat == null)
                return NotFound(new { success = false, message = "Seat not found" });

            if (!Enum.TryParse<SeatType>(dto.SeatType, out var seatType))
                return BadRequest(new { success = false, message = "Invalid seat type" });

            seat.SeatType = seatType;
            await _unitOfWork.Seats.UpdateAsync(seat);
            await _unitOfWork.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Seat type updated successfully",
                data = new
                {
                    seatId = seat.SeatID,
                    seatNumber = seat.SeatNumber,
                    seatType = seat.SeatType.ToString()
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }
}