using CinePass.Core.Configurations;
using CinePass.Domain.IRepository;
using CinePass.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CinePass.Core.Repositories;
 
public class CinemaRepository : Repository<Cinema>, ICinemaRepository
{
    public CinemaRepository(AppDbContext context) : base(context) { }

    public async Task<Cinema> GetCinemaWithScreensAsync(int cinemaId)
    {
        return await _dbSet
            .Include(c => c.Screens)
            .FirstOrDefaultAsync(c => c.CinemaID == cinemaId);
    }

    public async Task<IEnumerable<Cinema>> GetCinemasWithMovieAsync(int movieId)
    {
        return await _dbSet
            .Include(c => c.Screens)
            .Where(c => c.Screens.Any(s => s.Showtimes.Any(st => st.MovieID == movieId)))
            .ToListAsync();
    }
}