using CinePass.Domain;
using CinePass.Domain.IRepository;
using CinePass.Domain.Models;
using CinePass.Shared.DTOs.Payment;

namespace CinePass.Core.Services;

public class PaymentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly BookingService _bookingService;

    public PaymentService(IUnitOfWork unitOfWork, BookingService bookingService)
    {
        _unitOfWork = unitOfWork;
        _bookingService = bookingService;
    }

    public async Task<PaymentResponse> CreatePaymentAsync(PaymentRequest dto)
    {
        var payment = new Payment
        {
            BookingID = dto.BookingID,
            PaymentMethod = Enum.Parse<PaymentMethod>(dto.PaymentMethod),
            Amount = dto.Amount,
            Status = PaymentStatus.Pending
        };

        await _unitOfWork.Payments.AddAsync(payment);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(payment);
    }

    public async Task<PaymentResponse> ProcessCallbackAsync(PaymentCallback callback)
    {
        var payment = await _unitOfWork.Payments.GetPaymentByTransactionIdAsync(callback.TransactionID);
        if (payment == null)
            throw new InvalidOperationException("Payment not found");

        payment.Status = callback.Status == "Success" ? PaymentStatus.Success : PaymentStatus.Failed;
        await _unitOfWork.Payments.UpdateAsync(payment);

        if (payment.Status == PaymentStatus.Success)
        {
            await _bookingService.ConfirmBookingAsync(payment.BookingID);
        }

        await _unitOfWork.SaveChangesAsync();
        return MapToDto(payment);
    }

    public async Task<string> GenerateMoMoPaymentUrlAsync(int bookingId)
    {
        // Implement MoMo payment URL generation
        // This is a placeholder - you need to implement actual MoMo API integration
        var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId);
        return $"https://test-payment.momo.vn/v2/gateway/api/create?amount={booking.TotalAmount}&orderId={bookingId}";
    }

    public async Task<string> GenerateVnPayPaymentUrlAsync(int bookingId)
    {
        // Implement VnPay payment URL generation
        // This is a placeholder - you need to implement actual VnPay API integration
        var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId);
        return $"https://sandbox.vnpayment.vn/paymentv2/vpcpay.html?amount={booking.TotalAmount}&orderId={bookingId}";
    }

    private PaymentResponse MapToDto(Payment payment)
    {
        return new PaymentResponse
        {
            PaymentID = payment.PaymentID,
            BookingID = payment.BookingID,
            PaymentMethod = payment.PaymentMethod.ToString(),
            TransactionID = payment.TransactionID,
            Amount = payment.Amount,
            PaymentTime = payment.PaymentTime,
            Status = payment.Status.ToString()
        };
    }
}