using CinePass.Domain;
using CinePass.Domain.IRepository;
using CinePass.Domain.Models;
using CinePass.Shared.DTOs.Screen;

namespace CinePass.Core.Services;

public class ScreenService
{
    private readonly IUnitOfWork _unitOfWork;

    public ScreenService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ScreenDto>> GetAllScreensAsync()
    {
        var screens = await _unitOfWork.Screens.GetAllAsync();
        return screens.Select(MapToDto);
    }

    public async Task<ScreenDto> GetScreenByIdAsync(int id)
    {
        var screen = await _unitOfWork.Screens.GetByIdAsync(id);
        return screen != null ? MapToDto(screen) : null;
    }

    public async Task<IEnumerable<ScreenDto>> GetScreensByCinemaAsync(int cinemaId)
    {
        var screens = await _unitOfWork.Screens.GetScreensByCinemaAsync(cinemaId);
        return screens.Select(MapToDto);
    }

    public async Task<ScreenWithSeatsDto> GetScreenWithSeatsAsync(int id)
    {
        var screen = await _unitOfWork.Screens.GetScreenWithSeatsAsync(id);
        if (screen == null) return null;

        return new ScreenWithSeatsDto
        {
            ScreenID = screen.ScreenID,
            CinemaID = screen.CinemaID,
            CinemaName = screen.Cinema?.Name,
            Name = screen.Name,
            TotalSeats = screen.TotalSeats,
            Seats = screen.Seats?.Select(s => new SeatDetailDto
            {
                SeatID = s.SeatID,
                SeatNumber = s.SeatNumber,
                SeatType = s.SeatType.ToString()
            }).OrderBy(s => s.SeatNumber).ToList()
        };
    }

    public async Task<ScreenDto> CreateScreenAsync(ScreenDto dto)
    {
        // Kiểm tra cinema có tồn tại không
        var cinemaExists = await _unitOfWork.Cinemas.ExistsAsync(dto.CinemaID);
        if (!cinemaExists)
            throw new InvalidOperationException("Cinema not found");

        var screen = new Screen
        {
            CinemaID = dto.CinemaID,
            Name = dto.Name,
            TotalSeats = dto.TotalSeats
        };

        await _unitOfWork.Screens.AddAsync(screen);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(screen);
    }

    public async Task<ScreenDto> UpdateScreenAsync(int id, ScreenDto dto)
    {
        var screen = await _unitOfWork.Screens.GetByIdAsync(id);
        if (screen == null) return null;

        if (!string.IsNullOrWhiteSpace(dto.Name))
            screen.Name = dto.Name;

        if (dto.TotalSeats > 0)
            screen.TotalSeats = dto.TotalSeats;

        await _unitOfWork.Screens.UpdateAsync(screen);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(screen);
    }

    public async Task DeleteScreenAsync(int id)
    {
        // Kiểm tra xem screen có showtimes không
        var showtimes = await _unitOfWork.Showtimes.FindAsync(s => s.ScreenID == id);
        if (showtimes.Any())
        {
            throw new InvalidOperationException("Cannot delete screen with existing showtimes");
        }

        // Xóa tất cả seats của screen trước
        var seats = await _unitOfWork.Seats.GetSeatsByScreenAsync(id);
        foreach (var seat in seats)
        {
            await _unitOfWork.Seats.DeleteAsync(seat.SeatID);
        }

        await _unitOfWork.Screens.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<bool> InitializeSeatsForScreenAsync(int screenId, int rows, int seatsPerRow)
    {
        var screen = await _unitOfWork.Screens.GetByIdAsync(screenId);
        if (screen == null)
            throw new InvalidOperationException("Screen not found");

        // Kiểm tra xem đã có seats chưa
        var existingSeats = await _unitOfWork.Seats.GetSeatsByScreenAsync(screenId);
        if (existingSeats.Any())
            throw new InvalidOperationException("Screen already has seats initialized");

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var seatsList = new List<Seat>();
            var rowLabels = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            for (int row = 0; row < rows; row++)
            {
                for (int col = 1; col <= seatsPerRow; col++)
                {
                    var seatNumber = $"{rowLabels[row]}{col}";
                    
                    // Xác định loại ghế
                    SeatType seatType = SeatType.Regular;
                    
                    // VIP seats: 2 hàng cuối
                    if (row >= rows - 2)
                        seatType = SeatType.VIP;
                    
                    // // Couple seats: giữa 2 hàng cuối, ghế đôi (2 ghế liền nhau)
                    // if (row >= rows - 2 && col % 2 == 0 && col <= seatsPerRow - 2)
                    //     seatType = SeatType.Couple;

                    var seat = new Seat
                    {
                        ScreenID = screenId,
                        SeatNumber = seatNumber,
                        SeatType = seatType
                    };

                    await _unitOfWork.Seats.AddAsync(seat);
                }
            }

            // Update total seats
            screen.TotalSeats = rows * seatsPerRow;
            await _unitOfWork.Screens.UpdateAsync(screen);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return true;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    private ScreenDto MapToDto(Screen screen)
    {
        return new ScreenDto
        {
            CinemaID = screen.ScreenID,
            Name = screen.Name,
            TotalSeats = screen.TotalSeats
        };
    }
}