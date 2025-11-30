using CinePass.Domain.IRepository;
using CinePass.Domain.Models;
using CinePass.Shared.DTOs.Seat;
using CinePass.Shared.DTOs.Showtime;

namespace CinePass.Core.Services;

public class ShowtimeService
{
    private readonly IUnitOfWork _unitOfWork;

    public ShowtimeService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ShowtimeResponse>> GetShowtimesByMovieAsync(int movieId)
    {
        var showtimes = await _unitOfWork.Showtimes.GetShowtimesByMovieAsync(movieId);
        var result = new List<ShowtimeResponse>();

        foreach (var showtime in showtimes)
        {
            var bookedSeats = await _unitOfWork.BookingDetails.GetBookedSeatIdsForShowtimeAsync(showtime.ShowtimeID);
            result.Add(MapToDto(showtime, showtime.Screen.TotalSeats - bookedSeats.Count()));
        }

        return result;
    }

    public async Task<IEnumerable<ShowtimeResponse>> GetShowtimesByDateAsync(DateTime date)
    {
        var showtimes = await _unitOfWork.Showtimes.GetShowtimesByDateAsync(date);
        var result = new List<ShowtimeResponse>();

        foreach (var showtime in showtimes)
        {
            var bookedSeats = await _unitOfWork.BookingDetails.GetBookedSeatIdsForShowtimeAsync(showtime.ShowtimeID);
            result.Add(MapToDto(showtime, showtime.Screen.TotalSeats - bookedSeats.Count()));
        }

        return result;
    }

    public async Task<ShowtimeResponse> GetShowtimeByIdAsync(int id)
    {
        var showtime = await _unitOfWork.Showtimes.GetShowtimeWithDetailsAsync(id);
        if (showtime == null) return null;

        var bookedSeats = await _unitOfWork.BookingDetails.GetBookedSeatIdsForShowtimeAsync(id);
        return MapToDto(showtime, showtime.Screen.TotalSeats - bookedSeats.Count());
    }

    public async Task<ShowtimeResponse> CreateShowtimeAsync(ShowtimeRequest dto)
    {
        // Check for overlapping showtimes
        var hasOverlap = await _unitOfWork.Showtimes.HasOverlappingShowtimeAsync(
            dto.ScreenID, dto.StartTime, dto.EndTime);

        if (hasOverlap)
            throw new InvalidOperationException("Showtime overlaps with existing showtime");

        var showtime = new Showtime
        {
            MovieID = dto.MovieID,
            ScreenID = dto.ScreenID,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            Price = dto.Price
        };

        await _unitOfWork.Showtimes.AddAsync(showtime);
        await _unitOfWork.SaveChangesAsync();

        var result = await _unitOfWork.Showtimes.GetShowtimeWithDetailsAsync(showtime.ShowtimeID);
        return MapToDto(result, result.Screen.TotalSeats);
    }

    public async Task DeleteShowtimeAsync(int id)
    {
        await _unitOfWork.Showtimes.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<SeatMapDto> GetSeatMapAsync(int showtimeId)
    {
        var showtime = await _unitOfWork.Showtimes.GetShowtimeWithDetailsAsync(showtimeId);
        if (showtime == null) return null;

        var seats = await _unitOfWork.Seats.GetSeatsByScreenAsync(showtime.ScreenID);
        var bookedSeatIds = await _unitOfWork.BookingDetails.GetBookedSeatIdsForShowtimeAsync(showtimeId);

        return new SeatMapDto
        {
            ScreenID = showtime.ScreenID,
            ScreenName = showtime.Screen.Name,
            ShowtimeID = showtimeId,
            Seats = seats.Select(s => new SeatDto
            {
                SeatID = s.SeatID,
                SeatNumber = s.SeatNumber,
                SeatType = s.SeatType.ToString(),
                IsBooked = bookedSeatIds.Contains(s.SeatID)
            }).ToList()
        };
    }

    private ShowtimeResponse MapToDto(Showtime showtime, int availableSeats)
    {
        return new ShowtimeResponse
        {
            ShowtimeID = showtime.ShowtimeID,
            MovieID = showtime.MovieID,
            MovieTitle = showtime.Movie?.Title,
            ScreenID = showtime.ScreenID,
            ScreenName = showtime.Screen?.Name,
            CinemaName = showtime.Screen?.Cinema?.Name,
            StartTime = showtime.StartTime,
            EndTime = showtime.EndTime,
            Price = showtime.Price,
            AvailableSeats = availableSeats
        };
    }
}