using CinePass.Domain.Models;

namespace CinePass.Domain.IRepository;

public interface IScreenRepository
{
    Task<List<Screen>> GetAllAsync();
    Task<Screen?> GetByIdAsync(int id);
    Task<Screen> CreateAsync(Screen screen);
    Task UpdateAsync(Screen screen);
    Task DeleteAsync(Screen screen);
    Task<bool> ExistsCinemaIdAsync(int id);
}