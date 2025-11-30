namespace CinePass.Domain.IRepository;

public interface IUnitOfWork : IDisposable
{
    IMovieRepository Movies { get; }
    IShowtimeRepository Showtimes { get; }
    IBookingRepository Bookings { get; }
    IBookingDetailRepository BookingDetails { get; }
    ISeatRepository Seats { get; }
    IScreenRepository Screens { get; }
    ICinemaRepository Cinemas { get; }
    IPaymentRepository Payments { get; }
    // IUserRepository Users { get; }
    
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}