using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CinePass.Domain.Models;

public class Cinema
{
    [Key]
    public int CinemaID { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; }

    [MaxLength(255)]
    public string Address { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<Screen> Screens { get; set; }
}
