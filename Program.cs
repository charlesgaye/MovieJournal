using Microsoft.EntityFrameworkCore;
using MovieJournal.Data;
using MovieJournal.Services;

var builder = WebApplication.CreateBuilder(args);

// Force invariant culture for all threads
var invariantCulture = System.Globalization.CultureInfo.InvariantCulture;
System.Globalization.CultureInfo.DefaultThreadCurrentCulture = invariantCulture;
System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = invariantCulture;

builder.Services.AddRazorPages();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=movies.db"));

builder.Services.AddScoped<MovieRepository>();
builder.Services.AddScoped<MovieService>();
builder.Services.AddScoped<SearchService>();
builder.Services.AddScoped<RecommendationService>();
builder.Services.AddSingleton<TmdbClient>();

var app = builder.Build();
app.UseDeveloperExceptionPage();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();

app.Run();