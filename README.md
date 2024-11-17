
# Cofidis Credit API

This repository contains the API for the Cofidis Credit system, which manages **users**, **credit requests**, and **risk analyses**. The API is built with .NET Core and follows clean architecture principles, ensuring modularity and testability.

---

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [API Documentation](#api-documentation)
- [Setup and Installation](#setup-and-installation)
- [Architecture and Patterns](#architecture-and-patterns)
- [Tests](#tests)
- [Docker Support](#docker-support)
- [Database Structure](#database-structure)

---

## Overview

The Cofidis Credit API is a RESTful service designed to handle:
- **User management**: Adding, retrieving, and updating user details.
- **Risk analyses**: Evaluating financial risk based on user data.
- **Credit requests**: Processing credit applications based on risk assessments.

---

## Features

### Users
- Add a user manually or via **Digital Mobile Key**.
- Retrieve user details by ID or NIF.
- Update or delete user records.

### Risk Analyses
- Perform risk evaluations based on financial indicators.
- Update or delete risk analyses.
- Fetch details of specific analyses.

### Credit Requests
- Create credit applications linked to users and risk analyses.
- Retrieve, update, or delete credit requests.
- Get credit applications by user ID.

---

## API Documentation

### Endpoints

#### **UserController**
- `POST /api/user/add-user`: Add a new user.
- `POST /api/user/add-by-digitalkey/{nif}`: Add a user via Digital Mobile Key.
- `PUT /api/user/update-user/{id}`: Update an existing user.
- `GET /api/user/{id}`: Retrieve a user by ID.
- `GET /api/user/nif/{nif}`: Retrieve a user by NIF.
- `DELETE /api/user/delete-user/{id}`: Delete a user.

#### **RiskAnalysisController**
- `POST /api/risk-analisys/add-risk`: Add a new risk analysis.
- `PATCH /api/risk-analisys/update-risk/{id}`: Update an existing risk analysis.
- `GET /api/risk-analisys/by-id/{id}`: Retrieve a risk analysis by ID.
- `DELETE /api/risk-analisys/delete-risk-analisys/{id}`: Delete a risk analysis.

#### **CreditRequestController**
- `POST /api/credit-request/add-credit`: Add a new credit request.
- `PATCH /api/credit-request/update-credit/{id}`: Update a credit request.
- `GET /api/credit-request/by-id/{id}`: Retrieve a credit request by ID.
- `GET /api/credit-request/by-user/{userId}`: Retrieve credit requests by user ID.
- `DELETE /api/credit-request/delete-credit/{id}`: Delete a credit request.

---

## Setup and Installation

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [Docker](https://www.docker.com/)

### Steps
1. Clone the repository:
   ```bash
   git clone https://github.com/PedroPagel/credit-request.git
   cd credit-request
   ```

2. Configure the connection strings in `appsettings.json`:
   ```json
   {
       "ConnectionStrings": {
           "DefaultConnection": "Server=<your-server>;Database=CofidisCredit;Trusted_Connection=True;"
       }
   }
   ```

3. Run the database initialization scripts:
   ```sql
   -- Execute the init.sql file in your preferred SQL Server management tool
   ```

4. Build and run the application:
   ```bash
   dotnet build
   dotnet run
   ```

5. The API will be available at `http://localhost:5000`.

---

## Architecture and Patterns

This API adheres to clean architecture principles and incorporates the following design patterns:

### Patterns Used
1. **Repository Pattern**: Separates data access logic from business logic for maintainability.
2. **Notificator Pattern**: Centralized notification mechanism to handle validation and domain errors.
3. **Exception Middleware**: Global exception handling for a consistent error response structure.
4. **Validator**: Validation logic for incoming requests to ensure data integrity.

---

## Tests

The project includes both **unit tests** and **integration tests**:

1. **Unit Tests**:
   - Validate individual components like services and repositories in isolation.
   - Mock dependencies for precise testing.

2. **Integration Tests**:
   - Verify API endpoints using a `WebApplicationFactory`.
   - Simulate real-world usage scenarios and test end-to-end functionality.

To run the tests:
```bash
dotnet test
```

---

## Docker Support

This project includes a `Dockerfile` for containerized deployment.

### Dockerfile Overview

```dockerfile
# Base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Cofidis.Credit.Api.csproj", "."]
RUN dotnet restore "./Cofidis.Credit.Api.csproj"
COPY . .
RUN dotnet build "./Cofidis.Credit.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "./Cofidis.Credit.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cofidis.Credit.Api.dll"]
```

### Building and Running with Docker
1. Build the Docker image:
   ```bash
   docker build -t cofidis-credit-api .
   ```

2. Run the container:
   ```bash
   docker run -p 5000:8080 cofidis-credit-api
   ```

---

## Database Structure

The API uses two databases:
1. **Primary Database**: `CofidisCredit`
2. **Test Database**: `CofidisCredit_Test`

### Tables
#### `Users`
Stores user details, including NIF (unique tax identifier) and income information.

#### `RiskAnalyses`
Records financial risk assessments for users, including unemployment and inflation rates.

#### `CreditRequests`
Tracks credit applications, linking them to users and their associated risk analyses.

### Stored Procedure
The `sp_GetCreditLimitByIncome` stored procedure calculates a credit limit based on a user's monthly income. 

---

## Mock Digital Mobile Key API

This section describes the **Mock Digital Mobile Key API**, which serves as a mock service to simulate a Digital Mobile Key (DMK) provider for retrieving user information by **NIF**.

### Overview

The Mock Digital Mobile Key API is designed for testing purposes and facilitates the creation of users in the application by simulating external user data retrieval.

#### Key Features
- Retrieve user information based on a unique identifier (NIF).
- Pre-configured users for quick integration.
- Easily extendable for adding new mock users.

### Endpoints

#### **Get User Information by NIF**
- **Endpoint:** `GET /api/DigitalMobileKey/{nif}`
- **Parameters:** 
  - `nif` (string): Unique identifier (NIF) of the user.

#### Responses
- **200 OK:** Returns user details.
  ```json
  {
      "fullName": "João Silva",
      "email": "joao.silva@example.com",
      "phoneNumber": "912345678",
      "nif": "318646883"
  }
  ```
- **404 Not Found:** If the NIF is not found.
  ```json
  {
      "message": "NIF not found"
  }
  ```

### Example Usage

- **Request:** `GET /api/DigitalMobileKey/318646883`
  **Response:**
  ```json
  {
      "fullName": "João Silva",
      "email": "joao.silva@example.com",
      "phoneNumber": "912345678",
      "nif": "318646883"
  }
  ```

- **Request:** `GET /api/DigitalMobileKey/000000000`
  **Response:**
  ```json
  {
      "message": "NIF not found"
  }
  ```

### Adding New Users

To add more users, modify the `_userValues` dictionary in the `DigitalMobileKeyController` constructor.

Example:
```csharp
_userValues.Add("123456789", new User()
{
    FullName = "Maria Pereira",
    Email = "maria.pereira@example.com",
    PhoneNumber = "987654321",
    NIF = "123456789"
});
```
Now, you can retrieve Maria's details using:
```
GET /api/DigitalMobileKey/123456789
```

### Notes

- This API does not persist data and is not connected to a database.
- It is intended for development and testing purposes only.
