using CinePass.Core.Configurations;
using CinePass.Domain.IRepository;
using CinePass.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CinePass.Core.Repositories;

public class MovieRepository : Repository<Movie>, IMovieRepository
{
    public MovieRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Movie>> GetNowShowingAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.Set<Movie>()
            .Where(m => m.ReleaseDate <= now && 
                        m.Showtimes.Any(s => s.StartTime >= now))
            .OrderByDescending(m => m.ReleaseDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Movie>> GetComingSoonAsync()
    {
        var now = DateTime.UtcNow;
        return await _dbSet
            .Where(m => m.ReleaseDate > now)
            .OrderBy(m => m.ReleaseDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Movie>> SearchMoviesAsync(string keyword)
    {
        return await _dbSet
            .Where(m => m.Title.Contains(keyword) || m.Description.Contains(keyword))
            .ToListAsync();
    }

    public async Task<IEnumerable<Movie>> GetMoviesByGenreAsync(string genre)
    {
        return await _dbSet
            .Where(m => m.Genre == genre)
            .ToListAsync();
    }
}