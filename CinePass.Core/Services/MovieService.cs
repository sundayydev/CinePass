using CinePass.Domain.IRepository;
using CinePass.Domain.Models;
using CinePass.Shared.DTOs.Movie;

namespace CinePass.Core.Services;

public class MovieService
{
    private readonly IMovieRepository _repo;

    public MovieService(IMovieRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<MovieResponse>> GetAllAsync()
    {
        var movies = await _repo.GetAllAsync();
        return movies.Select(movie => new MovieResponse
        {
            MovieID = movie.MovieID,
            Title = movie.Title,
            Description = movie.Description,
            DurationMinutes = movie.DurationMinutes,
            ReleaseDate = movie.ReleaseDate,
            Language = movie.Language,
            Genre = movie.Genre,
            PosterUrl = movie.PosterUrl,
        }).ToList();
    }

    public async Task<MovieResponse?> GetByIdAsync(int id)
    {
        var movie = await _repo.GetByIdAsync(id);
        if (movie == null) return null;
        return new MovieResponse
        {
            MovieID = movie.MovieID,
            Title = movie.Title,
            Description = movie.Description,
            DurationMinutes = movie.DurationMinutes,
            ReleaseDate = movie.ReleaseDate,
            Language = movie.Language,
            Genre = movie.Genre,
            PosterUrl = movie.PosterUrl,
        };
    }

    public async Task<MovieResponse> CreateAsync(MovieRequest request)
    {
        try
        {
            var movie = new Movie()
            {
                Title = request.Title,
                Description = request.Description,
                DurationMinutes = request.DurationMinutes,
                ReleaseDate = request.ReleaseDate,
                Language = request.Language,
                Genre = request.Genre,
                PosterUrl = request.PosterUrl,
            };
            var result = await _repo.CreateAsync(movie);
            return new MovieResponse
            {
                MovieID = result.MovieID,
                Title = result.Title,
                Description = result.Description,
                DurationMinutes = result.DurationMinutes,
                ReleaseDate = result.ReleaseDate,
                Language = result.Language,
                Genre = result.Genre,
                PosterUrl = result.PosterUrl,
                CreatedAt = result.CreatedAt
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(int id, MovieRequest request)
    {
        var movie = await _repo.GetByIdAsync(id);
        if (movie == null) return false;
        
        movie.Title = request.Title;
        movie.Description = request.Description;
        movie.DurationMinutes = request.DurationMinutes;
        movie.ReleaseDate = request.ReleaseDate;
        movie.Language = request.Language;
        movie.Genre = request.Genre;
        movie.PosterUrl = request.PosterUrl;

        await _repo.UpdateAsync(movie);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var  movie = await _repo.GetByIdAsync(id);
        if (movie == null) return false;
        await _repo.DeleteAsync(id);
        return true;
    }
}