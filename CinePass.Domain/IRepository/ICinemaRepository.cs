using CinePass.Domain.Models;

namespace CinePass.Domain.Repositories;

public interface ICinemaRepository
{
    Task<List<Cinema>> GetAllAsync();
    Task<Cinema?> GetByIdAsync(int id);
    Task<Cinema> CreateAsync(Cinema cinema);
    Task UpdateAsync(Cinema cinema);
    Task DeleteAsync(Cinema cinema);
}