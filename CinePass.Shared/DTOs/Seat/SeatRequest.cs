using CinePass.Domain;

namespace CinePass.Shared.DTOs.Seat;

public class SeatRequest
{
    public int ScreenID { get; set; }
    public string SeatNumber { get; set; }
    public SeatType SeatType { get; set; } = SeatType.Regular;
}