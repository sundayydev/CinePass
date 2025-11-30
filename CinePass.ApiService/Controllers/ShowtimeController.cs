using CinePass.Core.Services;
using CinePass.Shared.DTOs.Seat;
using CinePass.Shared.DTOs.Showtime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinePass.ApiService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ShowtimesController : ControllerBase
{
    private readonly ShowtimeService _showtimeService;

    public ShowtimesController(ShowtimeService showtimeService)
    {
        _showtimeService = showtimeService;
    }

    [HttpGet("movie/{movieId}")]
    public async Task<ActionResult<IEnumerable<ShowtimeResponse>>> GetByMovie(int movieId)
    {
        var showtimes = await _showtimeService.GetShowtimesByMovieAsync(movieId);
        return Ok(showtimes);
    }

    [HttpGet("date/{date}")]
    public async Task<ActionResult<IEnumerable<ShowtimeResponse>>> GetByDate(DateTime date)
    {
        var showtimes = await _showtimeService.GetShowtimesByDateAsync(date);
        return Ok(showtimes);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ShowtimeResponse>> GetById(int id)
    {
        var showtime = await _showtimeService.GetShowtimeByIdAsync(id);
        if (showtime == null)
            return NotFound(new { message = "Showtime not found" });
        return Ok(showtime);
    }

    [HttpGet("{id}/seats")]
    public async Task<ActionResult<SeatMapDto>> GetSeatMap(int id)
    {
        var seatMap = await _showtimeService.GetSeatMapAsync(id);
        if (seatMap == null)
            return NotFound(new { message = "Showtime not found" });
        return Ok(seatMap);
    }

    // [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ShowtimeResponse>> Create([FromBody] ShowtimeRequest dto)
    {
        try
        {
            var showtime = await _showtimeService.CreateShowtimeAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = showtime.ShowtimeID }, showtime);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await _showtimeService.DeleteShowtimeAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}