namespace CinePass.Shared.DTOs.Movie;

public class MovieRequest
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public int DurationMinutes { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public required string Language { get; set; }
    public required string Genre { get; set; }
    public required string PosterUrl { get; set; }
}