using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieJournal.Data;
using MovieJournal.Models;
using MovieJournal.Services;

namespace MovieJournal.Pages;

public class AddModel : PageModel
{
    private readonly MovieService _movieService;
    private readonly TmdbClient _tmdbClient;

    public List<MovieSuggestion> Results { get; set; } = [];
    public bool Selected { get; set; }
    public bool Success { get; set; }
    public MovieSuggestion? SelectedMovie { get; set; }
    public string? SelectedDirector { get; set; }

    public AddModel(MovieService movieService, TmdbClient tmdbClient)
    {
        _movieService = movieService;
        _tmdbClient = tmdbClient;
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostSearchAsync(string query)
    {
        Results = await _tmdbClient.SearchMovieAsync(query);
        return Page();
    }

    public async Task<IActionResult> OnPostSelectAsync(
        int tmdbId, string title, int year, string genre)
    {
        SelectedMovie = new MovieSuggestion
        {
            TmdbId = tmdbId,
            Title = title,
            ReleaseYear = year,
            MainGenre = genre
        };
        SelectedDirector = await _tmdbClient.GetDirectorAsync(tmdbId);
        Selected = true;
        return Page();
    }

    public async Task<IActionResult> OnPostAddAsync(
        string title, string director, int year,
        string genre, double rating, string? comment)
    {
        await _movieService.AddMovieAsync(
            title, director, year, genre, rating, comment);
        Success = true;
        return Page();
    }
}