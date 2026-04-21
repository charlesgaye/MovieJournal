using MovieJournal.Data;
using MovieJournal.Models;

namespace MovieJournal.Services;

public class RecommendationService
{
    private readonly MovieRepository _repository;
    private readonly TmdbClient _tmdbClient;

    public RecommendationService(MovieRepository repository, TmdbClient tmdbClient)
    {
        _repository = repository;
        _tmdbClient = tmdbClient;
    }

    public async Task ShowRecommendationsAsync(int count = 5)
    {
        var myMovies = await _repository.GetAllAsync();

        if (myMovies.Count < 3)
        {
            Console.WriteLine("Ajoute au moins 3 films notés pour obtenir des recommandations.");
            return;
        }

        var profile = BuildProfile(myMovies);
        var genreMap = await _tmdbClient.GetGenreMapAsync();
        var candidates = await _tmdbClient.GetPopularMoviesAsync();

        foreach (var c in candidates)
        {
            c.MainGenre = c.GenreIds
                .Select(id => genreMap.TryGetValue(id, out var n) ? n : null)
                .FirstOrDefault(n => n != null) ?? "Inconnu";
        }

        var seenTitles = myMovies.Select(m => m.Title.ToLower()).ToHashSet();
        candidates = candidates
            .Where(c => !seenTitles.Contains(c.Title.ToLower()))
            .ToList();

        foreach (var candidate in candidates)
            candidate.PredictedScore = ScoreMovie(candidate, profile);

        var top = candidates
            .OrderByDescending(c => c.PredictedScore)
            .Take(count)
            .ToList();

        Console.WriteLine($"\n--- Recommandations pour toi ---");
        Console.WriteLine($"(basé sur {myMovies.Count} films, note moy. {myMovies.Average(m => m.Rating):F1}/10)\n");

        foreach (var movie in top)
        {
            var overview = movie.Overview.Length > 100
                ? movie.Overview[..100] + "..."
                : movie.Overview;

            Console.WriteLine($"  {movie.Title} ({movie.ReleaseYear})");
            Console.WriteLine($"  Genre      : {movie.MainGenre}");
            Console.WriteLine($"  Affinité   : {movie.PredictedScore * 10:F0}%");
            Console.WriteLine($"  Résumé     : {overview}");
            Console.WriteLine();
        }
    }

    // Construit le profil utilisateur à partir des films notés
    private UserProfile BuildProfile(List<Movie> movies)
    {
        var avgRating = movies.Average(m => m.Rating);

        var genreScores = movies
            .GroupBy(m => m.Genre)
            .ToDictionary(g => g.Key, g => g.Average(m => m.Rating));

        var decadeScores = movies
            .GroupBy(m => m.Year / 10 * 10)
            .ToDictionary(g => g.Key, g => g.Average(m => m.Rating));

        var popularityAvg = movies.Count > 0 ? 50.0 : 50.0; // valeur neutre

        return new UserProfile
        {
            AverageRating = avgRating,
            GenreScores = genreScores,
            DecadeScores = decadeScores,
            PopularityAvg = popularityAvg
        };
    }

    // Transforme un film en vecteur [genre, décennie, popularité]
    private double[] MovieToVector(MovieSuggestion candidate, UserProfile profile)
    {
        var genreScore = profile.GenreScores.TryGetValue(candidate.MainGenre, out var g)
            ? g                          // genre connu → ton score
            : profile.AverageRating - 2; // genre inconnu → pénalité volontaire

        var decade = candidate.ReleaseYear / 10 * 10;
        var decadeScore = profile.DecadeScores.TryGetValue(decade, out var d)
            ? d
            : profile.AverageRating;

        var popularityNorm = Math.Min(candidate.Popularity / 100.0, 10.0);

        return [genreScore, decadeScore, popularityNorm];
    }

    // Transforme ton profil en vecteur de référence
    private double[] ProfileToVector(UserProfile profile)
    {
        var avgGenre = profile.GenreScores.Values.Any()
            ? profile.GenreScores.Values.Average()
            : profile.AverageRating;

        var avgDecade = profile.DecadeScores.Values.Any()
            ? profile.DecadeScores.Values.Average()
            : profile.AverageRating;

        return [avgGenre, avgDecade, 5.0];  // popularité neutre pour le profil
    }

    // Calcule la similarité cosinus entre deux vecteurs
    // Résultat entre 0 (opposés) et 1 (identiques)
    private double CosineSimilarity(MovieSuggestion candidate, UserProfile profile)
    {
        var movieVec = MovieToVector(candidate, profile);

        // Le vecteur profil utilise le score SPECIFIQUE au genre du film
        var genreScore = profile.GenreScores.TryGetValue(candidate.MainGenre, out var g)
            ? g
            : profile.AverageRating;

        var decade = candidate.ReleaseYear / 10 * 10;
        var decadeScore = profile.DecadeScores.TryGetValue(decade, out var d)
            ? d
            : profile.AverageRating;

        var profileVec = new double[] { genreScore, decadeScore, 5.0 };

        var dot = movieVec.Zip(profileVec, (a, b) => a * b).Sum();
        var magMovie = Math.Sqrt(movieVec.Sum(x => x * x));
        var magProfile = Math.Sqrt(profileVec.Sum(x => x * x));

        if (magMovie == 0 || magProfile == 0) return 0;

        return dot / (magMovie * magProfile);
    }

    private double ScoreMovie(MovieSuggestion candidate, UserProfile profile)
    {
        double score = 0;
        double totalWeight = 0;

        // Genre (poids 50%) — le plus important
        if (profile.GenreScores.TryGetValue(candidate.MainGenre, out var genreScore))
        {
            score += genreScore * 0.5;
            totalWeight += 0.5;
        }
        else
        {
            // Genre jamais vu → forte pénalité
            score += (profile.AverageRating - 3) * 0.5;
            totalWeight += 0.5;
        }

        // Décennie (poids 30%)
        var decade = candidate.ReleaseYear / 10 * 10;
        if (profile.DecadeScores.TryGetValue(decade, out var decadeScore))
        {
            score += decadeScore * 0.3;
            totalWeight += 0.3;
        }
        else
        {
            score += profile.AverageRating * 0.3;
            totalWeight += 0.3;
        }

        // Popularité TMDB (poids 20%) — normalisée sur 10
        var popularityScore = Math.Min(candidate.Popularity / 50.0, 10.0);
        score += popularityScore * 0.2;
        totalWeight += 0.2;

        return Math.Clamp(score / totalWeight, 0, 10);
    }
}

public class UserProfile
{
    public double AverageRating { get; set; }
    public Dictionary<string, double> GenreScores { get; set; } = [];
    public Dictionary<int, double> DecadeScores { get; set; } = [];
    public double PopularityAvg { get; set; }
}