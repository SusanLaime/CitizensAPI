# CitizensAPI

< [EN](README.md) | [ES](README.es.md) >

Access the bilingual content by selecting your preferred language above before continuing.

Citizen registry API built with ASP.NET Core, CSV persistence, external API integration, and Twelve-Factor App practices.

> Academic Web API for `Certificacion I` focused on CRUD operations, external configuration, and clean project structure.
> Status: academic project, functional for coursework, not production-ready.

| Highlight | Value |
| --- | --- |
| API Style | Controller-based ASP.NET Core Web API |
| Persistence | `CitizenDataStore.csv` |
| External Service | `https://api.restful-api.dev/objects` |

## Table of Contents

- [👤 Author Information](#author-information)
- [📌 Project Overview](#project-overview)
- [🚀 Quick Start](#quick-start)
- [🧭 How It Works](#how-it-works)
- [🧱 Architecture](#architecture)
- [📚 Twelve-Factor App Explanation](#12-factor-app-explanation)
  - [1. Codebase](#factor-1-codebase)
  - [2. Dependencies](#factor-2-dependencies)
  - [3. Config](#factor-3-config)
  - [4. Backing Services](#factor-4-backing-services)
  - [5. Build, Release, Run](#factor-5-build-release-run)
  - [6. Processes](#factor-6-processes)
  - [7. Port Binding](#factor-7-port-binding)
  - [8. Concurrency](#factor-8-concurrency)
  - [9. Disposability](#factor-9-disposability)
  - [10. Dev / Prod Parity](#factor-10-dev-prod-parity)
  - [11. Logs](#factor-11-logs)
  - [12. Admin Processes](#factor-12-admin-processes)
- [🔌 API Endpoints](#api-endpoints)
- [🧰 Tech Stack](#tech-stack)
- [📋 Prerequisites](#prerequisites)
- [🧩 Installation & Setup](#installation--setup)
- [🗝️ Configuration](#configuration)
- [▶️ Running the Application](#running-the-application)
- [🗂️ Project Structure](#project-structure)
- [💻 Development](#development)
- [🛠️ Troubleshooting](#troubleshooting)
- [🔒 Critical Security Notes](#critical-security-notes)
- [🛡️ Security Improvements](#security-improvements)
- [📚 References](#references)
- [📞 Contact and Support](#contact-and-support)
- [📝 Conclusion](#conclusion)

<a id="author-information"></a>
## 👤 Author Information

`CitizensAPI` is an academic ASP.NET Core Web API project developed by `Susan Laime Lucero` for `Certificacion I`, `Segundo Parcial`, dated `March 18, 2026`. The project is maintained in the repository `SusanLaime/CitizensAPI`, uses ASP.NET Core with Serilog, Swagger, and CSV persistence, and is documented in both English and Spanish through `README.md` and `README.es.md`.

<a id="project-overview"></a>
## 📌 Project Overview

CitizensAPI is a .NET Web API application designed to manage citizens in a registry system. It supports CRUD operations, CSV-based persistence, external API integration, structured logging with Serilog, and configuration through `appsettings` files and environment variables.

Each citizen record includes `FirstName`, `LastName`, `CI`, `BloodGroup`, and `PersonalAsset`. The API also integrates with `https://api.restful-api.dev/objects` to assign a random personal asset when a new citizen is created.

### Key Features

| Feature | Description |
| --- | --- |
| CRUD Operations | Create, retrieve, update, and delete citizens |
| CSV Persistence | Store citizen records in `CitizenDataStore.csv` |
| External API Integration | Retrieve random objects to assign personal assets |
| Random Blood Group Assignment | Automatically assign a valid blood group during citizen creation |
| Controller-Based Architecture | Organize endpoints using ASP.NET Core controllers |
| Environment-Based Configuration | Load settings from `appsettings.json`, `appsettings.Development.json`, and environment variables |
| Serilog Logging | Record application events to the console and log files |
| Swagger Support | Enable interactive API testing in the development environment |
| Twelve-Factor Alignment | Document how the project applies Twelve-Factor App principles in practice |

<a id="quick-start"></a>
## 🚀 Quick Start

Before starting, make sure you have the .NET 10 SDK installed and an internet connection available so dependencies can be restored from NuGet and the external API can be reached.

> Important: `dotnet restore` downloads the project dependencies, and `dotnet build` verifies that the API compiles before you run it.

If you only want to run the API quickly, use this single flow:

```bash
git clone https://github.com/SusanLaime/CitizensAPI.git
cd CitizensAPI
dotnet restore
dotnet build
dotnet run
```

Then open:

- Swagger UI: `https://localhost:9070/swagger`
- OpenAPI document: `https://localhost:9070/openapi/v1.json`
- Base route for citizens: `https://localhost:9070/api/Citizen`

In the current implementation, Swagger uses the document version `v1` and the UI label `My API V1`.

<a id="how-it-works"></a>
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

<a id="architecture"></a>
## 🧱 Architecture

![Architecture Diagram](assets/architecture-diagram.png)

The application follows a layered architecture typical of an ASP.NET Core Web API.

### 🔵 Entry Layer
Handles the application startup and incoming requests.

- **Program.cs**: Configures the application, including dependency injection, logging, middleware, and Swagger.
- **Controllers**: Handle incoming HTTP requests and coordinate the application flow.

### 🟢 Application Layer
Contains the core logic and data structures of the system.

- **Models**: Define the data structures and request/response objects.
- **Services**: Manage business logic and communication with external APIs.
- **Utils**: Handle CSV persistence operations and file management.

### 🟡 Infrastructure Layer
Manages external systems and data persistence.

- **External API**: Provides additional data from outside the application.
- **Persistence File (`CitizenDataStore.csv`)**: Stores and retrieves application data.

<a id="12-factor-app-explanation"></a>
## 📚 Twelve-Factor App Explanation

<a id="factor-1-codebase"></a>
### 1. Codebase

> One codebase tracked in revision control, many deploys.

The project is managed in a single Git repository and published on GitHub. Development was carried out through Git commits on the practice branch `P2-001`.

Keeping the project in a single shared codebase with visible history supports traceability and aligns with the codebase factor.

<a id="factor-2-dependencies"></a>
### 2. Dependencies

> Explicitly declare and isolate dependencies.

Dependencies are explicitly declared in the project file `CitizensAPI.csproj`.

Current package references:

| Package | Version |
| --- | --- |
| `Microsoft.AspNetCore.OpenApi` | `10.0.3` |
| `Newtonsoft.Json` | `13.0.4` |
| `Swashbuckle.AspNetCore` | `10.1.5` |
| `Swashbuckle.AspNetCore.Swagger` | `10.1.5` |
| `Swashbuckle.AspNetCore.SwaggerGen` | `10.1.5` |
| `Swashbuckle.AspNetCore.SwaggerUI` | `10.1.5` |
| `Serilog` | `4.3.1` |
| `Serilog.AspNetCore` | `10.0.0` |
| `Serilog.Settings.Configuration` | `10.0.0` |
| `Serilog.Sinks.Console` | `6.1.1` |
| `Serilog.Sinks.File` | `7.0.0` |

<a id="factor-3-config"></a>
### 3. Config

> Store config in the environment.

The practical configuration of the application is described in the [Configuration](#configuration) section below.

From a Twelve-Factor perspective, this project externalizes configuration through `appsettings.json`, `appsettings.Development.json`, and environment variables instead of hardcoding operational values in controllers or services.

<a id="factor-4-backing-services"></a>
### 4. Backing Services

> Treat backing services as attached resources.

The project uses the external service:

- `https://api.restful-api.dev/objects`

This service is treated as an attached resource that can be replaced or reconfigured without changing the core business logic.

<a id="factor-5-build-release-run"></a>
### 5. Build, Release, Run

> Strictly separate build and run stages.

The application separates the following stages:

- build: `dotnet build`
- run: `dotnet run`

The release can be created from the repository state and configuration without changing the source code.

In this project, build and run are treated as separate, repeatable stages.

<a id="factor-6-processes"></a>
### 6. Processes

> Execute the app as one or more stateless processes.

The API is designed to behave as a stateless web process.

Citizen data is not permanently stored in the running application. Instead, persistent data is stored in the CSV file:

- `CitizenDataStore.csv`

Each operation reads the current data from the file, applies the requested change, and writes the updated state back to the CSV file. The project also uses `async/await` for external API calls so the application does not block unnecessarily while waiting for responses.

<a id="factor-7-port-binding"></a>
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

<a id="factor-8-concurrency"></a>
### 8. Concurrency

> Scale out via the process model.

| Summary | the API is mostly stateless, but CSV persistence prevents safe horizontal scaling. |
| --- | --- |

This factor is not fully implemented in the current project.

In this project, the application is designed in a mostly stateless way at the API level, but its persistence layer is a single CSV file. The project includes in-process locking for file access, which helps avoid corruption inside a single running instance. However, because the application rewrites the CSV file during create, update, and delete operations, it still does not provide full coordination for multiple processes or multiple deployed instances.

For that reason, the project is not prepared for real concurrent scaling across multiple processes or instances. The use of `async/await` improves the handling of external API calls by avoiding unnecessary blocking, but it does not by itself guarantee safe concurrency or horizontal scalability. In this project, concurrency is addressed only conceptually.

For the future, this factor could be applied more strongly in this project by:

- replacing CSV persistence with a database that supports transactions and concurrent writes
- adding a safer write coordination strategy
- running multiple API instances behind a load balancer
- separating web requests from background worker processes if the system grows

<a id="factor-9-disposability"></a>
### 9. Disposability

> Maximize robustness with fast startup and graceful shutdown.

The application starts quickly with `dotnet run` and can stop without requiring complex shutdown steps, for example by pressing `Ctrl + C`. Because persistent data is stored in the CSV file, process restarts do not lose citizen data.

<a id="factor-10-dev-prod-parity"></a>
### 10. Dev / Prod Parity

> Keep development, staging, and production as similar as possible.

Development and production should remain as similar as possible by:

- using the same codebase
- using the same dependency definitions
- using configuration files and environment variables for environment-specific values

This reduces unnecessary differences between environments.

It is important to avoid machine-specific noise in the repository and keep the project portable through configuration and ignored generated files.

<a id="factor-11-logs"></a>
### 11. Logs

> Treat logs as event streams.

| Summary | logging is one of the strongest operational parts of the project because it improves observability, troubleshooting, and execution tracing. |
| --- | --- |

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

<a id="factor-12-admin-processes"></a>
### 12. Admin Processes

> Run admin/management tasks as one-off processes.

| Summary | maintenance is supported indirectly through logs, but there is no dedicated admin command or script yet. |
| --- | --- |

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

<a id="api-endpoints"></a>
## 🔌 API Endpoints

> Note: the response notes in this section document the current controller behavior in the repository, even when that behavior is less strict than ideal REST conventions.

| Operation | Endpoint | Main Result |
| --- | --- | --- |
| Create citizen | `POST /api/Citizen` | Creates a citizen, assigns `BloodGroup` and `PersonalAsset` |
| Get all citizens | `GET /api/Citizen` | Returns the current citizens list |
| Get citizen by CI | `GET /api/Citizen/{id}` | Returns one citizen if found |
| Update citizen | `PUT /api/Citizen/{ci}` | Updates only `FirstName` and `LastName` |
| Delete citizen | `DELETE /api/Citizen/{ci}` | Removes one citizen by CI |

### Create Citizen

- `POST /api/Citizen`

Behavior:

- request body only includes `FirstName`, `LastName`, and `CI`
- validates duplicate CI
- assigns random blood group
- calls external API
- assigns random personal asset
- stores the citizen in CSV

Example request:

```json
{
  "firstName": "Ana",
  "lastName": "Lopez",
  "ci": 123456
}
```

Example created citizen shape:

```json
{
  "firstName": "Ana",
  "lastName": "Lopez",
  "ci": 123456,
  "bloodGroup": "O+",
  "personalAsset": "Apple Watch"
}
```

Current response behavior:

- success returns `200 OK`
- the controller currently returns the full citizens list after creation
- duplicate CI returns `409 Conflict`
- external asset service failure can return `503 Service Unavailable`

### Get All Citizens

- `GET /api/Citizen`

Current response behavior:

- returns `200 OK` with the full citizens list

Example response:

```json
[
  {
    "firstName": "Ana",
    "lastName": "Lopez",
    "ci": 123456,
    "bloodGroup": "O+",
    "personalAsset": "Apple Watch"
  },
  {
    "firstName": "Luis",
    "lastName": "Rojas",
    "ci": 654321,
    "bloodGroup": "A-",
    "personalAsset": "Keyboard"
  }
]
```

### Get Citizen By CI

- `GET /api/Citizen/{id}`

Current response behavior:

- returns `200 OK` with the citizen when found
- returns `200 OK` with a text message when the citizen does not exist

Example response:

```json
{
  "firstName": "Ana",
  "lastName": "Lopez",
  "ci": 123456,
  "bloodGroup": "O+",
  "personalAsset": "Apple Watch"
}
```

### Update Citizen

- `PUT /api/Citizen/{ci}`

Business rule:

- request body only includes `FirstName` and `LastName`
- only `FirstName` and `LastName` are updated

Example request:

```json
{
  "firstName": "Ana Maria",
  "lastName": "Lopez"
}
```

Current response behavior:

- returns `200 OK` with the full citizens list after update
- if the CI is missing, the controller currently returns `200 OK` with a text message

### Delete Citizen

- `DELETE /api/Citizen/{ci}`

Current response behavior:

- returns `200 OK` with the deleted citizen when found
- if the CI is missing, the controller currently returns `200 OK` with a text message

### Example CSV Format

File: `CitizenDataStore.csv`

```csv
Susan,Laime,1,O+,Laptop
Juan,Perez,2,A-,Mouse
```

<a id="operational-notes"></a>
#### Operational Notes

- Swagger is enabled in development mode.
- The project uses LF line endings through `.gitattributes`.
- The CSV file is the persistence layer for this practice.
- The current launch profile uses `https://localhost:9070`.
- The repository follows the controller-oriented style instead of the minimal scripting style.

All these elements can help an administrator understand how the application is configured, executed, and maintained.

<a id="tech-stack"></a>
## 🧰 Tech Stack

- **Framework:** ASP.NET Core Web API (`net10.0`)
- **Language:** C#
- **API Documentation:** OpenAPI + Swagger / Swashbuckle
- **Logging:** Serilog with console and file sinks
- **External HTTP Integration:** `HttpClient`
- **JSON Handling:** Newtonsoft.Json
- **Configuration:** `appsettings.json`, `appsettings.Development.json`, and environment variables
- **Persistence:** CSV file storage through `CitizenDataStore.csv`
- **Version Control:** Git and GitHub

<a id="prerequisites"></a>
## 📋 Prerequisites

Before running the project, make sure the following tools are available:

- **.NET SDK 10.0**
- **Git**, if you want to clone the repository and manage versions locally
- **Visual Studio** or **Visual Studio Code** with C# support
- **Internet connection**, because the application consumes `https://api.restful-api.dev/objects`

<a id="installation--setup"></a>
## 🧩 Installation & Setup

1. Clone the repository, restore dependencies, build, and run the API.
2. Open the project in your editor or IDE if you want to review or modify the code.
3. Review the configuration files if you need to change paths, logging, or environment values.

### Step 1: Clone, Restore, Build, and Run

```bash
# SSH
git clone git@github.com:SusanLaime/CitizensAPI.git
cd CitizensAPI
dotnet restore
dotnet build
dotnet run
```

or 
```bash
#HTTPS
git clone https://github.com/SusanLaime/CitizensAPI.git
cd CitizensAPI
dotnet restore
dotnet build
dotnet run
```

This sequence downloads the project dependencies, verifies that the code compiles, and starts the API locally.

### Step 2: Open the Project

Open the solution file in Visual Studio or open the project folder in Visual Studio Code.

### Step 3: Review Configuration

The application reads configuration from:

- `appsettings.json`
- `appsettings.Development.json`
- `Properties/launchSettings.json`

These files define important settings such as:

- CSV storage location
- external API base URL
- Serilog configuration
- local development environment values

<a id="configuration"></a>
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

Example values from the current project:

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

Note: in the current development configuration, `Data:Location` uses an absolute local Windows path. This matches the repository as it is now, but it reduces portability unless it is overridden through configuration or environment variables.

Example environment variable overrides:

```powershell
$env:Data__Location="D:\\path\\to\\CitizenDataStore.csv"
$env:ASPNETCORE_ENVIRONMENT="Development"
```

<a id="running-the-application"></a>
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
- OpenAPI mapping through `app.MapOpenApi()`

### Notes

- Make sure the external API `https://api.restful-api.dev/objects` is reachable
- Make sure the CSV file path configured in the app settings is valid
- Log files are generated automatically through Serilog during execution

<a id="project-structure"></a>
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

| Area | Responsibility |
| --- | --- |
| `Controllers` | Exposes the API endpoints and coordinates CRUD operations |
| `Models` | Contains the domain entities and request models |
| `Services` | Handles communication with the external object API through `CitizenBGService` |
| `Utils` | Contains helper logic for CSV persistence |
| `Program.cs` | Configures the application pipeline, services, logging, and Swagger |

<a id="development"></a>
## 💻 Development

### Development Workflow

| Step | Action |
| --- | --- |
| 1 | Open the project in Visual Studio or Visual Studio Code |
| 2 | Modify controllers, services, models, configuration files, or utilities |
| 3 | Run `dotnet build` to verify that the project compiles |
| 4 | Run `dotnet run` to validate the behavior locally |
| 5 | Test endpoints with Swagger and review logs plus CSV persistence |

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

| Element | Purpose |
| --- | --- |
| Controllers | Handle HTTP requests and coordinate the application flow |
| Services | Encapsulate external API communication and supporting logic |
| Models | Represent request bodies and domain entities |
| Utilities | Centralize reusable helper logic such as CSV file operations |
| Configuration | Keep runtime settings outside the business logic through app settings files and environment variables |
| Logging | Use Serilog to record development and runtime events with appropriate log levels |

### Validation During Development

| Validation | Purpose |
| --- | --- |
| `dotnet build` | Verify successful compilation |
| `dotnet run` | Execute the API locally |
| Swagger | Test endpoints interactively |
| Serilog outputs | Review runtime behavior and errors |

### Build for Production

A production-ready build or deployable version of the application can be generated with:

```bash
dotnet publish -c Release
```

This command creates an optimized publish output for deployment. The `-c Release` option tells .NET to use the **Release** configuration instead of the default development-oriented configuration.

Running `dotnet publish -c Release` does not change the normal development workflow. After publishing, the project can still be built and executed locally with `dotnet build` and `dotnet run` as usual.

<a id="troubleshooting"></a>
## 🛠️ Troubleshooting

| Issue | Check |
| --- | --- |
| Build issues | Run `dotnet restore` and `dotnet build`, verify .NET 10 SDK, restored NuGet packages, and `CitizensAPI.csproj` validity |
| Run issues | Run `dotnet run`, confirm the project builds first, the port is free, and `launchSettings.json` values are valid |
| Configuration issues | Review `appsettings.json`, `appsettings.Development.json`, `Properties/launchSettings.json`, `Data:Location`, `External Services:ObjectsApi:BaseUrl`, and `ASPNETCORE_ENVIRONMENT` |
| External API issues | Verify `https://api.restful-api.dev/objects` is reachable, the base URL is correct, and logs do not show API failures |
| CSV persistence issues | Confirm the CSV path exists, the app has file permissions, and the file content is valid |
| Logging issues | Review Serilog settings, `MinimumLevel`, output paths, and the selected environment |

### Common Validation Commands

```bash
dotnet restore
dotnet build
dotnet run
dotnet publish -c Release
```

<a id="critical-security-notes"></a>
## 🔒 Critical Security Notes

> Critical: the current API does not implement authentication or authorization, so it should be treated as an academic project and not as a production-ready secure service.

From a cybersecurity perspective, the most critical limitation in the current project is the absence of authentication and authorization. Any client that can reach the API can create, read, update, or delete citizen records, which means access to personal information is not properly restricted.

Other relevant security risks are also present:

- `CitizenDataStore.csv` is acceptable for academic practice, but it is not a strong persistence mechanism for confidentiality, integrity, auditing, or controlled concurrent access
- the external API used for personal assets should be treated as untrusted input and its responses should always be validated carefully
- logs are useful for observability, but in a real deployment they must avoid exposing unnecessary personal data or sensitive internal details

The project already has some positive security foundations, including local HTTPS, configuration outside the codebase, Swagger limited to development, and structured logging through Serilog. Even so, the current state should be understood as an academic implementation rather than a production-ready secure system.

<a id="security-improvements"></a>
## 🛡️ Security Improvements

If the project evolves beyond the academic scope, the most important improvements would be:

- add authentication and authorization to control who can access or modify citizen records
- replace CSV persistence with a database that provides stronger access control, integrity, and auditability
- strengthen input validation and external API response validation
- reduce sensitive logging in production and use stricter log levels outside development
- keep administrative actions controlled and auditable
- maintain protected branches and controlled merges as part of secure development practice

<a id="references"></a>
## 📚 References

1. Wiggins, A. (2017). *The Twelve-Factor App*. https://12factor.net/
2. GitHub. (n.d.). *Managing rulesets for a repository*. GitHub Docs. https://docs.github.com/en/repositories/configuring-branches-and-merges-in-your-repository/managing-rulesets
3. GitHub. (n.d.). *Managing protected branches*. GitHub Docs. https://docs.github.com/en/repositories/configuring-branches-and-merges-in-your-repository/managing-protected-branches
4. Microsoft. (n.d.). *ASP.NET Core configuration*. Microsoft Learn. https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/
5. Microsoft. (2022, April 13). *Storing application secrets safely during development*. Microsoft Learn. https://learn.microsoft.com/en-us/dotnet/architecture/microservices/secure-net-microservices-web-applications/developer-app-secrets-storage
6. OWASP Foundation. (2023). *OWASP API Security Top 10*. https://owasp.org/API-Security/
7. serilog. (n.d.). *Serilog.AspNetCore*. GitHub. https://github.com/serilog/serilog-aspnetcore

<a id="contact-and-support"></a>
## 📞 Contact and Support

If you have questions about the project, the implementation, or the course delivery, you can contact me here:

- Author: Susan Laime Lucero
- Email: `susanlaimel1@upb.edu`
- Last updated: March 18, 2026
- Status: Academic project in working state
- License: MIT License

<a id="conclusion"></a>
## 📝 Conclusion

This project applies the Twelve-Factor App principles at a practical level by combining configuration management, backing service integration, structured logging, and a simple maintenance-oriented architecture. Although some factors, such as concurrency and admin processes, are only partially implemented, the repository clearly documents both what is already working and what could be improved in a future development.
