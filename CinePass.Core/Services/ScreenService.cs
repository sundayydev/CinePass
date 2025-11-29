using CinePass.Domain.IRepository;
using CinePass.Domain.Models;
using CinePass.Shared.DTOs.Screen;

namespace CinePass.Core.Services;

public class ScreenService
{
    private readonly IScreenRepository _repository;

    public ScreenService(IScreenRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ScreenResponse>> GetAllAsync()
    {
        var screens = await _repository.GetAllAsync();
        return screens.Select(s => new ScreenResponse
        {
            ScreenID = s.ScreenID,
            CinemaID = s.CinemaID,
            Name = s.Name,
            TotalSeats = s.TotalSeats
        }).ToList();
    }

    public async Task<ScreenResponse?> GetByIdAsync(int id)
    {
        var screen = await _repository.GetByIdAsync(id);
        if (screen == null) return null;
        return new ScreenResponse
        {
            ScreenID = screen.ScreenID,
            CinemaID = screen.CinemaID,
            Name = screen.Name,
            TotalSeats = screen.TotalSeats
        };
    }

    public async Task<ScreenResponse> CreateAsync(ScreenRequest request)
    {
        try
        {
            var screen = new Screen()
            {
                CinemaID = request.CinemaID,
                Name = request.Name,
                TotalSeats = request.TotalSeats
            };
            var result = await _repository.CreateAsync(screen);
            return new ScreenResponse
            {
                ScreenID = result.ScreenID,
                CinemaID = result.CinemaID,
                Name = result.Name,
                TotalSeats = result.TotalSeats
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(int id, ScreenRequest request)
    {
        var screen = await _repository.GetByIdAsync(id);
        if (screen == null) return false;
        
        screen.CinemaID = request.CinemaID;
        screen.Name = request.Name;
        screen.TotalSeats = request.TotalSeats;
        
        await _repository.UpdateAsync(screen);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var screen = await _repository.GetByIdAsync(id);
        if (screen == null) return false;
        
        await _repository.DeleteAsync(screen);
        return true;
    }
}