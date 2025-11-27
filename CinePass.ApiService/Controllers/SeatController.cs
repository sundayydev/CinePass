using CinePass.Core.Services;
using CinePass.Shared.DTOs.Seat;
using Microsoft.AspNetCore.Mvc;

namespace CinePass.ApiService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SeatController : ControllerBase
{
    private readonly SeatService _service;
    
    public SeatController(SeatService service)
    {
        _service = service;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllAsync() => Ok(await _service.GetAllAsync());
    
    [HttpGet("/by-screen/{screenId}")]
    public async Task<IActionResult> GetByScreen(int screenId) => Ok(await _service.GetByScreenIdAsync(screenId));
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var s = await _service.GetByIdAsync(id);
        if (s == null) return NotFound();
        return Ok(s);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SeatRequest request)
    {
        try
        {
            var result = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.SeatID }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] SeatRequest request)
    {
        try
        {
            var updated = await _service.UpdateAsync(id, request);
            if (!updated) return NotFound();
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}