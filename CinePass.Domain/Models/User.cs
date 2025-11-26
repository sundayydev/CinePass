using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CinePass.Domain.Models;

public class User
{
    [Key]
    public int UserID { get; set; }

    [Required, MaxLength(100)]
    public required string FullName { get; set; }

    [Required, MaxLength(100)]
    public required string Email { get; set; }

    [MaxLength(20)]
    public required string PhoneNumber { get; set; }

    [Required]
    public required string PasswordHash { get; set; }

    public UserRole Role { get; set; } = UserRole.Customer;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<Booking> Bookings { get; set; }
}
