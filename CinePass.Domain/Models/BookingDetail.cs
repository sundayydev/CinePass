using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CinePass.Domain.Models;

public class BookingDetail
{
    [Key]
    public int BookingDetailID { get; set; }

    [Required]
    public int BookingID { get; set; }

    [ForeignKey("BookingID")]
    public Booking Booking { get; set; }

    [Required]
    public int SeatID { get; set; }

    [ForeignKey("SeatID")]
    public Seat Seat { get; set; }

    [Required, MaxLength(100)]
    public string QRToken { get; set; } // mã QR động

    public DateTime TokenExpires { get; set; }
}
