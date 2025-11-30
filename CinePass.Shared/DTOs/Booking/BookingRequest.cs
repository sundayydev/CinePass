namespace CinePass.Shared.DTOs.Booking;

public class BookingRequest
{
    public int UserID { get; set; }
    public int ShowtimeID { get; set; }
    public List<int> SeatIDs { get; set; }
    public string PaymentMethod { get; set; }
}