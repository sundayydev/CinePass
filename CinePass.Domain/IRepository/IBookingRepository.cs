using System.Linq.Expressions;
using CinePass.Domain.Models;

namespace CinePass.Domain.IRepository;

public interface IBookingRepository : IRepository<Booking>
{
    Task<IEnumerable<Booking>> GetBookingsByUserAsync(int userId);
    Task<Booking> GetBookingWithDetailsAsync(int bookingId);
    Task<IEnumerable<Booking>> GetBookingsByShowtimeAsync(int showtimeId);
    Task<IEnumerable<Booking>> GetBookingsByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Booking>> GetPendingBookingsAsync();
    Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null);
}