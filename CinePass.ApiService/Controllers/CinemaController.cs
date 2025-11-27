using CinePass.Core.Services;
using CinePass.Shared.DTOs.Cinema;
using Microsoft.AspNetCore.Mvc;

namespace CinePass.ApiService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CinemaController : ControllerBase
{
    private readonly CinemaService _service;

    public CinemaController(CinemaService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var cinema = await _service.GetByIdAsync(id);
        if (cinema == null) return NotFound();
        return Ok(cinema);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CinemaRequest request)
    {
        var result = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.CinemaID }, result);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CinemaRequest request)
    {
        var updated = await _service.UpdateAsync(id, request);
        if (!updated) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}