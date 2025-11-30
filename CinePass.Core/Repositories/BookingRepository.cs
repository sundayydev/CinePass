using CinePass.Core.Configurations;
using CinePass.Domain;
using CinePass.Domain.IRepository;
using CinePass.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CinePass.Core.Repositories;

public class BookingRepository : Repository<Booking>, IBookingRepository
{
    public BookingRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Booking>> GetBookingsByUserAsync(int userId)
    {
        return await _dbSet
            .Include(b => b.Showtime)
                .ThenInclude(s => s.Movie)
            .Include(b => b.Showtime)
                .ThenInclude(s => s.Screen)
                    .ThenInclude(sc => sc.Cinema)
            .Include(b => b.BookingDetails)
                .ThenInclude(bd => bd.Seat)
            .Where(b => b.UserID == userId)
            .OrderByDescending(b => b.BookingTime)
            .ToListAsync();
    }

    public async Task<Booking> GetBookingWithDetailsAsync(int bookingId)
    {
        return await _dbSet
            .Include(b => b.User)
            .Include(b => b.Showtime)
                .ThenInclude(s => s.Movie)
            .Include(b => b.Showtime)
                .ThenInclude(s => s.Screen)
                    .ThenInclude(sc => sc.Cinema)
            .Include(b => b.BookingDetails)
                .ThenInclude(bd => bd.Seat)
            .Include(b => b.Payments)
            .FirstOrDefaultAsync(b => b.BookingID == bookingId);
    }

    public async Task<IEnumerable<Booking>> GetBookingsByShowtimeAsync(int showtimeId)
    {
        return await _dbSet
            .Include(b => b.User)
            .Include(b => b.BookingDetails)
                .ThenInclude(bd => bd.Seat)
            .Where(b => b.ShowtimeID == showtimeId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Booking>> GetBookingsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Include(b => b.Showtime)
                .ThenInclude(s => s.Movie)
            .Include(b => b.User)
            .Where(b => b.BookingTime >= startDate && b.BookingTime <= endDate)
            .OrderByDescending(b => b.BookingTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Booking>> GetPendingBookingsAsync()
    {
        return await _dbSet
            .Include(b => b.Showtime)
            .Where(b => b.Status == BookingStatus.Pending)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _dbSet.Where(b => b.Status == BookingStatus.CheckedIn);

        if (startDate.HasValue)
            query = query.Where(b => b.BookingTime >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(b => b.BookingTime <= endDate.Value);

        return await query.SumAsync(b => b.TotalAmount);
    }
}
