using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CinePass.Domain.Models
{
    public class Screen
    {
        [Key]
        public int ScreenID { get; set; }

        [Required]
        public int CinemaID { get; set; }

        [ForeignKey("CinemaID")]
        public Cinema Cinema { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; }

        public int TotalSeats { get; set; }

        public ICollection<Seat> Seats { get; set; }
        public ICollection<Showtime> Showtimes { get; set; }
    }
}
