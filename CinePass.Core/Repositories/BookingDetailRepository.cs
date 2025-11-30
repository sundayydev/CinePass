using CinePass.Core.Configurations;
using CinePass.Domain;
using CinePass.Domain.IRepository;
using CinePass.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CinePass.Core.Repositories;

public class BookingDetailRepository : Repository<BookingDetail>, IBookingDetailRepository
{
    public BookingDetailRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<BookingDetail>> GetByBookingIdAsync(int bookingId)
    {
        return await _dbSet
            .Include(bd => bd.Seat)
            .Where(bd => bd.BookingID == bookingId)
            .ToListAsync();
    }

    public async Task<BookingDetail> GetByQRTokenAsync(string qrToken)
    {
        return await _dbSet
            .Include(bd => bd.Booking)
            .ThenInclude(b => b.Showtime)
            .ThenInclude(s => s.Movie)
            .Include(bd => bd.Booking)
            .ThenInclude(b => b.Showtime)
            .ThenInclude(s => s.Screen)
            .ThenInclude(sc => sc.Cinema)
            .Include(bd => bd.Seat)
            .FirstOrDefaultAsync(bd => bd.QRToken == qrToken);
    }

    public async Task<bool> IsSeatBookedForShowtimeAsync(int seatId, int showtimeId)
    {
        return await _dbSet
            .AnyAsync(bd => bd.SeatID == seatId && 
                            bd.Booking.ShowtimeID == showtimeId &&
                            bd.Booking.Status != BookingStatus.Cancelled);
    }

    public async Task<IEnumerable<int>> GetBookedSeatIdsForShowtimeAsync(int showtimeId)
    {
        return await _dbSet
            .Where(bd => bd.Booking.ShowtimeID == showtimeId &&
                         bd.Booking.Status != BookingStatus.Cancelled)
            .Select(bd => bd.SeatID)
            .ToListAsync();
    }
}