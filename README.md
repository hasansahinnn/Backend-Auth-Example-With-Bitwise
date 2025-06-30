
# BackendAuthExample

## Project Purpose

This project demonstrates a **modern, secure authentication and authorization infrastructure** built with **.NET Core Web API**. It features JWT-based authentication (including refresh tokens as JWT), Redis-backed token management, bitwise action-based authorization, and a clean, layered architecture.

---

## Key Features

- **JWT Authentication** (Access & Refresh tokens, both as JWT)
- **Refresh tokens are also generated as JWT**
- **Token storage and validation in Redis** (instant revocation support)
- **Role-based and Bitwise Action-based Authorization**
- **Custom Action Filter for centralized, flexible control**
- **Different token lifetimes for mobile and web clients**
- **SOLID, testable, and maintainable layered architecture**

---

## Technologies Used

- .NET 6+ Web API
- JWT (`System.IdentityModel.Tokens.Jwt`)
- Redis (`StackExchange.Redis`)
- Dependency Injection
- Custom Filters & Attributes
- Layered architecture (Service, Model, Entity, Filter, Controller)
- C#

---

## Folder Structure

```
BackendAuthExample.Api/
│
├── Controllers/
│   ├── LoginController.cs
│   └── ProductController.cs
│
├── Filters/
│   └── LoginFilter.cs
│
├── Models/
│   ├── Entities/
│   ├── RequestModels/
│   ├── Attributes/
│   ├── Exceptions/
│   └── WorkContext/
│
├── Services/
│   ├── TokenService/
│   ├── LoginService/
│   ├── ActionServices/
│   └── UserService/
│
├── appsettings.json
├── Program.cs
├── BackendAuthExample.sln
└── .gitignore
```

---

## How It Works

- **Login**:  
  - User logs in with email and password.
  - If valid, both an **access token** and a **refresh token** (both JWT) are generated.
  - The access token’s JTI is stored in Redis for session management.
  - Token lifetimes:  
    - Mobile: 30 minutes  
    - Web: 90 minutes  
    - Refresh token: 1 year

- **Authorization**:  
  - Custom `LoginFilter` validates the JWT, checks Redis for token validity, and performs bitwise action-based authorization.
  - Role and action permissions are mock-implemented for demo purposes.

- **Bitwise Authorization**:  
  - Each controller/action is mapped to a bitwise value.
  - User’s role determines which actions are allowed per controller.

---

## Getting Started

1. **Clone the repository**
2. **Install dependencies**  
   Run these commands in your project directory:
   ```sh
   dotnet add package Microsoft.IdentityModel.Tokens
   dotnet add package System.IdentityModel.Tokens.Jwt
   dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
   dotnet add package StackExchange.Redis
   ```
3. **Configure Redis**  
   Update `appsettings.json` with your Redis connection string.
4. **Run the project**
   ```sh
   dotnet run --project BackendAuthExample.Api
   ```

---

## Example Login Response

```json
{
  "userId": 1,
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "created": "2024-06-09T12:34:56Z",
  "expiry": "2024-06-09T13:04:56Z"
}
```

---

## Notes

- This project is for educational/demo purposes.  
- For production, always use secure password storage, proper user/role management, and production-grade Redis configuration.

---

If you need a more detailed section (e.g., for API endpoints, bitwise logic, or extending the system), let me know!
