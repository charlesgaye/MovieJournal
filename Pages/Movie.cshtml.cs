using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieJournal.Models;
using MovieJournal.Services;

namespace MovieJournal.Pages;

public class MovieModel : PageModel
{
    private readonly MovieService _movieService;
    public Movie? Film { get; set; }
    public string? ErrorMessage { get; set; }

    public MovieModel(MovieService movieService)
    {
        _movieService = movieService;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Film = await _movieService.GetMovieByIdAsync(id);

        if (Film is null)
        {
            ErrorMessage = "Movie not found.";
            return Page();
        }

        return Page();
    }
}