namespace CinePass.Shared.DTOs.Payment;

public class PaymentRequest
{
    public int BookingID { get; set; }
    public string PaymentMethod { get; set; }
    public decimal Amount { get; set; }
}