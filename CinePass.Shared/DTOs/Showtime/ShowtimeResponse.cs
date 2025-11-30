using CinePass.Shared.DTOs.Seat;

namespace CinePass.Shared.DTOs.Showtime;

public class ShowtimeResponse
{
    public int ShowtimeID { get; set; }
    public int MovieID { get; set; }
    public string MovieTitle { get; set; }
    public int ScreenID { get; set; }
    public string ScreenName { get; set; }
    public string CinemaName { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal Price { get; set; }
    public int AvailableSeats { get; set; }
}