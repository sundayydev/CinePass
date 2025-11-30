using System.ComponentModel.DataAnnotations;

namespace CinePass.Shared.DTOs.Screen;

public class InitializeSeatsDto
{
    [Required, Range(1, 26)]
    public int Rows { get; set; }
    
    [Required, Range(1, 50)]
    public int SeatsPerRow { get; set; }
}