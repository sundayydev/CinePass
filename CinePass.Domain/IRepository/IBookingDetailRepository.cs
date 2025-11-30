using CinePass.Domain.Models;

namespace CinePass.Domain.IRepository;

public interface IBookingDetailRepository : IRepository<BookingDetail>
{
    Task<IEnumerable<BookingDetail>> GetByBookingIdAsync(int bookingId);
    Task<BookingDetail> GetByQRTokenAsync(string qrToken);
    Task<bool> IsSeatBookedForShowtimeAsync(int seatId, int showtimeId);
    Task<IEnumerable<int>> GetBookedSeatIdsForShowtimeAsync(int showtimeId);
}