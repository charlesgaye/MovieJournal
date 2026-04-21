using MovieJournal.Data;
using MovieJournal.Models;

namespace MovieJournal.Services;

public class SearchService
{
    private readonly MovieRepository _repository;

    public SearchService(MovieRepository repository)
    {
        _repository = repository;
    }

    // Films filtrés par genre
    public async Task<List<Movie>> GetByGenreAsync(string genre)
    {
        var movies = await _repository.GetAllAsync();
        return movies
            .Where(m => m.Genre.Equals(genre, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(m => m.Rating)
            .ToList();
    }

    // Stats par genre : note moyenne + nombre de films
    public async Task ShowGenreStatsAsync()
    {
        var movies = await _repository.GetAllAsync();

        var stats = movies
            .GroupBy(m => m.Genre)
            .Select(g => new
            {
                Genre = g.Key,
                Count = g.Count(),
                AvgRating = g.Average(m => m.Rating),
                Best = g.OrderByDescending(m => m.Rating).First().Title
            })
            .OrderByDescending(g => g.AvgRating)
            .ToList();

        Console.WriteLine($"\n{"Genre",-20} {"Films",-8} {"Moy.",-8} {"Meilleur film"}");
        Console.WriteLine(new string('-', 70));

        foreach (var s in stats)
        {
            Console.WriteLine($"{s.Genre,-20} {s.Count,-8} {s.AvgRating:F1,-8} {s.Best}");
        }
    }

    // Progression mensuelle
    public async Task ShowMonthlyProgressAsync()
    {
        var movies = await _repository.GetAllAsync();

        var monthly = movies
            .GroupBy(m => new { m.WatchedOn.Year, m.WatchedOn.Month })
            .Select(g => new
            {
                Period = $"{g.Key.Year}/{g.Key.Month:D2}",
                Count = g.Count(),
                AvgRating = g.Average(m => m.Rating)
            })
            .OrderBy(g => g.Period)
            .ToList();

        Console.WriteLine($"\n{"Période",-12} {"Films vus",-12} {"Note moy."}");
        Console.WriteLine(new string('-', 36));

        foreach (var m in monthly)
        {
            var bar = new string('█', m.Count);
            Console.WriteLine($"{m.Period,-12} {bar,-12} {m.AvgRating:F1}/10");
        }
    }

    // Réalisateurs les plus regardés
    public async Task ShowTopDirectorsAsync()
    {
        var movies = await _repository.GetAllAsync();

        var directors = movies
            .GroupBy(m => m.Director)
            .Where(g => g.Count() > 1)
            .Select(g => new
            {
                Director = g.Key,
                Count = g.Count(),
                AvgRating = g.Average(m => m.Rating)
            })
            .OrderByDescending(d => d.Count)
            .Take(5)
            .ToList();

        if (directors.Count == 0)
        {
            Console.WriteLine("\nPas encore assez de films du même réalisateur.");
            return;
        }

        Console.WriteLine($"\n{"Réalisateur",-25} {"Films",-8} {"Note moy."}");
        Console.WriteLine(new string('-', 45));

        foreach (var d in directors)
        {
            Console.WriteLine($"{d.Director,-25} {d.Count,-8} {d.AvgRating:F1}/10");
        }
    }

    // Films sous-notés et sur-notés par rapport à ta moyenne
    public async Task ShowOutliersAsync()
    {
        var movies = await _repository.GetAllAsync();
        var avg = movies.Average(m => m.Rating);

        var underrated = movies
            .Where(m => m.Rating < avg - 1.5)
            .OrderBy(m => m.Rating)
            .Take(3)
            .ToList();

        var overrated = movies
            .Where(m => m.Rating > avg + 1.5)
            .OrderByDescending(m => m.Rating)
            .Take(3)
            .ToList();

        Console.WriteLine($"\nTa note moyenne : {avg:F1}/10");

        Console.WriteLine($"\nTes films les moins aimés :");
        foreach (var m in underrated)
            Console.WriteLine($"  {m.Title} — {m.Rating}/10");

        Console.WriteLine($"\nTes coups de coeur :");
        foreach (var m in overrated)
            Console.WriteLine($"  {m.Title} — {m.Rating}/10");
    }
}