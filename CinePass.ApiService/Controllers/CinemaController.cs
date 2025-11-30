using CinePass.Core.Services;
using CinePass.Domain.IRepository;
using CinePass.Domain.Models;
using CinePass.Shared.DTOs.Cinema;
using CinePass.Shared.DTOs.Screen;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinePass.ApiService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CinemasController : ControllerBase
{
    private readonly CinemaService _cinemaService;

    public CinemasController(CinemaService cinemaService)
    {
        _cinemaService = cinemaService;
    }

    /// <summary>
    /// Lấy danh sách tất cả rạp chiếu
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CinemaResponse>>> GetAll()
    {
        try
        {
            var cinemas = await _cinemaService.GetAllCinemasAsync();
            return Ok(new
            {
                success = true,
                data = cinemas
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy thông tin rạp chiếu theo ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<CinemaResponse>> GetById(int id)
    {
        try
        {
            var cinema = await _cinemaService.GetCinemaByIdAsync(id);
            if (cinema == null)
                return NotFound(new { success = false, message = "Cinema not found" });

            return Ok(new
            {
                success = true,
                data = cinema
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy thông tin rạp chiếu kèm danh sách phòng chiếu
    /// </summary>
    [HttpGet("{id}/screens")]
    public async Task<ActionResult<CinemaResponse>> GetWithScreens(int id)
    {
        try
        {
            var cinema = await _cinemaService.GetCinemaWithScreensAsync(id);
            if (cinema == null)
                return NotFound(new { success = false, message = "Cinema not found" });

            return Ok(new
            {
                success = true,
                data = cinema
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy danh sách rạp đang chiếu phim cụ thể
    /// </summary>
    [HttpGet("showing-movie/{movieId}")]
    public async Task<ActionResult<IEnumerable<CinemaResponse>>> GetCinemasShowingMovie(int movieId)
    {
        try
        {
            var cinemas = await _cinemaService.GetCinemasShowingMovieAsync(movieId);
            return Ok(new
            {
                success = true,
                data = cinemas
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Tạo rạp chiếu mới (Admin only)
    /// </summary>
    // [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<CinemaResponse>> Create([FromBody] CinemaRequest dto)
    {
        try
        {
            var cinema = await _cinemaService.CreateCinemaAsync(dto);
            return CreatedAtAction(
                nameof(GetById),
                new { id = cinema.CinemaID },
                new { success = true, data = cinema }
            );
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Cập nhật thông tin rạp chiếu (Admin only)
    /// </summary>
    // [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<CinemaResponse>> Update(int id, [FromBody] CinemaRequest dto)
    {
        try
        {
            var cinema = await _cinemaService.UpdateCinemaAsync(id, dto);
            if (cinema == null)
                return NotFound(new { success = false, message = "Cinema not found" });

            return Ok(new
            {
                success = true,
                message = "Cinema updated successfully",
                data = cinema
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Xóa rạp chiếu (Admin only)
    /// </summary>
    // [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await _cinemaService.DeleteCinemaAsync(id);
            return Ok(new
            {
                success = true,
                message = "Cinema deleted successfully"
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