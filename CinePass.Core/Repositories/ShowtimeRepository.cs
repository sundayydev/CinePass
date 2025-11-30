using CinePass.Core.Configurations;
using CinePass.Domain.IRepository;
using CinePass.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CinePass.Core.Repositories;

public class ShowtimeRepository : Repository<Showtime>, IShowtimeRepository
{
    public ShowtimeRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Showtime>> GetShowtimesByMovieAsync(int movieId)
    {
        return await _dbSet
            .Include(s => s.Screen)
                .ThenInclude(sc => sc.Cinema)
            .Where(s => s.MovieID == movieId && s.StartTime >= DateTime.UtcNow)
            .OrderBy(s => s.StartTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Showtime>> GetShowtimesByCinemaAsync(int cinemaId)
    {
        return await _dbSet
            .Include(s => s.Movie)
            .Include(s => s.Screen)
            .Where(s => s.Screen.CinemaID == cinemaId && s.StartTime >= DateTime.UtcNow)
            .OrderBy(s => s.StartTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Showtime>> GetShowtimesByDateAsync(DateTime date)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);

        return await _dbSet
            .Include(s => s.Movie)
            .Include(s => s.Screen)
                .ThenInclude(sc => sc.Cinema)
            .Where(s => s.StartTime >= startOfDay && s.StartTime < endOfDay)
            .OrderBy(s => s.StartTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Showtime>> GetShowtimesByMovieAndDateAsync(int movieId, DateTime date)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);

        return await _dbSet
            .Include(s => s.Screen)
                .ThenInclude(sc => sc.Cinema)
            .Where(s => s.MovieID == movieId && 
                   s.StartTime >= startOfDay && 
                   s.StartTime < endOfDay)
            .OrderBy(s => s.StartTime)
            .ToListAsync();
    }

    public async Task<Showtime> GetShowtimeWithDetailsAsync(int showtimeId)
    {
        return await _dbSet
            .Include(s => s.Movie)
            .Include(s => s.Screen)
                .ThenInclude(sc => sc.Cinema)
            .FirstOrDefaultAsync(s => s.ShowtimeID == showtimeId);
    }

    public async Task<bool> HasOverlappingShowtimeAsync(int screenId, DateTime startTime, DateTime endTime, int? excludeShowtimeId = null)
    {
        var query = _dbSet.Where(s => 
            s.ScreenID == screenId &&
            ((s.StartTime < endTime && s.EndTime > startTime)));

        if (excludeShowtimeId.HasValue)
        {
            query = query.Where(s => s.ShowtimeID != excludeShowtimeId.Value);
        }

        return await query.AnyAsync();
    }
}