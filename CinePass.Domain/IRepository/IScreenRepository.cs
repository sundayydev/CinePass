using CinePass.Domain.Models;

namespace CinePass.Domain.IRepository;

public interface IScreenRepository : IRepository<Screen>
{
    Task<IEnumerable<Screen>> GetScreensByCinemaAsync(int cinemaId);
    Task<Screen> GetScreenWithSeatsAsync(int screenId);
}