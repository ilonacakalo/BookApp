# BookApp

The goal of BookApp is to provide a digital platform for students that helps them discover, discuss and acquire helpful books for the subject topics their college courses follow.

## Features

* Search for books filtered by subject topic
* Create reading lists with tracked progress
* Receive app-generated book recommendations based on reading history and marked interests
* Mark recommendations as good or bad (+/-)
* Write reviews and rate books they have read
* Comment on other students' reviews to encourage discussion
* Borrow books directly from other students through borrowing requests, which can be altered, accepted or declined
* Edit profile appearance, interests and other information in the user settings

## Getting Started

### Prerequisites

* [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
* SQL Server (LocalDB, Express or Developer) and, optionally, SSMS to view the database
* Visual Studio 2022 (or another IDE of your choice)
* The EF Core tool: `dotnet tool install --global dotnet-ef --version "9.*"`

### Dependencies

NuGet restores these automatically when you build, so you don't need to install them manually:

* Microsoft.AspNetCore.OpenApi
* Swashbuckle.AspNetCore.SwaggerUI
* Microsoft.EntityFrameworkCore.SqlServer
* Microsoft.EntityFrameworkCore.Design
* BCrypt.Net-Next

### Connection string (User Secrets)

The connection string is not stored in the repository. Each developer sets their own:

1. In Visual Studio, right-click the **WebAPI** project and choose **Manage User Secrets**.
2. Paste the following between the existing `{}` brackets, replacing the server name and database name to your own:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YourServerName;Database=DatabaseName;Trusted_Connection=True;TrustServerCertificate=True"
}
```

Examples of `YourServerName`: `(localdb)\\MSSQLLocalDB` or `localhost\\SQLEXPRESS`.
In JSON the backslash must be doubled (`\\`).

Alternatively, use the terminal from the folder with `BookApp.sln`:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\MSSQLLocalDB;Database=DatabaseName;Trusted_Connection=True;TrustServerCertificate=True" --project WebAPI
```

### Migrations

After setting the connection string, create the database from the existing migrations:

```bash
dotnet ef database update --project WebAPI
```

To add a new migration after changing an entity:

```bash
dotnet ef migrations add MigrationName --project WebAPI
dotnet ef database update --project WebAPI
```

### Running the application

```bash
dotnet run --project WebAPI
```

or press **Run** in Visual Studio. Swagger UI is available at `https://localhost:<port>/swagger` (the port is printed in the console).

## Git workflow

* `main` is protected: direct pushes are blocked, and all changes go through a pull request with at least one review.
* Update `main` before starting a task: `git checkout main`, then `git pull`.
* Create one branch per task from the updated `main` and switch to it with `git checkout -b feature/SCRUM-108-users-migration`. Name it after the Jira key. Jira is not connected to GitHub yet, but the key links each branch and PR to its ticket and makes later review and tracking easier.
* Use short imperative commit messages e.g. `Add Users migration`.
* Push your feature branch (`git push -u origin <branch-name>`) and open a pull request into `main`. Put the Jira key in the PR title and describe what changed.
* After the review and approval, merge the pull request, delete the branch, and run `git pull` on `main`.