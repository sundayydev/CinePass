namespace CinePass.Shared.DTOs.Seat;

public class SeatMapDto
{
    public int ScreenID { get; set; }
    public string ScreenName { get; set; }
    public int ShowtimeID { get; set; }
    public List<SeatDto> Seats { get; set; }
}