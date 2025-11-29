using CinePass.Domain.Models;

namespace CinePass.Domain.IRepository;

public interface IShowtimeRepository
{
    Task<List<Showtime>> GetAllAsync();
    Task<Showtime?> GetByIdAsync(int id);
    Task AddAsync(Showtime showtime);
    Task UpdateAsync(Showtime showtime);
    Task DeleteAsync(Showtime showtime);
}