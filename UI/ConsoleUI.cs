using MovieJournal.Services;

namespace MovieJournal.UI;

public class ConsoleUI
{
    private readonly MovieService _movieService;
    private readonly SearchService _searchService;
    private readonly RecommendationService _recommendationService;

    public ConsoleUI(MovieService movieService, SearchService searchService, RecommendationService recommendationService)
    {
        _movieService = movieService;
        _searchService = searchService;
        _recommendationService = recommendationService;
    }

    public async Task RunAsync()
    {
        Console.WriteLine("Bienvenue dans Movie Journal !");

        while (true)
        {
            ShowMenu();
            var choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1": await AddMovieAsync(); break;
                case "2": await _movieService.ListMoviesAsync(); break;
                case "3": await SearchAsync(); break;
                case "4": await RateMovieAsync(); break;
                case "5": await _movieService.ShowStatsAsync(); break;
                case "6": await _searchService.ShowGenreStatsAsync(); break;
                case "7": await _searchService.ShowMonthlyProgressAsync(); break;
                case "8": await _searchService.ShowTopDirectorsAsync(); break;
                case "9": await _searchService.ShowOutliersAsync(); break;
                case "r": await _recommendationService.ShowRecommendationsAsync(); break;
                case "0":
                    Console.WriteLine("À bientôt !");
                    return;
                default:
                    Console.WriteLine("Choix invalide, réessaie.");
                    break;
            }

            Console.WriteLine("\nAppuie sur Entrée pour continuer...");
            Console.ReadLine();
        }
    }

    private void ShowMenu()
    {
        Console.Clear();
        Console.WriteLine("╔══════════════════════════════╗");
        Console.WriteLine("║       MOVIE JOURNAL          ║");
        Console.WriteLine("╠══════════════════════════════╣");
        Console.WriteLine("║  1. Ajouter un film          ║");
        Console.WriteLine("║  2. Voir tous les films      ║");
        Console.WriteLine("║  3. Rechercher               ║");
        Console.WriteLine("║  4. Modifier une note        ║");
        Console.WriteLine("║  5. Mes stats générales      ║");
        Console.WriteLine("║  6. Stats par genre          ║");
        Console.WriteLine("║  7. Progression mensuelle    ║");
        Console.WriteLine("║  8. Mes réalisateurs         ║");
        Console.WriteLine("║  9. Coups de coeur           ║");
        Console.WriteLine("║  r. Recommandations          ║");
        Console.WriteLine("║  0. Quitter                  ║");
        Console.WriteLine("╚══════════════════════════════╝");
        Console.Write("\nTon choix : ");
    }

    private async Task AddMovieAsync()
    {
        Console.WriteLine("\n--- Ajouter un film ---");

        Console.Write("Titre : ");
        var title = Console.ReadLine() ?? string.Empty;

        Console.Write("Réalisateur : ");
        var director = Console.ReadLine() ?? string.Empty;

        Console.Write("Année : ");
        var year = int.TryParse(Console.ReadLine(), out var y) ? y : DateTime.Now.Year;

        Console.Write("Genre : ");
        var genre = Console.ReadLine() ?? string.Empty;

        Console.Write("Note (0-10) : ");
        var rating = double.TryParse(Console.ReadLine(), out var r) ? r : 0;

        Console.Write("Commentaire (optionnel) : ");
        var comment = Console.ReadLine();

        await _movieService.AddMovieAsync(title, director, year, genre, rating,
            string.IsNullOrWhiteSpace(comment) ? null : comment);
    }

    private async Task SearchAsync()
    {
        Console.Write("\nRechercher (titre ou réalisateur) : ");
        var query = Console.ReadLine() ?? string.Empty;
        await _movieService.SearchMoviesAsync(query);
    }

    private async Task RateMovieAsync()
    {
        await _movieService.ListMoviesAsync();

        Console.Write("\nID du film à noter : ");
        if (!int.TryParse(Console.ReadLine(), out var id)) return;

        Console.Write("Nouvelle note (0-10) : ");
        if (!double.TryParse(Console.ReadLine(), out var rating)) return;

        await _movieService.RateMovieAsync(id, rating);
    }

    
}