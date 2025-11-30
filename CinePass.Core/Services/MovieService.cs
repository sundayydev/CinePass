using CinePass.Domain.IRepository;
using CinePass.Domain.Models;
using CinePass.Shared.DTOs.Movie;

namespace CinePass.Core.Services;

public class MovieService
{
    private readonly IUnitOfWork _unitOfWork;

    public MovieService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<MovieResponse>> GetAllMoviesAsync()
    {
        var movies = await _unitOfWork.Movies.GetAllAsync();
        return movies.Select(MapToDto);
    }

    public async Task<IEnumerable<MovieResponse>> GetNowShowingAsync()
    {
        var movies = await _unitOfWork.Movies.GetNowShowingAsync();
        return movies.Select(MapToDto);
    }

    public async Task<IEnumerable<MovieResponse>> GetComingSoonAsync()
    {
        var movies = await _unitOfWork.Movies.GetComingSoonAsync();
        return movies.Select(MapToDto);
    }

    public async Task<MovieResponse> GetMovieByIdAsync(int id)
    {
        var movie = await _unitOfWork.Movies.GetByIdAsync(id);
        return movie != null ? MapToDto(movie) : null;
    }

    public async Task<MovieResponse> CreateMovieAsync(MovieRequest dto)
    {
        var movie = new Movie
        {
            Title = dto.Title,
            Description = dto.Description,
            DurationMinutes = dto.DurationMinutes ?? 0,
            ReleaseDate = dto.ReleaseDate,
            Language = dto.Language,
            Genre = dto.Genre,
            PosterUrl = dto.PosterUrl
        };

        await _unitOfWork.Movies.AddAsync(movie);
        await _unitOfWork.SaveChangesAsync();
        return MapToDto(movie);
    }

    public async Task<MovieResponse> UpdateMovieAsync(int id, MovieRequest dto)
    {
        var movie = await _unitOfWork.Movies.GetByIdAsync(id);
        if (movie == null) return null;

        if (!string.IsNullOrEmpty(dto.Title)) movie.Title = dto.Title;
        if (!string.IsNullOrEmpty(dto.Description)) movie.Description = dto.Description;
        if (dto.DurationMinutes.HasValue) movie.DurationMinutes = dto.DurationMinutes.Value;
        if (dto.ReleaseDate.HasValue) movie.ReleaseDate = dto.ReleaseDate;
        if (!string.IsNullOrEmpty(dto.Language)) movie.Language = dto.Language;
        if (!string.IsNullOrEmpty(dto.Genre)) movie.Genre = dto.Genre;
        if (!string.IsNullOrEmpty(dto.PosterUrl)) movie.PosterUrl = dto.PosterUrl;

        await _unitOfWork.Movies.UpdateAsync(movie);
        await _unitOfWork.SaveChangesAsync();
        return MapToDto(movie);
    }

    public async Task DeleteMovieAsync(int id)
    {
        await _unitOfWork.Movies.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<MovieResponse>> SearchMoviesAsync(string keyword)
    {
        var movies = await _unitOfWork.Movies.SearchMoviesAsync(keyword);
        return movies.Select(MapToDto);
    }

    private MovieResponse MapToDto(Movie movie)
    {
        return new MovieResponse
        {
            MovieID = movie.MovieID,
            Title = movie.Title,
            Description = movie.Description,
            DurationMinutes = movie.DurationMinutes,
            ReleaseDate = movie.ReleaseDate,
            Language = movie.Language,
            Genre = movie.Genre,
            PosterUrl = movie.PosterUrl
        };
    }
}