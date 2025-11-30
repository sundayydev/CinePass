using CinePass.Shared.DTOs.Screen;

namespace CinePass.Shared.DTOs.Cinema;

public class CinemaResponse
{
    public int CinemaID { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public List<ScreenDto> Screens { get; set; }
    public DateTime CreatedAt { get; set; }
}