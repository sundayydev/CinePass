using CinePass.Domain.Models;

namespace CinePass.Domain.IRepository;

public interface ISeatRepository : IRepository<Seat>
{
    Task<IEnumerable<Seat>> GetSeatsByScreenAsync(int screenId);
    Task<IEnumerable<Seat>> GetAvailableSeatsForShowtimeAsync(int showtimeId);
    Task<bool> AreSeatAvailableAsync(List<int> seatIds, int showtimeId);
}