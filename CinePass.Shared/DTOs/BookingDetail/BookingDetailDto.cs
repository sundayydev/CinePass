namespace CinePass.Shared.DTOs.BookingDetail;

public class BookingDetailDto
{
    public int BookingDetailID { get; set; }
    public string SeatNumber { get; set; }
    public string SeatType { get; set; }
    public string QRToken { get; set; }
    public DateTime TokenExpires { get; set; }
}