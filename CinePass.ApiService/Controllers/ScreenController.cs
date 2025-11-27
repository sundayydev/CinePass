using CinePass.Core.Services;
using CinePass.Shared.DTOs.Screen;
using Microsoft.AspNetCore.Mvc;

namespace CinePass.ApiService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ScreenController : ControllerBase
{
    private readonly ScreenService _service;
    
    public ScreenController(ScreenService service)
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
        var screen = await _service.GetByIdAsync(id);
        if (screen == null) return NotFound();
        return Ok(screen);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ScreenRequest request)
    {
        var result = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.ScreenID }, result);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ScreenRequest request)
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