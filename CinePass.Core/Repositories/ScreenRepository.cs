using CinePass.Core.Configurations;
using CinePass.Domain.IRepository;
using CinePass.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CinePass.Core.Repositories;

public class ScreenRepository : IScreenRepository
{
    private readonly AppDbContext _context;
    
    public ScreenRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<Screen>> GetAllAsync()
    {
        return await _context.Screens.ToListAsync();
    }

    public async Task<Screen?> GetByIdAsync(int id)
    {
        return await _context.Screens.FindAsync(id);
    }

    public async Task<Screen> CreateAsync(Screen screen)
    {
        _context.Screens.Add(screen);
        await _context.SaveChangesAsync();
        return screen;
    }

    public async Task UpdateAsync(Screen screen)
    {
        _context.Screens.Update(screen);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Screen screen)
    {
        _context.Screens.Remove(screen);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsCinemaIdAsync(int id)
    {
        return await _context.Cinemas.AnyAsync(u => u.CinemaID == id);
    }
}