using CinePass.Domain.IRepository;
using CinePass.Domain.Models;
using CinePass.Shared.DTOs.Showtime;

namespace CinePass.Core.Services;

public class ShowtimeService
{
    private readonly IShowtimeRepository _repo;

    public ShowtimeService(IShowtimeRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<ShowtimeResponse>> GetAllAsync()
        => (await _repo.GetAllAsync()).Select(ToResponse).ToList();

    public async Task<ShowtimeResponse?> GetByIdAsync(int id)
    {
        var item = await _repo.GetByIdAsync(id);
        return item == null ? null : ToResponse(item);
    }

    public async Task<ShowtimeResponse> CreateAsync(ShowtimeRequest request)
    {
        var model = new Showtime()
        {
            MovieID = request.MovieID,
            ScreenID = request.ScreenID,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            Price = request.Price
        };

        await _repo.AddAsync(model);
        return ToResponse(model);
    }

    public async Task<bool> UpdateAsync(int id, ShowtimeRequest request)
    {
        var model = await _repo.GetByIdAsync(id);
        if (model == null) return false;

        model.MovieID = request.MovieID;
        model.ScreenID = request.ScreenID;
        model.StartTime = request.StartTime;
        model.EndTime = request.EndTime;
        model.Price = request.Price;

        await _repo.UpdateAsync(model);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var model = await _repo.GetByIdAsync(id);
        if (model == null) return false;

        await _repo.DeleteAsync(model);
        return true;
    }

    private ShowtimeResponse ToResponse(Showtime s)
        => new ShowtimeResponse
        {
            ShowtimeID = s.ShowtimeID,
            MovieID = s.MovieID,
            ScreenID = s.ScreenID,
            StartTime = s.StartTime,
            EndTime = s.EndTime,
            Price = s.Price
        };
}