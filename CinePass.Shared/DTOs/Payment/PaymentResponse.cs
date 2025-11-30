using CinePass.Domain;

namespace CinePass.Shared.DTOs.Payment;

public class PaymentResponse
{
    public int PaymentID { get; set; }
    public int BookingID { get; set; }
    public string PaymentMethod { get; set; }
    public string TransactionID { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentTime { get; set; }
    public string Status { get; set; }
}