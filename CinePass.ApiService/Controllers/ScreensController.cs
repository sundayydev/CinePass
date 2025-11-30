using CinePass.Core.Services;
using CinePass.Shared.DTOs.Screen;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinePass.ApiService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ScreensController : ControllerBase
{
    private readonly ScreenService _screenService;

    public ScreensController(ScreenService screenService)
    {
        _screenService = screenService;
    }

    /// <summary>
    /// Lấy danh sách tất cả phòng chiếu
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ScreenDto>>> GetAll()
    {
        try
        {
            var screens = await _screenService.GetAllScreensAsync();
            return Ok(new
            {
                success = true,
                data = screens
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy thông tin phòng chiếu theo ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ScreenDto>> GetById(int id)
    {
        try
        {
            var screen = await _screenService.GetScreenByIdAsync(id);
            if (screen == null)
                return NotFound(new { success = false, message = "Screen not found" });

            return Ok(new
            {
                success = true,
                data = screen
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy danh sách phòng chiếu theo rạp
    /// </summary>
    [HttpGet("cinema/{cinemaId}")]
    public async Task<ActionResult<IEnumerable<ScreenDto>>> GetByCinema(int cinemaId)
    {
        try
        {
            var screens = await _screenService.GetScreensByCinemaAsync(cinemaId);
            return Ok(new
            {
                success = true,
                data = screens
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy thông tin phòng chiếu kèm danh sách ghế
    /// </summary>
    [HttpGet("{id}/seats")]
    public async Task<ActionResult<ScreenWithSeatsDto>> GetWithSeats(int id)
    {
        try
        {
            var screen = await _screenService.GetScreenWithSeatsAsync(id);
            if (screen == null)
                return NotFound(new { success = false, message = "Screen not found" });

            return Ok(new
            {
                success = true,
                data = screen
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Tạo phòng chiếu mới (Admin only)
    /// </summary>
    // [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ScreenDto>> Create([FromBody] ScreenDto dto)
    {
        try
        {
            var screen = await _screenService.CreateScreenAsync(dto);
            return CreatedAtAction(
                nameof(GetById), 
                new { id = screen.ScreenID }, 
                new { success = true, data = screen }
            );
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Cập nhật thông tin phòng chiếu (Admin only)
    /// </summary>
    // [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<ScreenDto>> Update(int id, [FromBody] ScreenDto dto)
    {
        try
        {
            var screen = await _screenService.UpdateScreenAsync(id, dto);
            if (screen == null)
                return NotFound(new { success = false, message = "Screen not found" });

            return Ok(new
            {
                success = true,
                message = "Screen updated successfully",
                data = screen
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Xóa phòng chiếu (Admin only)
    /// </summary>
    // [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await _screenService.DeleteScreenAsync(id);
            return Ok(new
            {
                success = true,
                message = "Screen deleted successfully"
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Khởi tạo ghế ngồi cho phòng chiếu (Admin only)
    /// </summary>
    /// <remarks>
    /// Tạo sơ đồ ghế tự động theo số hàng và số ghế mỗi hàng.
    /// - Ghế Regular: Các hàng đầu
    /// - Ghế VIP: 2 hàng cuối
    /// - Ghế Couple: Các ghế đôi ở giữa 2 hàng cuối
    /// </remarks>
    // [Authorize(Roles = "Admin")]
    [HttpPost("{id}/initialize-seats")]
    public async Task<ActionResult> InitializeSeats(int id, [FromBody] InitializeSeatsDto dto)
    {
        try
        {
            var result = await _screenService.InitializeSeatsForScreenAsync(id, dto.Rows, dto.SeatsPerRow);
            
            return Ok(new
            {
                success = true,
                message = $"Successfully initialized {dto.Rows * dto.SeatsPerRow} seats for screen",
                data = new
                {
                    screenId = id,
                    totalSeats = dto.Rows * dto.SeatsPerRow,
                    rows = dto.Rows,
                    seatsPerRow = dto.SeatsPerRow
                }
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }
}