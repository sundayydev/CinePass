using CinePass.Domain.Models;

namespace CinePass.Domain.IRepository;

public interface ICinemaRepository
{
    Task<List<Cinema>> GetAllAsync();
    Task<Cinema?> GetByIdAsync(int id);
    Task<Cinema> CreateAsync(Cinema cinema);
    Task UpdateAsync(Cinema cinema);
    Task DeleteAsync(Cinema cinema);
}