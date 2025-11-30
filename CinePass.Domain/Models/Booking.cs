using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CinePass.Domain.Models;

public class Booking
{
    [Key]
    public int BookingID { get; set; }

    [Required]
    public int UserID { get; set; }

    [ForeignKey("UserID")]
    public User User { get; set; }

    [Required]
    public int ShowtimeID { get; set; }

    [ForeignKey("ShowtimeID")]
    public Showtime Showtime { get; set; }

    public DateTime BookingTime { get; set; } = DateTime.UtcNow;

    public BookingStatus Status { get; set; } = BookingStatus.Pending;

    [Required]
    public decimal TotalAmount { get; set; }

    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;

    [MaxLength(100)]
    public string? PaymentTransactionID { get; set; }

    public ICollection<BookingDetail> BookingDetails { get; set; }
    public ICollection<Payment> Payments { get; set; }
}
