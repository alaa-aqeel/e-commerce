# ASP.NET Core E-Commerce API 🚀

A production-ready, feature-rich E-Commerce REST API built using **ASP.NET Core 10** following **Clean Architecture** principles.

This project is structured for scalability and maintainability, providing core functionalities needed to run an e-commerce platform out of the box, including product management, user authentication, a shopping cart logic, coupon redemption, dynamic orders, and robust integration tests.

---

## Architecture Overview

This project implements the **Clean Architecture** (Onion Architecture) with distinct layers:

1.  **Domain (`ECommerceApi.Domain`)**: The core models, entities, enums, and base interfaces. No dependencies on external frameworks here.
2.  **Application (`ECommerceApi.Application`)**: Handles all the business logic, mapping (`AutoMapper`), validations (`FluentValidation`), and defines Data Transfer Objects (DTOs) and Service Interfaces.
3.  **Infrastructure (`ECommerceApi.Infrastructure`)**: Carries out data persistence using **Entity Framework Core 10** onto a **PostgreSQL** database. Also contains implementations for external services (e.g., JWT token generation and BCrypt hashing).
4.  **API (`ECommerceApi.API`)**: The presentation layer. RESTful endpoints mapping to `Controllers`, equipped with extensive Swagger documentation and global exception middlewares.

---

## 🌟 Key Features

*   **Authentication & Authorization:** Secure user login utilizing JSON Web Tokens (JWT Bearer) and BCrypt for advanced password hashing. Includes Refresh Tokens logic and localized Roles (Admin/Customer).
*   **Categories & Products:** Support varying categories, product variants, images tracking, inventory limits, and soft-deletes via Global Query Filters.
*   **Interactive Shopping Cart:** Sophisticated Cart service managing products iteratively securely via user profiles.
*   **Checkout & Order Pipeline:** Place orders straight from user carts matching address deliveries and adjusting dynamic stock limits. 
*   **Coupons & Discounts:** Discount logic processing, validative expirations, order minimums, and usage limits handling percentages or flat discounts.
*   **Reviews & Ratings:** Native product reviews linked securely to individual customer accounts.
*   **Notifications Mechanism:** Integrated alert mappings supporting localized "Mark As Read" features.

---

## 🛠️ Tech Stack

*   **Framework:** .NET 10 (ASP.NET Core REST API)
*   **Database:** PostgreSQL (via `Npgsql.EntityFrameworkCore.PostgreSQL`)
*   **ORM:** Entity Framework Core 10
*   **Testing:** xUnit, FluentAssertions, Microsoft.AspNetCore.Mvc.Testing (In-Memory Database integration evaluations)
*   **Key Libraries:** AutoMapper, FluentValidation, BCrypt.Net, Swashbuckle (Swagger/OpenAPI)
*   **Containerization:** Docker support included for Integration Test pipelines.

---

## 🚀 Getting Started

### Prerequisites

*   [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
*   [PostgreSQL](https://www.postgresql.org/download/) installed and running locally (or via Docker).
*   (Optional but Recommended) [Docker](https://www.docker.com/) for running tests easily.

### 1. Database Setup

Update the connection string inside `ECommerceApi.API/appsettings.json` and `appsettings.Development.json` with your actual PostgreSQL credentials:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=ECommerceDb;Username=postgres;Password=your_password"
}
```

Make sure the `dotnet-ef` global tooling is installed:
```bash
dotnet tool install --global dotnet-ef
```

Run Entity Framework Core migrations to generate tables:
```bash
cd ECommerceApi.API
dotnet ef database update -p ../ECommerceApi.Infrastructure -s .
```

### 2. Running Locally

You can launch the web API via standard CLI:

```bash
dotnet run --project ECommerceApi.API/ECommerceApi.API.csproj
```

The application will start mapping to HTTPS (usually `https://localhost:7xxx`).
Navigate to `https://localhost:XXXX/swagger` in your browser to view all available endpoints. Keep an eye out for the interactive Swagger sandbox which fully supports inserting your generated JWT `Bearer` tokens!

---

## 🧪 Testing

This solution emphasizes functional code stability via robust integration tests validating `200 OK`, schema validation (`400 Bad Request`), and authorization limits (`403 Forbidden`). 

### Running Tests Locally

Navigate to the workspace root and run:
```bash
dotnet test ECommerceApi.Tests
```

*Note: The testing pipeline injects its own **In-Memory** Database. No real database records will be manipulated or wiped during these runs.*

### Running Tests via Docker

A standalone setup is mapped directly to a separate `Dockerfile.test`. You do not need the .NET SDK installed to run tests via this layout:
```bash
docker-compose -f docker-compose.test.yml up --build
```
Results automatically mount back into your working directory inside the locally exposed `./TestResults` folder.

---

## 📂 Project Structure Snapshot

```text
├── ECommerceApi.API
│   ├── Controllers     # Route endpoints mapping to services
│   ├── Middleware      # Custom internal error mappings
│   ├── Program.cs      # Native service allocations & DI mapping
├── ECommerceApi.Application
│   ├── Common          # Service / Paged result structures
│   ├── DTOs            # Contract definition schemas
│   ├── Interfaces      # Bound abstractions
│   ├── Mappings        # AutoMapper profile links
├── ECommerceApi.Domain
│   ├── Entities        # Main architectural domain bounds 
│   ├── Enums           # Order states & Types
├── ECommerceApi.Infrastructure
│   ├── Persistence     # EF Core database mapping
│   ├── Repositories    # Generic Repository implementation
│   ├── Services        # Hard implementations executing specific queries
├── ECommerceApi.Tests
│   ├── IntegrationTestBase.cs          # Client and JWT auto-mocker 
│   ├── CustomWebApplicationFactory.cs  # In-memory mapping
│   └── *EndpointTests.cs               # xUnit Fact tests 
└── docker-compose.test.yml
```
