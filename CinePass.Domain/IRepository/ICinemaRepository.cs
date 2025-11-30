using CinePass.Domain.Models;

namespace CinePass.Domain.IRepository;

public interface ICinemaRepository : IRepository<Cinema>
{
    Task<Cinema> GetCinemaWithScreensAsync(int cinemaId);
    Task<IEnumerable<Cinema>> GetCinemasWithMovieAsync(int movieId);
}