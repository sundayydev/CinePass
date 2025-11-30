using CinePass.Core.Services;
using CinePass.Shared.DTOs.Movie;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinePass.ApiService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MoviesController : ControllerBase
{
    private readonly MovieService _movieService;

    public MoviesController(MovieService movieService)
    {
        _movieService = movieService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MovieResponse>>> GetAll()
    {
        var movies = await _movieService.GetAllMoviesAsync();
        return Ok(movies);
    }

    [HttpGet("now-showing")]
    public async Task<ActionResult<IEnumerable<MovieResponse>>> GetNowShowing()
    {
        var movies = await _movieService.GetNowShowingAsync();
        return Ok(movies);
    }

    [HttpGet("coming-soon")]
    public async Task<ActionResult<IEnumerable<MovieResponse>>> GetComingSoon()
    {
        var movies = await _movieService.GetComingSoonAsync();
        return Ok(movies);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MovieResponse>> GetById(int id)
    {
        var movie = await _movieService.GetMovieByIdAsync(id);
        if (movie == null)
            return NotFound(new { message = "Movie not found" });
        return Ok(movie);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<MovieResponse>>> Search([FromQuery] string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return BadRequest(new { message = "Keyword is required" });

        var movies = await _movieService.SearchMoviesAsync(keyword);
        return Ok(movies);
    }

    // [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<MovieResponse>> Create([FromBody] MovieRequest dto)
    {
        try
        {
            var movie = await _movieService.CreateMovieAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = movie.MovieID }, movie);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<MovieResponse>> Update(int id, [FromBody] MovieRequest dto)
    {
        try
        {
            var movie = await _movieService.UpdateMovieAsync(id, dto);
            if (movie == null)
                return NotFound(new { message = "Movie not found" });
            return Ok(movie);
        }
        catch (Exception ex)
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
            await _movieService.DeleteMovieAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}