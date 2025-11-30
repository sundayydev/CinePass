namespace CinePass.Shared.DTOs.Payment;

public class PaymentCallback
{
    public string TransactionID { get; set; }
    public int BookingID { get; set; }
    public string Status { get; set; }
    public decimal Amount { get; set; }
}