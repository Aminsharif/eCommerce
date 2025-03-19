# eCommerce Platform

A modern, scalable eCommerce platform built with .NET 8.0 and Angular 17.

## Features

- User Authentication and Authorization
- Product Management
- Shopping Cart
- Order Processing
- Payment Integration
- Admin Dashboard
- Multi-vendor Support
- Reviews and Ratings
- Wishlist Management
- Order Tracking
- Analytics Dashboard
- Inventory Management

## Technology Stack

### Backend
- .NET 8.0
- Entity Framework Core
- SQL Server
- Redis (for caching)
- SignalR (for real-time updates)

### Frontend
- Angular 17
- NgRx for state management
- Angular Material UI
- Bootstrap 5
- Chart.js for analytics

### Infrastructure
- Docker support
- Azure DevOps CI/CD
- Swagger for API documentation

## Prerequisites

1. .NET 8.0 SDK
2. Node.js (v18 or later)
3. Angular CLI
4. SQL Server
5. Redis (optional, for caching)
6. Docker (optional, for containerization)

## Project Structure

```
eCommerce/
├── src/
│   ├── API/                 # Web API project
│   ├── Core/               # Domain and Application Core
│   ├── Infrastructure/     # Data Access and External Services
│   └── Web/                # Angular Frontend
├── tests/
│   ├── Unit/
│   ├── Integration/
│   └── E2E/
└── docs/                   # Documentation
```

## Getting Started

1. Clone the repository
2. Set up the backend:
   ```bash
   cd src/API
   dotnet restore
   dotnet ef database update
   dotnet run
   ```

3. Set up the frontend:
   ```bash
   cd src/Web
   npm install
   ng serve
   ```

4. Access the application:
   - API: https://localhost:5001
   - Frontend: http://localhost:4200
   - Swagger Documentation: https://localhost:5001/swagger

## Contributing

Please read our [Contributing Guidelines](CONTRIBUTING.md) for details on our code of conduct and the process for submitting pull requests.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.