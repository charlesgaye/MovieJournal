# Movie Journal

A personal movie tracking application built in C# to explore the .NET ecosystem,
object-oriented programming, and machine learning concepts.

## Overview

Movie Journal lets you log films you have watched, rate them, and receive
personalized recommendations powered by a weighted scoring algorithm fed by
the TMDB API.

Built as a learning project to demonstrate C# proficiency, clean architecture,
and data-driven thinking.

## Features

- Add, rate, and search movies from a local database
- Genre, director, and decade-based statistics via LINQ
- Personalized movie recommendations using a weighted scoring algorithm
- Real-time movie data fetched from the TMDB API
- Persistent storage with Entity Framework Core and SQLite

## Architecture

The project follows a 3-layer architecture to separate concerns cleanly:

    MovieJournal/
    ├── Models/          # Domain entities (Movie, MovieSuggestion)
    ├── Data/            # Database access (EF Core, Repository, TMDB client)
    ├── Services/        # Business logic (MovieService, SearchService, RecommendationService)
    ├── UI/              # Console interface (ConsoleUI)
    └── Program.cs       # Entry point and dependency injection

Each layer only depends on the layer below it — UI calls Services,
Services call Data, never the other way around.

## Recommendation Algorithm

The current recommendation algorithm is quite simple and is mostly here
to create the sensation of an complete experience. Changes may be needed to
improve the quality of the algorithm, perhaps using ML.

The recommendation engine builds a user profile from rated movies:

- **Genre score** — average rating per genre (weight: 50%)
- **Decade score** — average rating per decade (weight: 30%)
- **Popularity** — normalized TMDB popularity score (weight: 20%)

Each candidate film fetched from TMDB is scored against this profile.
Films from genres the user has never rated receive a penalty to favour
known preferences.

## Tech Stack

| Layer | Technology |
|---|---|
| Language | C# 12 / .NET 8 |
| ORM | Entity Framework Core |
| Database | SQLite |
| External API | TMDB (The Movie Database) |
| DI Container | Microsoft.Extensions.DependencyInjection |
| UI | Console (with Unicode formatting) |

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- A free [TMDB API token](https://www.themoviedb.org/settings/api)

### Setup

```bash
# Clone the repository
git clone https://github.com/charlesgaye/MovieJournal
cd MovieJournal

# Set your TMDB token
export TMDB_TOKEN="your_token_here"   # bash
$env:TMDB_TOKEN="your_token_here"     # PowerShell

# Run the application
dotnet run
```

The SQLite database is created automatically on first launch.

## What I Learned

- Structuring a .NET project with clean layered architecture
- Object-oriented design: encapsulation, dependency injection, separation of concerns
- Entity Framework Core: code-first migrations, LINQ queries, relationships
- Consuming a REST API with HttpClient and System.Text.Json
- Implementing a scoring algorithm from scratch without ML libraries

## Roadmap

- [ ] Migrate UI to ASP.NET Core Razor Pages
- [ ] Add ML.NET collaborative filtering
- [ ] Cache TMDB results locally to reduce API calls
- [ ] Unit tests with xUnit