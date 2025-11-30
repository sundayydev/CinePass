using CinePass.Domain;

namespace CinePass.Shared.DTOs.Seat;

public class SeatDto
{
    public int SeatID { get; set; }
    public string SeatNumber { get; set; }
    public string SeatType { get; set; }
    public bool IsBooked { get; set; }
}