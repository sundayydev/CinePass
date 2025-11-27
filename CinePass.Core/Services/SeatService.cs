using CinePass.Core.Configurations;
using CinePass.Domain.IRepository;
using CinePass.Domain.Models;
using CinePass.Shared.DTOs.Seat;
using Microsoft.EntityFrameworkCore;

namespace CinePass.Core.Services;

public class SeatService
{
    private readonly ISeatRepository _repository;
    private readonly AppDbContext _context; // for validating Screen existence

    public SeatService(ISeatRepository repository, AppDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    public async Task<List<SeatResponse>> GetAllAsync()
    {
        var seats = await _repository.GetAllAsync();
        return seats.Select(s => new SeatResponse
        {
            SeatID = s.SeatID,
            ScreenID = s.ScreenID,
            SeatNumber = s.SeatNumber,
            SeatType = s.SeatType
        }).ToList();
    }

    public async Task<List<SeatResponse>> GetByScreenIdAsync(int screenId)
    {
        var seats = await _repository.GetByScreenIdAsync(screenId);
        return seats.Select(s => new SeatResponse
        {
            SeatID = s.SeatID,
            ScreenID = s.ScreenID,
            SeatNumber = s.SeatNumber,
            SeatType = s.SeatType
        }).ToList();
    }

    public async Task<SeatResponse?> GetByIdAsync(int id)
    {
        var s = await _repository.GetByIdAsync(id);
        if (s == null) return null;
        return new SeatResponse
        {
            SeatID = s.SeatID,
            ScreenID = s.ScreenID,
            SeatNumber = s.SeatNumber,
            SeatType = s.SeatType
        };
    }

    public async Task<SeatResponse> CreateAsync(SeatRequest request)
    {
        try
        {
            if (!await _context.Screens.AnyAsync(s => s.ScreenID == request.ScreenID))
                throw new ArgumentException("Screen không tồn tại");

            if (await _context.Seats.AnyAsync(s => s.ScreenID == request.ScreenID
                                                   && s.SeatNumber == request.SeatNumber))
                throw new ArgumentException("Ghế đã tồn tại trong phòng này");

            var seat = new Seat
            {
                ScreenID = request.ScreenID,
                SeatNumber = request.SeatNumber,
                SeatType = request.SeatType
            };

            var result = await _repository.CreateAsync(seat);

            return new SeatResponse
            {
                SeatID = result.SeatID,
                ScreenID = result.ScreenID,
                SeatNumber = result.SeatNumber,
                SeatType = result.SeatType
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(int id, SeatRequest request)
    {
        var seat = await _repository.GetByIdAsync(id);
        if (seat == null) return false;

        // validate screen
        var screenExists = await _context.Screens.AnyAsync(x => x.ScreenID == request.ScreenID);
        if (!screenExists)
            throw new InvalidOperationException("Screen does not exist");

        // check duplicate seat number on other seats
        var duplicate = await _context.Seats.AnyAsync(x =>
            x.ScreenID == request.ScreenID && x.SeatNumber == request.SeatNumber && x.SeatID != id);
        if (duplicate)
            throw new InvalidOperationException("SeatNumber already exists in this screen");

        seat.ScreenID = request.ScreenID;
        seat.SeatNumber = request.SeatNumber;
        seat.SeatType = request.SeatType;

        await _repository.UpdateAsync(seat);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var seat = await _repository.GetByIdAsync(id);
        if (seat == null) return false;

        // Optional: check if seat has booking details -> prevent deletion
        var hasBooking = await _context.Set<BookingDetail>().AnyAsync(b => b.SeatID == id);
        if (hasBooking)
            throw new InvalidOperationException("Cannot delete seat which has booking history");

        await _repository.DeleteAsync(seat);
        return true;
    }
}