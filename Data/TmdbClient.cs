using System.Net.Http.Headers;
using System.Text.Json;
using MovieJournal.Models;

namespace MovieJournal.Data;

public class TmdbClient
{
    private readonly HttpClient _http;
    private const string BaseUrl = "https://api.themoviedb.org/3";

    // Remplace par ton token JWT (le long)
    private readonly string Token = 
    Environment.GetEnvironmentVariable("TMDB_TOKEN") 
    ?? throw new InvalidOperationException("TMDB_TOKEN non défini.");
    public TmdbClient()
    {
        _http = new HttpClient();
        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", Token);
    }

    public async Task<List<MovieSuggestion>> GetPopularMoviesAsync(int page = 1)
    {
        var url = $"{BaseUrl}/movie/popular?language=fr-FR&page={page}";
        var response = await _http.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);
        var results = doc.RootElement.GetProperty("results");

        var movies = new List<MovieSuggestion>();

        foreach (var item in results.EnumerateArray())
        {
            var genreIds = item.GetProperty("genre_ids")
                .EnumerateArray()
                .Select(g => g.GetInt32())
                .ToList();

            movies.Add(new MovieSuggestion
            {
                TmdbId = item.GetProperty("id").GetInt32(),
                Title = item.GetProperty("title").GetString() ?? "",
                Overview = item.GetProperty("overview").GetString() ?? "",
                ReleaseYear = ParseYear(item.GetProperty("release_date").GetString()),
                Popularity = item.GetProperty("popularity").GetDouble(),
                GenreIds = genreIds
            });
        }

        return movies;
    }

    public async Task<string> GetGenreNameAsync(int genreId)
    {
        var genres = await GetGenreMapAsync();
        return genres.TryGetValue(genreId, out var name) ? name : "Inconnu";
    }

    public async Task<Dictionary<int, string>> GetGenreMapAsync()
    {
        var url = $"{BaseUrl}/genre/movie/list?language=fr-FR";
        var response = await _http.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);

        return doc.RootElement
            .GetProperty("genres")
            .EnumerateArray()
            .ToDictionary(
                g => g.GetProperty("id").GetInt32(),
                g => g.GetProperty("name").GetString() ?? ""
            );
    }

    private int ParseYear(string? date)
        => int.TryParse(date?.Split('-').FirstOrDefault(), out var y) ? y : 0;

    public async Task<List<MovieSuggestion>> SearchMovieAsync(string query)
    {
        var url = $"{BaseUrl}/search/movie?query={Uri.EscapeDataString(query)}&language=fr-FR";
        var response = await _http.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);
        var results = doc.RootElement.GetProperty("results");

        var genreMap = await GetGenreMapAsync();
        var movies = new List<MovieSuggestion>();

        foreach (var item in results.EnumerateArray().Take(5))
        {
            var genreIds = item.GetProperty("genre_ids")
                .EnumerateArray()
                .Select(g => g.GetInt32())
                .ToList();

            movies.Add(new MovieSuggestion
            {
                TmdbId = item.GetProperty("id").GetInt32(),
                Title = item.GetProperty("title").GetString() ?? "",
                Overview = item.GetProperty("overview").GetString() ?? "",
                ReleaseYear = ParseYear(item.GetProperty("release_date").GetString()),
                Popularity = item.GetProperty("popularity").GetDouble(),
                GenreIds = genreIds,
                MainGenre = genreIds
                    .Select(id => genreMap.TryGetValue(id, out var n) ? n : null)
                    .FirstOrDefault(n => n != null) ?? "Unknown"
            });
        }

        return movies;
    }

    public async Task<string?> GetDirectorAsync(int tmdbId)
    {
        var url = $"{BaseUrl}/movie/{tmdbId}/credits?language=fr-FR";
        var response = await _http.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);

        return doc.RootElement
            .GetProperty("crew")
            .EnumerateArray()
            .FirstOrDefault(p => p.GetProperty("job").GetString() == "Director")
            .GetProperty("name")
            .GetString();
    }
}