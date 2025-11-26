using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CinePass.Domain.Models;

public class Payment
{
    [Key]
    public int PaymentID { get; set; }

    [Required]
    public int BookingID { get; set; }

    [ForeignKey("BookingID")]
    public Booking Booking { get; set; }

    public PaymentMethod PaymentMethod { get; set; }

    [MaxLength(100)]
    public string TransactionID { get; set; }

    [Required]
    public decimal Amount { get; set; }

    public DateTime PaymentTime { get; set; } = DateTime.Now;

    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
}
