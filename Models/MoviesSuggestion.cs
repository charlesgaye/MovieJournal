namespace MovieJournal.Models;

public class MovieSuggestion
{
    public int TmdbId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Overview { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }
    public double Popularity { get; set; }
    public List<int> GenreIds { get; set; } = [];
    public double PredictedScore { get; set; }  // calculé par RecommendationService
    public string MainGenre { get; set; } = string.Empty;
}