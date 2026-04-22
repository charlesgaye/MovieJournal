# Movie Journal

A personal movie tracking web application built in C# to explore the .NET ecosystem,
object-oriented programming, and data-driven recommendation concepts.

## Overview

Movie Journal lets you log films you have watched, rate them, and receive
personalized recommendations powered by a weighted scoring algorithm fed by
the TMDB API. The application runs locally as a web app accessible via browser.

Built as a learning project to demonstrate C# proficiency, clean architecture,
and data-driven thinking.

## Features

- Search any movie by title — metadata auto-filled from the TMDB API
- Rate and comment on watched films
- Detail page per movie with full information
- Genre, director, and decade-based statistics via LINQ
- Personalized movie recommendations using a weighted scoring algorithm
- Persistent storage with Entity Framework Core and SQLite

## Architecture

The project follows a 3-layer architecture to separate concerns cleanly:

    MovieJournal/
    ├── Models/          # Domain entities (Movie, MovieSuggestion)
    ├── Data/            # Database access (EF Core, Repository, TMDB client)
    ├── Services/        # Business logic (MovieService, SearchService, RecommendationService)
    ├── Pages/           # Razor Pages web interface
    ├── wwwroot/         # Static assets (CSS)
    └── Program.cs       # Entry point, DI container, middleware pipeline

Each layer only depends on the layer below it — Pages call Services,
Services call Data, never the other way around.

## Recommendation Algorithm

The recommendation engine builds a user profile from rated movies:

- **Genre score** — average rating per genre (weight: 50%)
- **Decade score** — average rating per decade (weight: 30%)
- **Popularity** — normalized TMDB popularity score (weight: 20%)

Each candidate film fetched from TMDB is scored against this profile.
Films from genres the user has never rated receive a penalty to favour
known preferences. The algorithm is intentionally simple and transparent —
no ML library required — and is a candidate for future improvement with ML.NET.

## Tech Stack

| Layer | Technology |
|---|---|
| Language | C# / .NET 10 |
| Web framework | ASP.NET Core Razor Pages |
| ORM | Entity Framework Core |
| Database | SQLite |
| External API | TMDB (The Movie Database) |
| DI Container | Built-in ASP.NET Core DI |

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- A free [TMDB API token](https://www.themoviedb.org/settings/api)

### Setup

    # Clone the repository
    git clone https://github.com/charlesgaye/MovieJournal
    cd MovieJournal

    # Set your TMDB token as an environment variable
    export TMDB_TOKEN="your_token_here"        # bash
    $env:TMDB_TOKEN="your_token_here"          # PowerShell

    # Run the application
    dotnet run

Then open http://localhost:5000 in your browser.
The SQLite database is created automatically on first launch.

## What I Learned

- Structuring a .NET project with clean layered architecture
- Object-oriented design: encapsulation, dependency injection, separation of concerns
- Entity Framework Core: code-first approach, LINQ queries, SQLite integration
- ASP.NET Core Razor Pages: page model pattern, routing, form handling
- Consuming a REST API with HttpClient and System.Text.Json
- Implementing a scoring algorithm from scratch without ML libraries
- Git workflow: branching, rebasing, managing a clean repository

## Roadmap

- [ ] Duplicate detection when adding a movie
- [ ] Edit existing movie entries
- [ ] Stats and analytics page
- [ ] Recommendations page with explanations
- [ ] Cache TMDB results locally to reduce API calls
- [ ] Unit tests with xUnit
- [ ] Improve recommendation algorithm with ML.NET