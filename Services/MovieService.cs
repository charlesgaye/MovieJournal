using MovieJournal.Data;
using MovieJournal.Models;

namespace MovieJournal.Services;

public class MovieService
{
    private readonly MovieRepository _repository;

    public MovieService(MovieRepository repository)
    {
        _repository = repository;
    }

    public async Task AddMovieAsync(string title, string director, int year, string genre, double rating, string? comment = null)
    {
        var movie = new Movie
        {
            Title = title,
            Director = director,
            Year = year,
            Genre = genre,
            Rating = rating,
            Comment = comment,
            WatchedOn = DateTime.Now
        };

        await _repository.AddAsync(movie);
        Console.WriteLine($"Film '{title}' ajouté avec succès !");
    }

    public async Task ListMoviesAsync()
    {
        var movies = await _repository.GetAllAsync();

        if (movies.Count == 0)
        {
            Console.WriteLine("Aucun film enregistré pour l'instant.");
            return;
        }

        Console.WriteLine($"\n{"#",-4} {"Titre",-30} {"Réalisateur",-20} {"Année",-6} {"Genre",-15} {"Note",-5}");
        Console.WriteLine(new string('-', 84));

        foreach (var m in movies)
        {
            Console.WriteLine($"{m.Id,-4} {m.Title,-30} {m.Director,-20} {m.Year,-6} {m.Genre,-15} {m.Rating,-5}");
        }
    }

    public async Task RateMovieAsync(int id, double rating)
    {
        if (rating < 0 || rating > 10)
        {
            Console.WriteLine("La note doit être entre 0 et 10.");
            return;
        }

        await _repository.UpdateRatingAsync(id, rating);
        Console.WriteLine("Note mise à jour !");
    }

    public async Task SearchMoviesAsync(string query)
    {
        var results = await _repository.SearchAsync(query);

        if (results.Count == 0)
        {
            Console.WriteLine($"Aucun résultat pour '{query}'.");
            return;
        }

        Console.WriteLine($"\n{results.Count} résultat(s) pour '{query}' :");
        foreach (var m in results)
        {
            Console.WriteLine($"  [{m.Id}] {m.Title} ({m.Year}) — {m.Director} — {m.Rating}/10");
        }
    }

    public async Task ShowStatsAsync()
    {
        var movies = await _repository.GetAllAsync();

        if (movies.Count == 0)
        {
            Console.WriteLine("Pas encore assez de données.");
            return;
        }

        var avgRating = movies.Average(m => m.Rating);
        var favoriteGenre = await _repository.GetFavoriteGenreAsync();
        var bestMovie = movies.OrderByDescending(m => m.Rating).First();
        var totalMovies = movies.Count;

        Console.WriteLine($"\n--- Tes stats ---");
        Console.WriteLine($"Films vus       : {totalMovies}");
        Console.WriteLine($"Note moyenne    : {avgRating:F1}/10");
        Console.WriteLine($"Genre favori    : {favoriteGenre}");
        Console.WriteLine($"Meilleur film   : {bestMovie.Title} ({bestMovie.Rating}/10)");
    }

    public async Task<List<Movie>> GetAllMoviesAsync()
    => await _repository.GetAllAsync();

    public async Task<Movie?> GetMovieByIdAsync(int id)
    => await _repository.GetByIdAsync(id);
}