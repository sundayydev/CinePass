using CinePass.Domain;

namespace CinePass.Shared.DTOs.Seat;

public class SeatResponse
{
    public int SeatID { get; set; }
    public int ScreenID { get; set; }
    public string SeatNumber { get; set; }
    public SeatType SeatType { get; set; }
}