using CinePass.Shared.DTOs.BookingDetail;
namespace CinePass.Shared.DTOs.Booking;

public class BookingResponse
{
    public int BookingID { get; set; }
    public int UserID { get; set; }
    public string UserName { get; set; }
    public int ShowtimeID { get; set; }
    public string MovieTitle { get; set; }
    public string CinemaName { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime BookingTime { get; set; }
    public string Status { get; set; }
    public decimal TotalAmount { get; set; }
    public string PaymentMethod { get; set; }
    public List<BookingDetailDto> BookingDetails { get; set; }
}