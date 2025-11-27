namespace CinePass.Shared.DTOs.Screen;

public class ScreenRequest
{
    public int CinemaID { get; set; }
    public string Name { get; set; }
    public int TotalSeats { get; set; }
}