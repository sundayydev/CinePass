using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CinePass.Domain.Models;

public class Movie
{
    [Key]
    public int MovieID { get; set; }

    [Required, MaxLength(255)]
    public required string Title { get; set; }

    public required string Description { get; set; }

    public int DurationMinutes { get; set; }

    public DateTime? ReleaseDate { get; set; }

    [MaxLength(50)]
    public required string Language { get; set; }

    [MaxLength(50)]
    public required string Genre { get; set; }

    // Thêm trường lưu ảnh phim
    [MaxLength(500)]
    public required string PosterUrl { get; set; } // URL hoặc path tới ảnh

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<Showtime> Showtimes { get; set; }
}

