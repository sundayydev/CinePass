using CinePass.Domain.Models;

namespace CinePass.Domain.IRepository;

public interface ISeatRepository
{
    Task<List<Seat>> GetAllAsync();
    Task<List<Seat>> GetByScreenIdAsync(int screenId);
    Task<Seat?> GetByIdAsync(int id);
    Task<Seat> CreateAsync(Seat seat);
    Task UpdateAsync(Seat seat);
    Task DeleteAsync(Seat seat);
}