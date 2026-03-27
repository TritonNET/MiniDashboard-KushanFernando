### MiniDashboard – Architecture, Setup & Testing Guide

MiniDashboard is a modular .NET 8 solution that demonstrates a complete stack:

- ASP.NET Core API – exposes product CRUD + search
- Service layer – business logic
- Data access layer – JSON-based storage engine
- Client library – strongly typed HTTP client + caching layer
- WPF Desktop Application – MVVM UI for managing products
- Unit Tests (xUnit + Moq) – service & API controller tests

The project is intentionally simple, clean, and fully testable.


### Solution Structure

```
MiniDashboard/
│
├── MiniDashboard.Api/          -> ASP.NET Core Web API
├── MiniDashboard.Services/     -> Business logic layer
├── MiniDashboard.DataAccess/   -> JSON store, repository interfaces
├── MiniDashboard.Client/       -> HttpClient wrapper + cache service
├── MiniDashboard.Common/       -> Shared models + interfaces + helpers
├── MiniDashboard.App/          -> WPF/MVVM Desktop UI
│
├── MiniDashboard.Api.Test/     -> API controller tests
└── MiniDashboard.Services.Test -> Service logic unit tests
```


### Architecture Summary

1. Data Access Layer (`MiniDashboard.DataAccess`)
   
   Stores products in datastore.json located next to the executing assembly.
   Implements:
        `IProductStore`
    Reads/writes data using thread-safe file operations.
    Config-aware via `IConfigProvider`.

2. Service Layer (`MiniDashboard.Services`)

   Business logic sits here:
   
   Implements `IProductService`
     Handles:
        - Validation
        - Filtering
        - CRUD logic
        - Logging integration via `ILogger`

3. API Layer (`MiniDashboard.Api`)
    
   Exposes the REST interface:
     Routes under `/product`

        - GET /product
        - GET /product/{id}
        - GET /product/search
        - POST /product
        - PUT /product/{id}
        - DELETE /product/{id}

   Has unified exception pipeline in `ApiControllerBase`

   ++ Swagger is enabled

4. Client Layer (`MiniDashboard.Client`)

   Provides external access to the API:

    `ProductClient` – HttpClient wrapper
    `ProductCacheService` – adds in-memory caching to reduce API calls

5. WPF App (`MiniDashboard.App`)

    MVVM-based desktop UI.
    Key components:

      `VmProducts` – manages product list
      `ProductWindow.xaml` – UI with
          - filtering UI
          - async loading
    


# Setup Instructions

Install Prerequisites

- .NET 8 SDK
- Visual Studio 2022
- Windows (for WPF app)

Clone the Repository

```
https://github.com/TritonNET/MiniDashboard-KushanFernando
cd MiniDashboard-KushanFernando
```

Run the API

From the `MiniDashboard.Api` project folder:

```
dotnet run
```

It will start at:
```
http://localhost:5130
```

Swagger UI:
```
http://localhost:5130/swagger
```

Run the WPF App

In Visual Studio:

 Set MiniDashboard.App as startup project -> `F5`


## Running the Tests

There are two test projects:

`MiniDashboard.Api.Test`
`MiniDashboard.Services.Test`

To run all tests:
```
dotnet test
```

Run a specific project:

```
dotnet test MiniDashboard.Services.Test
```

## Configuration

 - Currently uses a hardcoded file name next to the executable. It is configured in `APIServerConfigProvider`
```c#
public string GetProductStoreFilePath()
{
    return Path.Combine(AppContext.BaseDirectory, "datastore.json");
}
```