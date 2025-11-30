using CinePass.Core.Configurations;
using CinePass.Domain.IRepository;
using CinePass.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CinePass.Core.Repositories;

public class ShowtimeRepository : IShowtimeRepository
{
    private readonly AppDbContext _context;

    public ShowtimeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Showtime>> GetAllAsync()
        => await _context.Showtimes.Include(s => s.Movie).Include(s => s.Screen).ToListAsync();

    public async Task<Showtime?> GetByIdAsync(int id)
        => await _context.Showtimes.FirstOrDefaultAsync(s => s.ShowtimeID == id);

    public async Task AddAsync(Showtime showtime)
    {
        _context.Showtimes.Add(showtime);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Showtime showtime)
    {
        _context.Showtimes.Update(showtime);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Showtime showtime)
    {
        _context.Showtimes.Remove(showtime);
        await _context.SaveChangesAsync();
    }
}