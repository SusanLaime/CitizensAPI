# CitizensAPI

Practice 2 project for a .NET Web API that manages citizens using CRUD operations, CSV persistence, external API integration, and 12 Factor App principles.

## Project Overview

This API manages citizens in a registry system.

Each citizen contains:

- `FirstName`
- `LastName`
- `CI`
- `BloodGroup`
- `PersonalAsset`

The application supports:

- Create citizen
- Get all citizens
- Get citizen by CI
- Update citizen
- Delete citizen

It also integrates with the external API:

- `https://api.restful-api.dev/objects`

This external API is used to assign a random `PersonalAsset` when creating a citizen.

## Build, Release, Run

### Build

```bash
dotnet build
```

### Run

```bash
dotnet run
```

### Launch Settings and Port

The current repository is configured to run on port `9070`.

This is defined in:

- `Properties/launchSettings.json`

Current launch configuration:

```json
{
  "profiles": {
    "https": {
      "applicationUrl": "https://localhost:9070"
    }
  }
}
```

In the class demonstration, the teacher explained the idea of defining a standard local port and documenting it in the README as part of the Port Binding factor.

### Swagger

When the application runs in development mode, Swagger UI is enabled and can be used to test the endpoints from the browser.

Swagger is enabled only in development mode, which follows the class explanation that Swagger is useful for development and testing, but should not be unnecessarily exposed in production.

## Configuration

Configuration is stored outside the code through:

- `appsettings.json`
- `appsettings.Development.json`
- environment variables

Current configuration includes:

- CSV file location
- external API base URL
- logging configuration

Example:

```json
{
  "Data": {
    "Location": "D:\\UPB D\\UPB 5th Semester\\Certification I\\CitizensAPI\\CitizensAPI\\CitizenDataStore.csv"
  },
  "External Services": {
    "ObjectsApi": {
      "BaseUrl": "https://api.restful-api.dev/"
    }
  }
}
```

The configuration loading is explicit in `Program.cs`, following the class explanation that configuration should be loaded deliberately from:

- `appsettings.json`
- `appsettings.Development.json`
- environment variables

instead of hardcoding those values in the application logic.

## Architecture Notes

### Controller-Based Structure

The project uses a controller-based architecture instead of the minimal API scripting style.

This follows the class recommendation to organize the API in a more scalable and maintainable way for larger or more structured projects.

### Local Protocol Choice

In the class demonstration, the teacher simplified local execution by discussing HTTP-only local setup in order to focus on:

- controllers
- CRUD endpoints
- CSV persistence
- external API integration
- 12 Factor concepts

In this repository, the current local launch profile is configured with:

- `https://localhost:9070`

So the README documents the repository exactly as it is now, while still reflecting the class discussion about keeping local setup practical and explicit.

## 12 Factor App Explanation

### 1. Codebase

The project is managed in a single Git repository.
Development was done through Git commits on the practice branch `P2-001`.

The class also emphasized keeping the project in a single public codebase in the cloud and pushing progress frequently so that development history remains visible.

### 2. Dependencies

Dependencies are explicitly declared in `CitizensAPI.csproj`.

Main dependencies:

- `Microsoft.AspNetCore.OpenApi`
- `Swashbuckle.AspNetCore`
- `Newtonsoft.Json`

### 3. Config

Configuration is externalized from code.

Instead of hardcoding values directly in the controller or services, the application reads:

- file storage path
- external API base URL
- logging configuration

from the configuration files and environment variables.

### 4. Backing Services

The project uses the external service:

- `https://api.restful-api.dev/objects`

This service is treated as an attached resource that can be replaced or reconfigured without changing core business logic.

### 5. Build, Release, Run

The application separates:

- build: `dotnet build`
- run: `dotnet run`

The release can be created from the repository state and configuration without changing code.

This matches the class explanation that build and run should be repeatable and clearly documented.

### 6. Processes

The API behaves as a stateless process.

Citizen data is not permanently stored in memory.
Instead, persistent data is stored in the CSV file:

- `CitizenDataStore.csv`

Each request loads the data from file, applies changes, and writes the updated state back to the CSV file.

### 7. Port Binding

The application exposes itself through the ASP.NET Core web server.
It can be accessed through the configured local port when running with `dotnet run` or launch settings.

In the current repository, the configured local endpoint is:

- `https://localhost:9070`

### 8. Concurrency

This application can scale by running multiple API processes behind a load balancer.
Because the application is designed to be stateless at the process level, scaling horizontally is possible in principle.

For this practice, the persistence layer is a CSV file, so concurrency is conceptually explained even if file-based storage is limited compared to a database.

### 9. Disposability

The application starts quickly with `dotnet run` and can stop without requiring complex shutdown steps.
Because persistent data is stored in the CSV file, process restarts do not lose citizen data.

### 10. Dev / Prod Parity

Development and production should remain as similar as possible by:

- using the same codebase
- using the same dependency definitions
- using configuration files and environment variables for environment-specific values

This reduces differences between environments.

The class also emphasized avoiding machine-specific noise in the repository and keeping the project portable through configuration and ignored generated files.

### 11. Logs

Logs are treated as event streams written by the application.

The current implementation logs:

- citizen creation
- citizen update
- citizen deletion
- external API calls
- file read failures
- API operation failures

### 12. Admin Processes

Administrative tasks should run separately from the main API process.

Examples for this project:

- cleaning CSV test data
- migrating the CSV format
- seeding initial data

These tasks should be executed independently and not embedded into normal API request handling.

## API Endpoints

### Create Citizen

- `POST /api/Citizen`

Behavior:

- validates duplicate CI
- assigns random blood group
- calls external API
- assigns random personal asset
- stores the citizen in CSV

### Get All Citizens

- `GET /api/Citizen`

### Get Citizen By CI

- `GET /api/Citizen/{id}`

### Update Citizen

- `PUT /api/Citizen/{ci1}/{ci2}`

Business rule:

- only `FirstName` and `LastName` are updated

### Delete Citizen

- `DELETE /api/Citizen/{ci}`

## Notes

- Swagger is enabled in development mode.
- The project uses LF line endings through `.gitattributes`.
- The CSV file is the persistence layer for this practice.
- The current launch profile uses `https://localhost:9070`.
- The repository follows the controller-oriented style explained in class instead of the minimal scripting style.
