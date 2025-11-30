using CinePass.Domain.IRepository;
using CinePass.Domain.Models;
using CinePass.Shared.DTOs.Cinema;
using CinePass.Shared.DTOs.Screen;

namespace CinePass.Core.Services;

public class CinemaService
{
    private readonly IUnitOfWork _unitOfWork;

    public CinemaService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<CinemaResponse>> GetAllCinemasAsync()
    {
        var cinemas = await _unitOfWork.Cinemas.GetAllAsync();
        return cinemas.Select(MapToDto);
    }

    public async Task<CinemaResponse> GetCinemaByIdAsync(int id)
    {
        var cinema = await _unitOfWork.Cinemas.GetByIdAsync(id);
        return cinema != null ? MapToDto(cinema) : null;
    }

    public async Task<CinemaResponse> GetCinemaWithScreensAsync(int id)
    {
        var cinema = await _unitOfWork.Cinemas.GetCinemaWithScreensAsync(id);
        if (cinema == null) return null;

        return new CinemaResponse
        {
            CinemaID = cinema.CinemaID,
            Name = cinema.Name,
            Address = cinema.Address,
            Screens = cinema.Screens?.Select(s => new ScreenDto()
            {
                CinemaID = s.ScreenID,
                Name = s.Name,
                TotalSeats = s.TotalSeats
            }).ToList()
        };
    }

    public async Task<IEnumerable<CinemaResponse>> GetCinemasShowingMovieAsync(int movieId)
    {
        var cinemas = await _unitOfWork.Cinemas.GetCinemasWithMovieAsync(movieId);
        return cinemas.Select(c => new CinemaResponse
        {
            CinemaID = c.CinemaID,
            Name = c.Name,
            Address = c.Address,
            Screens = c.Screens?.Select(s => new ScreenDto
            {
                CinemaID = s.ScreenID,
                Name = s.Name,
                TotalSeats = s.TotalSeats
            }).ToList()
        });
    }

    public async Task<CinemaResponse> CreateCinemaAsync(CinemaRequest dto)
    {
        var cinema = new Cinema
        {
            Name = dto.Name,
            Address = dto.Address
        };

        await _unitOfWork.Cinemas.AddAsync(cinema);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(cinema);
    }

    public async Task<CinemaResponse> UpdateCinemaAsync(int id, CinemaRequest dto)
    {
        var cinema = await _unitOfWork.Cinemas.GetByIdAsync(id);
        if (cinema == null) return null;

        if (!string.IsNullOrWhiteSpace(dto.Name))
            cinema.Name = dto.Name;

        if (!string.IsNullOrWhiteSpace(dto.Address))
            cinema.Address = dto.Address;

        await _unitOfWork.Cinemas.UpdateAsync(cinema);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(cinema);
    }

    public async Task DeleteCinemaAsync(int id)
    {
        // Kiểm tra xem cinema có screens không
        var cinema = await _unitOfWork.Cinemas.GetCinemaWithScreensAsync(id);
        if (cinema != null && cinema.Screens != null && cinema.Screens.Any())
        {
            throw new InvalidOperationException("Cannot delete cinema with existing screens. Delete screens first.");
        }

        await _unitOfWork.Cinemas.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }

    private CinemaResponse MapToDto(Cinema cinema)
    {
        return new CinemaResponse()
        {
            CinemaID = cinema.CinemaID,
            Name = cinema.Name,
            Address = cinema.Address
        };
    }
}