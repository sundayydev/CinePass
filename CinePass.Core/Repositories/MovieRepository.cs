using CinePass.Core.Configurations;
using CinePass.Domain.IRepository;
using CinePass.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CinePass.Core.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly AppDbContext _context;
    
    public MovieRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Movie>> GetAllAsync()
    {
        return await _context.Movies.ToListAsync();
    }
    
    public async Task<Movie?> GetByIdAsync(int id)
    {
        return await _context.Movies.FirstOrDefaultAsync(m => m.MovieID == id);
    }
    
    public async Task<Movie> CreateAsync(Movie movie)
    {
        _context.Movies.Add(movie);
        await _context.SaveChangesAsync();
        return movie;
    }
    
    public async Task<bool> UpdateAsync(Movie movie)
    {
        _context.Movies.Update(movie);
        return await _context.SaveChangesAsync() > 0;
    }
    
    public async Task<bool> DeleteAsync(int id)
    {
        var movie = await GetByIdAsync(id);
        if (movie == null) return false;
        
        _context.Movies.Remove(movie);
        return await _context.SaveChangesAsync() > 0;
    }
}