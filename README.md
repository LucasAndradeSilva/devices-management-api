# 🚀 Devices Management API — Complete Technical Documentation



## 📌 Overview



The **Devices Management API** is a production-ready RESTful service built with modern backend best practices using **.NET**. It provides a robust foundation for managing devices with scalability, testability, and cloud-native architecture in mind.



This project is designed to be:



* Clean and maintainable (Clean Architecture)

* Easily deployable (Dockerized)

* Production-ready (JWT authentication, health checks, logging)

* Testable (Unit tests with mocking)



---



## 🧱 Architecture



The solution follows **Clean Architecture principles**, ensuring separation of concerns:

```csharp
src/

├── Devices.Api            → Presentation Layer (Controllers, Middleware)
├── Devices.Application    → Business Logic (Services, DTOs)
├── Devices.Domain         → Core Entities
├── Devices.Infrastructure → Data Access (EF Core, Repositories)
└── Devices.UnitTests      → Unit Tests
```

### Key Principles:



* Dependency Injection

* SOLID principles

* Repository Pattern

* Service Layer abstraction



---



## ⚙️ Tech Stack



* **.NET 8**

* **ASP.NET Core Web API**

* **Entity Framework Core**

* **SQL Server (Dockerized)**

* **Docker \& Docker Compose**

* **JWT Authentication**

* **xUnit + Moq + FluentAssertions**

* **GitHub Actions (CI/CD ready)**



---



## 🔐 Authentication (JWT)



Authentication is handled using **JSON Web Tokens (JWT)**.



### Flow:



1\. User authenticates via login endpoint

2\. API generates JWT token

3\. Client sends token in headers:



```

Authorization: Bearer {token}

```



### Configuration:



```json

"JwtSettings": {

&#x20; "Secret": "your-secret-key",

&#x20; "Issuer": "devices-api",

&#x20; "Audience": "devices-client",

&#x20; "ExpirationMinutes": 60

}

```



---



## 📡 API Endpoints



### 🔹 Devices



#### GET /api/devices



* Retrieves all devices

* Requires authentication



#### GET /api/devices/{id}



* Retrieves a device by ID



#### POST /api/devices



* Creates a new device



#### PUT /api/devices/{id}



* Updates an existing device



#### DELETE /api/devices/{id}



* Deletes a device



---



## 🧠 Business Rules



* Device name must be unique

* Device status must be valid (Active / Inactive)

* Cannot delete a non-existing device

* Validation handled at service layer



---



## 🗄️ Database



* SQL Server running via Docker

* EF Core used for ORM



### Connection String



```

Server=sqlserver,1433;Database=DevicesDb;User Id=sa;Password=devices123;TrustServerCertificate=True;

```



---



## 📦 Entity Framework Commands



### Create Migration



```bash

dotnet ef migrations add InitialCreate

```



### Update Database



```bash

dotnet ef database update

```



### Remove Migration



```bash

dotnet ef migrations remove

```



---



## 🐳 Docker Setup



### Run the application:



```bash

docker-compose up --build

```



### Services:



* **devices-api** → .NET API

* **sqlserver** → SQL Server container



### Features:



* Healthcheck for SQL Server

* Container networking

* Volume persistence



---



## 🧪 Unit Tests



Implemented using:



* **xUnit**

* **Moq**

* **FluentAssertions**



### Covered Areas:



* Service layer logic

* Business rules validation

* Repository interactions (mocked)



### Example:



```csharp

_repositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
  .ReturnsAsync(device);

result.Should().NotBeNull();

```



---



## 🔄 CI/CD (GitHub Actions)



The project is ready for CI/CD with **GitHub Actions**.



### Pipeline includes:



* Build

* Restore dependencies

* Run unit tests

* Validate code integrity



---



## 📊 Logging \& Observability



* Structured logging with `ILogger`

* Error handling middleware

* Health checks for services



---



## 🛠️ How to Run Locally



### Requirements:



* Docker

* .NET SDK (optional if using Docker only)



### Steps:



```bash

docker-compose up --build

```



API will be available at:



```

http://localhost:5000

```



---



## 📈 Production Readiness



✔ Clean Architecture

✔ Dockerized

✔ Secure with JWT

✔ Unit tested

✔ Scalable structure

✔ CI/CD ready



