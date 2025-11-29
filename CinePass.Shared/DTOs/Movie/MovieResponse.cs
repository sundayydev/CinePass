namespace CinePass.Shared.DTOs.Movie;

public class MovieResponse
{
    public int MovieID { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int DurationMinutes { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public string Language { get; set; }
    public string Genre { get; set; }
    public string PosterUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}