using CinePass.Core.Configurations;
using CinePass.Domain.IRepository;
using CinePass.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CinePass.Core.Repositories;
 
public class CinemaRepository : ICinemaRepository
{
    private readonly AppDbContext _context;
    
    public CinemaRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<Cinema>> GetAllAsync()
    {
        return await _context.Cinemas.ToListAsync();
    }
    
    public async Task<Cinema?> GetByIdAsync(int id)
    {
        return await _context.Cinemas.FindAsync(id);
    }
    
    public async Task<Cinema> CreateAsync(Cinema cinema)
    {
        _context.Cinemas.Add(cinema);
        await _context.SaveChangesAsync();
        return cinema;
    }
    
    public async Task UpdateAsync(Cinema cinema)
    {
        _context.Cinemas.Update(cinema);
        await _context.SaveChangesAsync();
    }
    
    public async Task DeleteAsync(Cinema cinema)
    {
        _context.Cinemas.Remove(cinema);
        await _context.SaveChangesAsync();
    }
}