using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CinePass.Domain.Models;

public class Seat
{
    [Key]
    public int SeatID { get; set; }

    [Required]
    public int ScreenID { get; set; }

    [ForeignKey("ScreenID")]
    public Screen Screen { get; set; }

    [Required, MaxLength(10)]
    public string SeatNumber { get; set; }

    public SeatType SeatType { get; set; } = SeatType.Regular;

    public ICollection<BookingDetail> BookingDetails { get; set; }
}

