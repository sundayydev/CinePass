using CinePass.Core.Services;
using CinePass.Shared.DTOs.Movie;
using Microsoft.AspNetCore.Mvc;

namespace CinePass.ApiService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MovieController : ControllerBase
{
    private readonly MovieService _service;
    
    public MovieController(MovieService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());


    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var movie = await _service.GetByIdAsync(id);
        if (movie == null) return NotFound(new { message = "Movie not found" });
        return Ok(movie);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(MovieRequest request)
    {
        var result = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.MovieID }, result);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, MovieRequest request)
    {
        var ok = await _service.UpdateAsync(id, request);
        if (!ok) return NotFound(new { message = "Movie not found" });
        return NoContent();
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.DeleteAsync(id);
        if (!ok) return NotFound(new { message = "Movie not found" });
        return NoContent();
    }
}