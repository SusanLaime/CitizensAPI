# CitizensAPI

A project developed using a .NET Web API that manages citizens using CRUD operations, CSV persistence, external API integration, and 12 Factor App principles.

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

## Architecture Notes

### Controller-Based Structure

The project uses a controller-based architecture instead of the minimal API scripting style.

This helps organize the API in a more scalable and maintainable way for larger or more structured projects.

## 12 Factor App Explanation

### 1. Codebase

> One codebase tracked in revision control, many deploys.

The project is managed in a single Git repository and published in GitHub.
Development was done through Git commits on the practice branch `P2-001`.

Keeping the project in a single public codebase in the cloud and pushing progress frequently is a good and professional practice so that development history remains visible.

### 2. Dependencies

> Explicitly declare and isolate dependencies.

Dependencies are explicitly declared in the manifesto file `CitizensAPI.csproj`.

Main dependencies:

- `Microsoft.AspNetCore.OpenApi`
- `Swashbuckle.AspNetCore`
- `Newtonsoft.Json`
- `Serilog`

### 3. Config

> Store config in the environment.

Configuration is externalized from code.

Instead of hardcoding values directly in the controller or services, the application reads:

- file storage path
- external API base URL
- logging configuration

from the configuration files and environment variables.

In this project, the application first loads configuration from `appsettings.json` and `appsettings.Development.json`. After that, it also loads environment variables in `Program.cs` using `.AddEnvironmentVariables()`. This means environment variables can override values defined in the configuration files.

A simple example is `ASPNETCORE_ENVIRONMENT`, defined in `launchSettings.json`. It tells the application that it is running in the `Development` environment. Because of that, the app loads development-specific settings and enables development features such as Swagger.

This relates to the config section because environment variables are another way to provide configuration from outside the code. They allow the behavior of the application to change without modifying controllers or services. For example, values such as `Data:Location` and `External Services:ObjectsApi:BaseUrl` can be stored in configuration files during development, but replaced with environment variables in testing or production.


### 4. Backing Services

> Treat backing services as attached resources.

The project uses the external service:

- `https://api.restful-api.dev/objects`

This service is treated as an attached resource that can be replaced or reconfigured without changing core business logic.

### 5. Build, Release, Run

> Strictly separate build and run stages.

The application separates:

- build: `dotnet build`
- run: `dotnet run`

The release can be created from the repository state and configuration without changing code.

In this project, build and run are executed as separate stages and can be repeated consistently.

### 6. Processes

> Execute the app as one or more stateless processes.

The API is designed to behave as a stateless web process.

Citizen data is not permanently stored in the running application. Instead, persistent data is stored in the CSV file:

- `CitizenDataStore.csv`

Each operation reads the current data from the file, applies the requested change, and writes the updated state back to the CSV file. The project also uses `async/await` for external API calls so the application does not block unnecessarily while waiting for responses.

### 7. Port Binding

> Export services via port binding.

The application exposes itself through the ASP.NET Core web server.
It can be accessed through the configured local port when running with `dotnet run` or launch settings.

In the current repository, the configured local endpoint is:

- `https://localhost:9070`

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

### 8. Concurrency

> Scale out via the process model.

This factor is not fully implemented in the current project.

In this project, the application is designed in a mostly stateless way at the API level, but its persistence layer is a single CSV file.
The project includes in-process locking for file access, which helps avoid corruption inside a single running instance. However, because the application rewrites the CSV file during create, update, and delete operations, it still does not provide full coordination for multiple processes or multiple deployed instances.

For that reason, the project is not prepared for real concurrent scaling across multiple processes or instances.
The use of `async/await` improves the handling of external API calls by avoiding unnecessary blocking, but it does not by itself guarantee safe concurrency or horizontal scalability.
In this project, concurrency is addressed only conceptually.

For the future, this factor could be applied more strongly in this project by:

- replacing CSV persistence with a database that supports transactions and concurrent writes
- adding a safer write coordination strategy
- running multiple API instances behind a load balancer
- separating web requests from background worker processes if the system grows


### 9. Disposability

> Maximize robustness with fast startup and graceful shutdown.

The application starts quickly with `dotnet run` and can stop without requiring complex shutdown steps, for example by pressing `Ctrl + C`.
Because persistent data is stored in the CSV file, process restarts do not lose citizen data.

### 10. Dev / Prod Parity

> Keep development, staging, and production as similar as possible.

Development and production should remain as similar as possible by:

- using the same codebase
- using the same dependency definitions
- using configuration files and environment variables for environment-specific values

This reduces differences between environments.

The class also emphasized avoiding machine-specific noise in the repository and keeping the project portable through configuration and ignored generated files.

### 11. Logs

> Treat logs as event streams.

Logs are treated as event streams written by the application.

The current implementation logs:

- citizen creation
- citizen update
- citizen deletion
- external API calls
- file read failures
- API operation failures

In this project, Serilog uses log levels to control which events are recorded. The order of severity starts with `Debug`, followed by `Information`, `Warning`, `Error`, and `Fatal`.

When the `MinimumLevel` is set to `Debug`, the application does not log only debug messages. It logs all messages from `Debug` upward, including `Information`, `Warning`, `Error`, and `Fatal`. In other words, `Debug` acts as the lowest threshold, so every level above it is also included.

This is useful in development because it allows detailed tracing of the application behavior, which helps during debugging.

In the current implementation, `Debug` is used for internal flow tracing, such as loading the CSV file, searching for citizens, preparing file writes, and sending requests to the external API. `Information` is used for successful business events, such as loading citizens, creating a citizen, updating a citizen, deleting a citizen, or receiving valid data from the external API.

`Warning` is used for situations that are important but do not break the application completely. For example, it is used when a citizen is not found, when a duplicate CI is detected, when the external API returns no available assets, or when malformed rows are found in the CSV file. `Error` is used in `try-catch` blocks and failure scenarios where an operation could not be completed correctly, such as file read failures, external API failures, or CRUD exceptions.

This complements the Twelve-Factor idea of logs as event streams because the application is not only writing logs, but also classifying them by severity and purpose. In this way, logs become more useful for debugging, monitoring, maintenance, and understanding the behavior of the system during execution.

### 12. Admin Processes

> Run admin/management tasks as one-off processes.

This factor is not fully implemented as a separate one-off administrative process in the current project.

In this project, the repository does not include a dedicated script or command for administrative tasks such as:

- cleaning the CSV file
- resetting stored citizen data
- seeding initial records
- migrating the file structure

However, the project already includes support elements that can help future maintenance and administration tasks.

For example, the current logging implementation helps with:

- reviewing citizen creation, update, and deletion events
- detecting external API failures
- identifying file read or write problems
- supporting troubleshooting, maintenance, and operational analysis

Because of that, this factor is addressed only partially and conceptually in this practice.

The logs provide useful support for maintenance and administration, but they do not replace a real admin process.
In this project, a real admin task would be a separate one-time script or command for things like cleaning `CitizenDataStore.csv`, preloading sample citizens, repairing malformed rows, or migrating the CSV structure if the model changes.

For the future, this factor could be applied more completely in this project by adding a dedicated maintenance script or command specifically created to clean, reset, seed, or migrate the CSV data.


#### Configuration Summary

Configuration is stored outside the code through:

- `appsettings.json`
- `appsettings.Development.json`
- environment variables

Current configuration includes:

- CSV file location
- external API base URL
- logging configuration

This configuration can support administrative and maintenance tasks because it centralizes where important operational values are defined.

Example:

```json
{
  "Data": {
    "Location": "CitizenDataStore.csv"
  },
  "External Services": {
    "ObjectsApi": {
      "BaseUrl": "https://api.restful-api.dev/"
    }
  }
}
```

#### API Endpoints

### Create Citizen

- `POST /api/Citizen`

Behavior:

- request body only includes `FirstName`, `LastName`, and `CI`
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

- `PUT /api/Citizen/{ci}`

Business rule:

- request body only includes `FirstName` and `LastName`
- only `FirstName` and `LastName` are updated

### Delete Citizen

- `DELETE /api/Citizen/{ci}`

#### Operational Notes

- Swagger is enabled in development mode.
- The project uses LF line endings through `.gitattributes`.
- The CSV file is the persistence layer for this practice.
- The current launch profile uses `https://localhost:9070`.
- The repository follows the controller-oriented style explained in class instead of the minimal scripting style.

All these elements can help an administrator understand how the application is configured, executed, and maintained.

## Conclusion

This project applies the Twelve-Factor App principles at a practical level by combining configuration management, backing service integration, structured logging, and a simple maintenance-oriented architecture. Although some factors such as concurrency and admin processes are only partially implemented, the repository documents both what is already working and what could be improved in a future iteration.
