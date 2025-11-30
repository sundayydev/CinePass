using CinePass.Core.Configurations;
using CinePass.Domain;
using CinePass.Domain.IRepository;
using CinePass.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CinePass.Core.Repositories;

public class PaymentRepository : Repository<Payment>, IPaymentRepository
{
    public PaymentRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Payment>> GetPaymentsByBookingAsync(int bookingId)
    {
        return await _dbSet
            .Where(p => p.BookingID == bookingId)
            .OrderByDescending(p => p.PaymentTime)
            .ToListAsync();
    }

    public async Task<Payment> GetPaymentByTransactionIdAsync(string transactionId)
    {
        return await _dbSet
            .Include(p => p.Booking)
            .FirstOrDefaultAsync(p => p.TransactionID == transactionId);
    }

    public async Task<IEnumerable<Payment>> GetPaymentsByStatusAsync(PaymentStatus status)
    {
        return await _dbSet
            .Include(p => p.Booking)
            .Where(p => p.Status == status)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalPaymentsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _dbSet.Where(p => p.Status == PaymentStatus.Success);

        if (startDate.HasValue)
            query = query.Where(p => p.PaymentTime >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(p => p.PaymentTime <= endDate.Value);

        return await query.SumAsync(p => p.Amount);
    }
}