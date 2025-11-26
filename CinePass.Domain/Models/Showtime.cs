using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CinePass.Domain.Models;

public class Showtime
{
    [Key]
    public int ShowtimeID { get; set; }

    [Required]
    public int MovieID { get; set; }

    [ForeignKey("MovieID")]
    public Movie Movie { get; set; }

    [Required]
    public int ScreenID { get; set; }

    [ForeignKey("ScreenID")]
    public Screen Screen { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    [Required]
    public decimal Price { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<Booking> Bookings { get; set; }
}
