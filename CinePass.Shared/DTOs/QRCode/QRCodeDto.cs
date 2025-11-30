namespace CinePass.Shared.DTOs.QRCode;

public class QRCodeDto
{
    public string QRToken { get; set; }
    public int BookingID { get; set; }
    public string MovieTitle { get; set; }
    public string CinemaName { get; set; }
    public DateTime StartTime { get; set; }
    public List<string> SeatNumbers { get; set; }
    public DateTime TokenExpires { get; set; }
}