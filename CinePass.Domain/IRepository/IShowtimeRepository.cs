using System.Linq.Expressions;
using CinePass.Domain.Models;

namespace CinePass.Domain.IRepository;

public interface IShowtimeRepository : IRepository<Showtime>
{
    Task<IEnumerable<Showtime>> GetShowtimesByMovieAsync(int movieId);
    Task<IEnumerable<Showtime>> GetShowtimesByCinemaAsync(int cinemaId);
    Task<IEnumerable<Showtime>> GetShowtimesByDateAsync(DateTime date);
    Task<IEnumerable<Showtime>> GetShowtimesByMovieAndDateAsync(int movieId, DateTime date);
    Task<Showtime> GetShowtimeWithDetailsAsync(int showtimeId);
    Task<bool> HasOverlappingShowtimeAsync(int screenId, DateTime startTime, DateTime endTime, int? excludeShowtimeId = null);
}