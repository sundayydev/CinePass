using System.Linq.Expressions;
using CinePass.Domain.Models;

namespace CinePass.Domain.IRepository;

public interface IMovieRepository : IRepository<Movie>
{
    Task<IEnumerable<Movie>> GetNowShowingAsync();
    Task<IEnumerable<Movie>> GetComingSoonAsync();
    Task<IEnumerable<Movie>> SearchMoviesAsync(string keyword);
    Task<IEnumerable<Movie>> GetMoviesByGenreAsync(string genre);
}