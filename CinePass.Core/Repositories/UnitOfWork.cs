using CinePass.Core.Configurations;
using CinePass.Domain.IRepository;
using Microsoft.EntityFrameworkCore.Storage;

namespace CinePass.Core.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction _transaction;

    public IMovieRepository Movies { get; }
    public IShowtimeRepository Showtimes { get; }
    public IBookingRepository Bookings { get; }
    public IBookingDetailRepository BookingDetails { get; }
    public ISeatRepository Seats { get; }
    public IScreenRepository Screens { get; }
    public ICinemaRepository Cinemas { get; }
    public IPaymentRepository Payments { get; }
    // public IUserRepository Users { get; }

    public UnitOfWork(AppDbContext context,
        IMovieRepository movieRepo,
        IShowtimeRepository showtimeRepo,
        IBookingRepository bookingRepo,
        IBookingDetailRepository bookingDetailRepo,
        ISeatRepository seatRepo,
        IScreenRepository screenRepo,
        ICinemaRepository cinemaRepo,
        IPaymentRepository paymentRepo)
    {
        _context = context;
        Movies = movieRepo;
        Showtimes = showtimeRepo;
        Bookings = bookingRepo;
        BookingDetails = bookingDetailRepo;
        Seats = seatRepo;
        Screens = screenRepo;
        Cinemas = cinemaRepo;
        Payments = paymentRepo;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
            }
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context?.Dispose();
    }
}