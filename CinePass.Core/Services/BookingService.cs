using System.Security.Cryptography;
using System.Text;
using CinePass.Core.Configurations;
using CinePass.Domain;
using CinePass.Domain.IRepository;
using CinePass.Domain.Models;
using CinePass.Shared.DTOs.Booking;
using CinePass.Shared.DTOs.BookingDetail;
using CinePass.Shared.DTOs.QRCode;
using Microsoft.EntityFrameworkCore;

namespace CinePass.Core.Services;

public class BookingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly AppDbContext _context;

    public BookingService(IUnitOfWork unitOfWork, AppDbContext context)
    {
        _unitOfWork = unitOfWork;
        _context = context;
    }

    public async Task<BookingResponse> CreateBookingAsync(BookingRequest dto)
    {
        // FIX: Sử dụng ExecutionStrategy thay vì BeginTransactionAsync trực tiếp
        var strategy = _context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Validate seats availability
                var areSeatsAvailable = await _unitOfWork.Seats.AreSeatAvailableAsync(dto.SeatIDs, dto.ShowtimeID);
                if (!areSeatsAvailable)
                    throw new InvalidOperationException("One or more seats are already booked");

                // Get showtime and calculate total
                var showtime = await _unitOfWork.Showtimes.GetByIdAsync(dto.ShowtimeID);
                if (showtime == null)
                    throw new InvalidOperationException("Showtime not found");

                var totalAmount = showtime.Price * dto.SeatIDs.Count;

                // Create booking
                var booking = new Booking
                {
                    UserID = dto.UserID,
                    ShowtimeID = dto.ShowtimeID,
                    TotalAmount = totalAmount,
                    PaymentMethod = Enum.Parse<PaymentMethod>(dto.PaymentMethod),
                    Status = BookingStatus.Pending
                };

                await _unitOfWork.Bookings.AddAsync(booking);
                await _unitOfWork.SaveChangesAsync();

                // Create booking details with QR tokens
                foreach (var seatId in dto.SeatIDs)
                {
                    var qrToken = GenerateQRToken(booking.BookingID, seatId);
                    var bookingDetail = new BookingDetail
                    {
                        BookingID = booking.BookingID,
                        SeatID = seatId,
                        QRToken = qrToken,
                        TokenExpires = showtime.StartTime.AddHours(2)
                    };
                    await _unitOfWork.BookingDetails.AddAsync(bookingDetail);
                }

                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                return await GetBookingByIdAsync(booking.BookingID);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }

    public async Task<BookingResponse> GetBookingByIdAsync(int id)
    {
        var booking = await _unitOfWork.Bookings.GetBookingWithDetailsAsync(id);
        return booking != null ? MapToDto(booking) : null;
    }

    public async Task<IEnumerable<BookingResponse>> GetUserBookingsAsync(int userId)
    {
        var bookings = await _unitOfWork.Bookings.GetBookingsByUserAsync(userId);
        return bookings.Select(MapToDto);
    }

    public async Task<bool> CancelBookingAsync(int bookingId)
    {
        var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId);
        if (booking == null || booking.Status == BookingStatus.Cancelled)
            return false;

        booking.Status = BookingStatus.Cancelled;
        await _unitOfWork.Bookings.UpdateAsync(booking);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ConfirmBookingAsync(int bookingId)
    {
        var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId);
        if (booking == null) return false;

        booking.Status = BookingStatus.CheckedIn;
        await _unitOfWork.Bookings.UpdateAsync(booking);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<QRCodeDto> GetQRCodeAsync(int bookingId)
    {
        var booking = await _unitOfWork.Bookings.GetBookingWithDetailsAsync(bookingId);
        if (booking == null) return null;

        return new QRCodeDto
        {
            QRToken = booking.BookingDetails.First().QRToken,
            BookingID = booking.BookingID,
            MovieTitle = booking.Showtime.Movie.Title,
            CinemaName = booking.Showtime.Screen.Cinema.Name,
            StartTime = booking.Showtime.StartTime,
            SeatNumbers = booking.BookingDetails.Select(bd => bd.Seat.SeatNumber).ToList(),
            TokenExpires = booking.BookingDetails.First().TokenExpires
        };
    }

    public async Task<BookingResponse> CheckInAsync(string qrToken)
    {
        var bookingDetail = await _unitOfWork.BookingDetails.GetByQRTokenAsync(qrToken);
        if (bookingDetail == null)
            throw new InvalidOperationException("Invalid QR code");

        if (bookingDetail.TokenExpires < DateTime.UtcNow)
            throw new InvalidOperationException("QR code has expired");

        if (bookingDetail.Booking.Status != BookingStatus.CheckedIn)
            throw new InvalidOperationException("Booking is not confirmed");

        // Mark as used (you can add a flag to BookingDetail if needed)
        return MapToDto(bookingDetail.Booking);
    }

    private string GenerateQRToken(int bookingId, int seatId)
    {
        var data = $"{bookingId}-{seatId}-{DateTime.UtcNow.Ticks}";
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Convert.ToBase64String(hash);
    }

    private BookingResponse MapToDto(Booking booking)
    {
        return new BookingResponse
        {
            BookingID = booking.BookingID,
            UserID = booking.UserID,
            UserName = booking.User?.FullName,
            ShowtimeID = booking.ShowtimeID,
            MovieTitle = booking.Showtime?.Movie?.Title,
            CinemaName = booking.Showtime?.Screen?.Cinema?.Name,
            StartTime = booking.Showtime.StartTime,
            BookingTime = booking.BookingTime,
            Status = booking.Status.ToString(),
            TotalAmount = booking.TotalAmount,
            PaymentMethod = booking.PaymentMethod.ToString(),
            BookingDetails = booking.BookingDetails?.Select(bd => new BookingDetailDto()
            {
                BookingDetailID = bd.BookingDetailID,
                SeatNumber = bd.Seat?.SeatNumber,
                SeatType = bd.Seat?.SeatType.ToString(),
                QRToken = bd.QRToken,
                TokenExpires = bd.TokenExpires
            }).ToList()
        };
    }
}