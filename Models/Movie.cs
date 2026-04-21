using System.ComponentModel.DataAnnotations;

namespace MovieJournal.Models;

public class Movie
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Director { get; set; } = string.Empty;

    [Range(1888, 2100)]
    public int Year { get; set; }

    [Required]
    [MaxLength(50)]
    public string Genre { get; set; } = string.Empty;

    [Range(0.0, 10.0)]
    public double Rating { get; set; }

    public DateTime WatchedOn { get; set; } = DateTime.Now;

    [MaxLength(2000)]
    public string? Comment { get; set; }
}