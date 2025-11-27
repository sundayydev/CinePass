using CinePass.Core.Configurations;
using CinePass.Domain.IRepository;
using CinePass.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CinePass.Core.Repositories;

public class SeatRepository :  ISeatRepository
{
    private readonly AppDbContext _context;
    
    public SeatRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<Seat>> GetAllAsync() => await _context.Seats.Include(s => s.Screen).ToListAsync();
    
    public async Task<List<Seat>> GetByScreenIdAsync(int screenId) => await _context.Seats.Where(s => s.ScreenID == screenId).ToListAsync();

    public async Task<Seat?> GetByIdAsync(int id) => await _context.Seats.FindAsync(id);
    
    public async Task<Seat> CreateAsync(Seat seat)
    {
        _context.Seats.Add(seat);
        await _context.SaveChangesAsync();
        return seat;
    }
    
    public async Task UpdateAsync(Seat seat)
    {
        _context.Seats.Update(seat);
        await _context.SaveChangesAsync();
    }
    
    public async Task DeleteAsync(Seat seat)
    {
        _context.Seats.Remove(seat);
        await _context.SaveChangesAsync();
    }
}