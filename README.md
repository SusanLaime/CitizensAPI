# CitizensAPI

A .NET Web API project that manages citizens through CRUD operations, CSV persistence, external API integration, and Twelve-Factor App practices.

## Table of Contents

- [Overview](#overview)
- [How It Works](#how-it-works)
- [Architecture](#architecture)
- [Twelve-Factor App Explanation](#12-factor-app-explanation)
- [Tech Stack](#tech-stack)
- [Prerequisites](#prerequisites)
- [Installation & Setup](#installation--setup)
- [Configuration](#configuration)
- [Running the Application](#running-the-application)
- [Project Structure](#project-structure)
- [Development](#development)
- [Troubleshooting](#troubleshooting)
- [Configuration Summary](#configuration-summary)
- [API Endpoints](#api-endpoints)
- [Operational Notes](#operational-notes)
- [Conclusion](#conclusion)

## 📌 Overview

CitizensAPI is a .NET Web API application designed to manage citizens in a registry system. It supports CRUD operations, CSV-based persistence, external API integration, structured logging with Serilog, and configuration through `appsettings` files and environment variables.

Each citizen record includes:

- `FirstName`
- `LastName`
- `CI`
- `BloodGroup`
- `PersonalAsset`

The application also integrates with `https://api.restful-api.dev/objects` to assign a random personal asset when a new citizen is created.

### Key Features

- **CRUD Operations:** Create, retrieve, update, and delete citizens
- **CSV Persistence:** Store citizen records in `CitizenDataStore.csv`
- **External API Integration:** Retrieve random objects to assign personal assets
- **Random Blood Group Assignment:** Automatically assign a valid blood group during citizen creation
- **Controller-Based Architecture:** Organize endpoints using ASP.NET Core controllers
- **Environment-Based Configuration:** Load settings from `appsettings.json`, `appsettings.Development.json`, and environment variables
- **Serilog Logging:** Record application events to the console and log files
- **Swagger Support:** Enable interactive API testing in the development environment
- **Twelve-Factor Alignment:** Document how the project applies Twelve-Factor App principles in practice

## 🧭 How It Works

### Controller-Based Structure

The project uses a controller-based architecture instead of the minimal API scripting style. This approach keeps the API clearer, easier to maintain, and easier to extend as the project grows.

The application starts in `Program.cs`, where it loads configuration, configures Serilog, registers services, and enables Swagger in the development environment.

Client requests are handled by `CitizenController`, which exposes the CRUD endpoints for citizens. The controller reads and updates citizen data stored in `CitizenDataStore.csv`, while `CitizenBGService` connects to the external API to retrieve random objects used as personal assets.

Supporting logic is separated into small parts:

- `Models` define the request and domain objects
- `Services` handle external integration
- `Utils` contains helper logic for CSV read and write operations

The architectural decisions, configuration flow, execution model, and operational behavior of the application are explained in greater detail in the **Twelve-Factor App Explanation** section below, where each relevant aspect of the project is connected to its corresponding factor.

## 🧱 Architecture

```text
Program.cs
   |
   v
Configuration + Serilog + Service Registration
   |
   v
CitizenController
   |
   v
Business Logic + Validation
   |
   +--> CSVHelper --> CitizenDataStore.csv
   |
   +--> CitizenBGService --> External API
```

The application starts in `Program.cs`, where configuration, logging, dependency registration, and middleware are initialized. Incoming HTTP requests are handled by `CitizenController`, which coordinates validation, CRUD operations, CSV persistence through `CSVHelper`, and external object retrieval through `CitizenBGService`.

## 📚 12 Factor App Explanation

### 1. Codebase

> One codebase tracked in revision control, many deploys.

The project is managed in a single Git repository and published on GitHub. Development was carried out through Git commits on the practice branch `P2-001`.

Keeping the project in a single shared codebase with visible history supports traceability and aligns with the codebase factor.

### 2. Dependencies

> Explicitly declare and isolate dependencies.

Dependencies are explicitly declared in the project file `CitizensAPI.csproj`.

Main dependencies:

- `Microsoft.AspNetCore.OpenApi`
- `Swashbuckle.AspNetCore`
- `Newtonsoft.Json`
- `Serilog`

### 3. Config

> Store config in the environment.

The practical configuration of the application is described in the [Configuration](#configuration) section below.

From a Twelve-Factor perspective, this project externalizes configuration through `appsettings.json`, `appsettings.Development.json`, and environment variables instead of hardcoding operational values in controllers or services.

This allows the application to adapt across environments without requiring changes to the source code. For example, settings such as the CSV storage path, the external API base URL, and the active environment can be changed through configuration rather than through direct code edits.

### 4. Backing Services

> Treat backing services as attached resources.

The project uses the external service:

- `https://api.restful-api.dev/objects`

This service is treated as an attached resource that can be replaced or reconfigured without changing the core business logic.

### 5. Build, Release, Run

> Strictly separate build and run stages.

The application separates the following stages:

- build: `dotnet build`
- run: `dotnet run`

The release can be created from the repository state and configuration without changing the source code.

In this project, build and run are treated as separate, repeatable stages.

### 6. Processes

> Execute the app as one or more stateless processes.

The API is designed to behave as a stateless web process.

Citizen data is not permanently stored in the running application. Instead, persistent data is stored in the CSV file:

- `CitizenDataStore.csv`

Each operation reads the current data from the file, applies the requested change, and writes the updated state back to the CSV file. The project also uses `async/await` for external API calls so the application does not block unnecessarily while waiting for responses.

### 7. Port Binding

> Export services via port binding.

The application exposes itself through the ASP.NET Core web server and can be accessed through the configured local port when running with `dotnet run` or the launch settings profile.

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

In this project, the application is designed in a mostly stateless way at the API level, but its persistence layer is a single CSV file. The project includes in-process locking for file access, which helps avoid corruption inside a single running instance. However, because the application rewrites the CSV file during create, update, and delete operations, it still does not provide full coordination for multiple processes or multiple deployed instances.

For that reason, the project is not prepared for real concurrent scaling across multiple processes or instances. The use of `async/await` improves the handling of external API calls by avoiding unnecessary blocking, but it does not by itself guarantee safe concurrency or horizontal scalability. In this project, concurrency is addressed only conceptually.

For the future, this factor could be applied more strongly in this project by:

- replacing CSV persistence with a database that supports transactions and concurrent writes
- adding a safer write coordination strategy
- running multiple API instances behind a load balancer
- separating web requests from background worker processes if the system grows

### 9. Disposability

> Maximize robustness with fast startup and graceful shutdown.

The application starts quickly with `dotnet run` and can stop without requiring complex shutdown steps, for example by pressing `Ctrl + C`. Because persistent data is stored in the CSV file, process restarts do not lose citizen data.

### 10. Dev / Prod Parity

> Keep development, staging, and production as similar as possible.

Development and production should remain as similar as possible by:

- using the same codebase
- using the same dependency definitions
- using configuration files and environment variables for environment-specific values

This reduces unnecessary differences between environments.

It is important to avoid machine-specific noise in the repository and keeping the project portable through configuration and ignored generated files.

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

However, the project already includes supporting elements that can help future maintenance and administration tasks.

For example, the current logging implementation helps with:

- reviewing citizen creation, update, and deletion events
- detecting external API failures
- identifying file read or write problems
- supporting troubleshooting, maintenance, and operational analysis

Because of that, this factor is addressed only partially and conceptually in this practice.

The logs provide useful support for maintenance and administration, but they do not replace a real admin process. In this project, a real admin task would be a separate one-time script or command for activities such as cleaning `CitizenDataStore.csv`, preloading sample citizens, repairing malformed rows, or migrating the CSV structure if the model changes.

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

## 🧰 Tech Stack

- **Framework:** ASP.NET Core Web API (.NET 10)
- **Language:** C#
- **API Documentation:** Swagger / Swashbuckle
- **Logging:** Serilog
- **External HTTP Integration:** `HttpClient`
- **JSON Handling:** Newtonsoft.Json
- **Configuration:** `appsettings.json`, `appsettings.Development.json`, and environment variables
- **Persistence:** CSV file storage through `CitizenDataStore.csv`
- **Version Control:** Git and GitHub

## 📋 Prerequisites

Before running the project, make sure the following tools are available:

- **.NET SDK 10.0**
- **Git**, if you want to clone the repository and manage versions locally
- **Visual Studio** or **Visual Studio Code** with C# support
- **Internet connection**, because the application consumes `https://api.restful-api.dev/objects`

## 🧩 Installation & Setup

1. Clone the repository.
2. Open the project in your editor or IDE.
3. Restore and build the project.
4. Review the configuration files.

### Step 1: Clone the Repository

```bash
git clone <repository-url>
cd CitizensAPI
```

### Step 2: Open the Project

Open the solution in Visual Studio or open the project folder in Visual Studio Code.

### Step 3: Restore and Build the Project

Restore dependencies using:

```bash
dotnet restore
```

Build the application using:

```bash
dotnet build
```

### Step 4: Review Configuration

The application reads configuration from:

- `appsettings.json`
- `appsettings.Development.json`
- `Properties/launchSettings.json`

These files define important settings such as:

- CSV storage location
- external API base URL
- Serilog configuration
- local development environment values

## 🗝️ Configuration

The application uses external configuration files and environment variables to define its runtime behavior.

The main configuration sources are:

- `appsettings.json`
- `appsettings.Development.json`
- `Properties/launchSettings.json`
- environment variables

These sources control settings such as:

- CSV storage location
- external API base URL
- Serilog log levels and outputs
- environment selection through `ASPNETCORE_ENVIRONMENT`

This configuration is loaded in `Program.cs`, allowing the application to keep operational values outside the business logic.

## ▶️ Running the Application

### Start the API

Use the following command to start the API locally:

```bash
dotnet run
```

### Development Features

When the environment is set to `Development`, the application also enables:

- Swagger UI for endpoint testing
- development-specific configuration from `appsettings.Development.json`

### Notes

- Make sure the external API `https://api.restful-api.dev/objects` is reachable
- Make sure the CSV file path configured in the app settings is valid
- Log files are generated automatically through Serilog during execution

## 🗂️ Project Structure

The repository is organized as follows:

```text
CitizensAPI/
├── Controllers/
│   └── CitizenController.cs
├── Models/
│   ├── BloodGroups.cs
│   ├── Citizen.cs
│   ├── CitizenBG.cs
│   ├── CreateCitizenRequest.cs
│   ├── Response.cs
│   └── UpdateCitizenRequest.cs
├── Properties/
│   └── launchSettings.json
├── Services/
│   └── CitizenService.cs
├── Utils/
│   └── CSVHelper.cs
├── appsettings.Development.json
├── appsettings.json
├── CitizenDataStore.csv
├── CitizensAPI.csproj
├── CitizensAPI.sln
├── Program.cs
└── README.md
```

Generated runtime artifacts such as `Logs/`, `bin/`, and `obj/` are not part of the core source structure and are therefore omitted from this overview.

Main responsibilities:

- `Controllers` exposes the API endpoints and coordinates CRUD operations
- `Models` contains the domain entities and request models
- `Services` handles communication with the external object API
- `Utils` contains helper logic for CSV persistence
- `Program.cs` configures the application pipeline, services, logging, and Swagger

## 💻 Development

### Development Workflow

1. **Open the Project**
   - Open the project in Visual Studio or Visual Studio Code

2. **Make Changes**
   - Modify controllers, services, models, configuration files, or utilities depending on the feature being implemented

3. **Build the Project**
   - Run `dotnet build` to verify that the code compiles correctly

4. **Run the Application**
   - Run `dotnet run` to validate the behavior locally

5. **Verify the Changes**
   - Test endpoints with Swagger
   - Review logs generated by Serilog
   - Confirm that CSV persistence behaves correctly

### Code Style

- **Language:** C#
- **Architecture Style:** Controller-based ASP.NET Core Web API
- **Formatting:** Consistent C# formatting through the IDE or editor tooling
- **Naming Conventions:**
  - **Classes and files:** PascalCase, for example `CitizenController.cs`
  - **Methods and properties:** PascalCase, for example `GetCitizenBGs`
  - **Private fields:** `_camelCase`, for example `_httpClient`
  - **Local variables and parameters:** camelCase, for example `citizenRequest`

### Key Development Patterns

1. **Controllers**
   - Handle HTTP requests and coordinate the application flow

2. **Services**
   - Encapsulate external API communication and supporting logic

3. **Models**
   - Represent request bodies and domain entities

4. **Utilities**
   - Centralize reusable helper logic such as CSV file operations

5. **Configuration**
   - Keep runtime settings outside the business logic through app settings files and environment variables

6. **Logging**
   - Use Serilog to record development and runtime events with appropriate log levels

### Validation During Development

The project is currently validated through:

- `dotnet build` to verify successful compilation
- `dotnet run` to execute the API locally
- Swagger to test the endpoints interactively
- Serilog outputs to review runtime behavior and errors

### Build for Production

A production-ready build or deployable version of the application can be generated with:

```bash
dotnet publish -c Release
```

This command creates an optimized publish output for deployment. The `-c Release` option tells .NET to use the **Release** configuration instead of the default development-oriented configuration.

Running `dotnet publish -c Release` does not change the normal development workflow. After publishing, the project can still be built and executed locally with `dotnet build` and `dotnet run` as usual.

## 🛠️ Troubleshooting

### Build Issues

If the project does not build correctly, run:

```bash
dotnet restore
dotnet build
```

Make sure that:

- the .NET SDK 10.0 is installed correctly
- all NuGet dependencies are restored
- the project file `CitizensAPI.csproj` is valid
- there are no syntax errors in recently modified files

### Run Issues

If the application does not start correctly, run:

```bash
dotnet run
```

Verify that:

- the project builds successfully first
- the configured port is not already in use
- the values in `launchSettings.json` are valid
- the environment configuration is correct

### Configuration Issues

If the application cannot find files or services, verify the following configuration sources:

- `appsettings.json`
- `appsettings.Development.json`
- `Properties/launchSettings.json`

Pay special attention to:

- `Data:Location`
- `External Services:ObjectsApi:BaseUrl`
- `ASPNETCORE_ENVIRONMENT`

Incorrect values in these settings may prevent the application from reading the CSV file or connecting to the external API.

### External API Issues

If personal assets are not being assigned correctly:

- verify that `https://api.restful-api.dev/objects` is reachable
- verify that the configured base URL is correct
- review warning and error logs generated by Serilog
- confirm that the external API is returning valid data

### CSV Persistence Issues

If citizen data is not being stored or updated correctly:

- verify that the configured CSV path exists
- verify that the application has permission to read and write the file
- confirm that the CSV file format is valid
- review logs for file read or write errors

### Logging Issues

If logs are not appearing as expected, verify:

- Serilog configuration in `appsettings.json`
- Serilog configuration in `appsettings.Development.json`
- the configured `MinimumLevel`
- the existence of the output log path

Also confirm that the application is running in the expected environment so the correct configuration file is loaded.

### Common Validation Commands

```bash
dotnet restore
dotnet build
dotnet run
dotnet publish -c Release
```

## 📝 Conclusion

This project applies the Twelve-Factor App principles at a practical level by combining configuration management, backing service integration, structured logging, and a simple maintenance-oriented architecture. Although some factors, such as concurrency and admin processes, are only partially implemented, the repository clearly documents both what is already working and what could be improved in a future development.
