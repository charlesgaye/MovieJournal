using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MovieJournal.Data;
using MovieJournal.Services;
using MovieJournal.UI;

System.Threading.Thread.CurrentThread.CurrentCulture = 
    System.Globalization.CultureInfo.InvariantCulture;

var services = new ServiceCollection();

services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=movies.db"));

services.AddScoped<MovieRepository>();
services.AddScoped<MovieService>();
services.AddScoped<SearchService>();
services.AddScoped<RecommendationService>();
services.AddSingleton<TmdbClient>();
services.AddScoped<ConsoleUI>();

var provider = services.BuildServiceProvider();

using var scope = provider.CreateScope();

var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
db.Database.EnsureCreated();

var ui = scope.ServiceProvider.GetRequiredService<ConsoleUI>();
await ui.RunAsync();