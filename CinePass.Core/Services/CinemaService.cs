using CinePass.Domain.Models;
using CinePass.Domain.Repositories;
using CinePass.Shared.DTOs.Cinema;

namespace CinePass.Core.Services;

public class CinemaService
{
    private readonly ICinemaRepository _repository;
    
    public CinemaService(ICinemaRepository repository)
    {
        _repository = repository;
    }


    public async Task<List<CinemaResponse>> GetAllAsync()
    {
        var cinemas = await _repository.GetAllAsync();
        return cinemas.Select(c => new CinemaResponse
        {
            CinemaID = c.CinemaID,
            Name = c.Name,
            Address = c.Address,
            CreatedAt = c.CreatedAt
        }).ToList();
    }


    public async Task<CinemaResponse?> GetByIdAsync(int id)
    {
        var cinema = await _repository.GetByIdAsync(id);
        if (cinema == null) return null;


        return new CinemaResponse
        {
            CinemaID = cinema.CinemaID,
            Name = cinema.Name,
            Address = cinema.Address,
            CreatedAt = cinema.CreatedAt
        };
    }


    public async Task<CinemaResponse> CreateAsync(CinemaRequest request)
    {
        try
        {
            var cinema = new Cinema
            {
                Name = request.Name,
                Address = request.Address,
            };

            var result = await _repository.CreateAsync(cinema);

            return new CinemaResponse
            {
                CinemaID = result.CinemaID,
                Name = result.Name,
                Address = result.Address,
                CreatedAt = result.CreatedAt
            };
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }



    public async Task<bool> UpdateAsync(int id, CinemaRequest request)
    {
        var cinema = await _repository.GetByIdAsync(id);
        if (cinema == null) return false;


        cinema.Name = request.Name;
        cinema.Address = request.Address;


        await _repository.UpdateAsync(cinema);
        return true;
    }
    public async Task<bool> DeleteAsync(int id)
    {
        var cinema = await _repository.GetByIdAsync(id);
        if (cinema == null) return false;


        await _repository.DeleteAsync(cinema);
        return true;
    }
}