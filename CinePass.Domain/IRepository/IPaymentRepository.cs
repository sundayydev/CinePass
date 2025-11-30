using CinePass.Domain.Models;

namespace CinePass.Domain.IRepository;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<IEnumerable<Payment>> GetPaymentsByBookingAsync(int bookingId);
    Task<Payment> GetPaymentByTransactionIdAsync(string transactionId);
    Task<IEnumerable<Payment>> GetPaymentsByStatusAsync(PaymentStatus status);
    Task<decimal> GetTotalPaymentsAsync(DateTime? startDate = null, DateTime? endDate = null);
}