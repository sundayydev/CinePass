using CinePass.Core.Configurations;
using CinePass.Domain;
using CinePass.Domain.IRepository;
using CinePass.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CinePass.Core.Repositories;

public class SeatRepository : Repository<Seat>, ISeatRepository
{
    public SeatRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Seat>> GetSeatsByScreenAsync(int screenId)
    {
        return await _dbSet
            .Where(s => s.ScreenID == screenId)
            .OrderBy(s => s.SeatNumber)
            .ToListAsync();
    }

    public async Task<IEnumerable<Seat>> GetAvailableSeatsForShowtimeAsync(int showtimeId)
    {
        var bookedSeatIds = await _context.Set<BookingDetail>()
            .Where(bd => bd.Booking.ShowtimeID == showtimeId &&
                         bd.Booking.Status != BookingStatus.Cancelled)
            .Select(bd => bd.SeatID)
            .ToListAsync();

        var showtime = await _context.Set<Showtime>().FindAsync(showtimeId);
        
        return await _dbSet
            .Where(s => s.ScreenID == showtime.ScreenID && 
                        !bookedSeatIds.Contains(s.SeatID))
            .ToListAsync();
    }

    public async Task<bool> AreSeatAvailableAsync(List<int> seatIds, int showtimeId)
    {
        var bookedSeatIds = await _context.Set<BookingDetail>()
            .Where(bd => bd.Booking.ShowtimeID == showtimeId &&
                         bd.Booking.Status != BookingStatus.Cancelled &&
                         seatIds.Contains(bd.SeatID))
            .Select(bd => bd.SeatID)
            .ToListAsync();

        return !bookedSeatIds.Any();
    }
}