using Microsoft.EntityFrameworkCore;
using MovieJournal.Models;

namespace MovieJournal.Data;

public class MovieRepository
{
    private readonly AppDbContext _context;

    public MovieRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Movie>> GetAllAsync()
        => await _context.Movies
            .OrderByDescending(m => m.WatchedOn)
            .ToListAsync();

    public async Task<Movie?> GetByIdAsync(int id)
        => await _context.Movies.FindAsync(id);

    public async Task AddAsync(Movie movie)
    {
        _context.Movies.Add(movie);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRatingAsync(int id, double rating)
    {
        var movie = await GetByIdAsync(id);
        if (movie is null) return;

        movie.Rating = rating;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var movie = await GetByIdAsync(id);
        if (movie is null) return;

        _context.Movies.Remove(movie);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Movie>> SearchAsync(string query)
        => await _context.Movies
            .Where(m => m.Title.Contains(query) || m.Director.Contains(query))
            .ToListAsync();

    public async Task<string?> GetFavoriteGenreAsync()
        => await _context.Movies
            .GroupBy(m => m.Genre)
            .OrderByDescending(g => g.Average(m => m.Rating))
            .Select(g => g.Key)
            .FirstOrDefaultAsync();
}