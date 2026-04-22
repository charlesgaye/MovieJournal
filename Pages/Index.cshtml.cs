using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieJournal.Models;
using MovieJournal.Services;

namespace MovieJournal.Pages;

public class IndexModel : PageModel
{
    private readonly MovieService _movieService;
    public List<Movie> Movies { get; set; } = [];

    public IndexModel(MovieService movieService)
    {
        _movieService = movieService;
    }

    public async Task OnGetAsync()
    {
        Movies = await _movieService.GetAllMoviesAsync();
    }
}