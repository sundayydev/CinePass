namespace CinePass.Shared.DTOs.Screen;

public class ScreenWithSeatsDto
{
    public int ScreenID { get; set; }
    public int CinemaID { get; set; }
    public string CinemaName { get; set; }
    public string Name { get; set; }
    public int TotalSeats { get; set; }
    public List<SeatDetailDto> Seats { get; set; }
}