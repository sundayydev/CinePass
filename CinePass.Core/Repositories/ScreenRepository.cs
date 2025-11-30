using CinePass.Core.Configurations;
using CinePass.Domain.IRepository;
using CinePass.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CinePass.Core.Repositories;

public class ScreenRepository : Repository<Screen>, IScreenRepository
{
    public ScreenRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Screen>> GetScreensByCinemaAsync(int cinemaId)
    {
        return await _dbSet
            .Where(s => s.CinemaID == cinemaId)
            .ToListAsync();
    }

    public async Task<Screen> GetScreenWithSeatsAsync(int screenId)
    {
        return await _dbSet
            .Include(s => s.Seats)
            .FirstOrDefaultAsync(s => s.ScreenID == screenId);
    }
}