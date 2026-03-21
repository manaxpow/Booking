# Booking System API

A modern vehicle booking management system built with ASP.NET Core, featuring RESTful APIs for managing cars, drivers, schedules, destinations, and user authentication.

## 🚀 Features

- **User Authentication & Authorization** - JWT-based secure authentication
- **Car Management** - CRUD operations for vehicles and seat management
- **Driver Management** - Manage driver profiles and assignments
- **Schedule Management** - Create and manage travel schedules
- **Destination Management** - Manage pickup and drop-off locations
- **Validation** - Request validation using FluentValidation
- **Logging** - Comprehensive logging with Serilog
- **API Documentation** - Interactive Swagger/OpenAPI documentation
- **Caching** - Redis integration for performance optimization
- **Clean Architecture** - Separated layers for better maintainability

## 🛠️ Technology Stack

- **Framework**: ASP.NET Core 10.0
- **Language**: C#
- **Database**: PostgreSQL 16
- **Cache**: Redis 7
- **ORM**: Entity Framework Core 10.0
- **Authentication**: JWT Bearer Tokens
- **Validation**: FluentValidation
- **Logging**: Serilog
- **API Documentation**: Swagger/OpenAPI (Swashbuckle)
- **Containerization**: Docker & Docker Compose

## 📋 Prerequisites

Before running this project, ensure you have the following installed:

- **.NET 10.0 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/10.0)
- **Docker** - [Download here](https://www.docker.com/get-started)
- **Docker Compose** - Usually included with Docker

## 🚦 Quick Start (Docker - Recommended)

The easiest way to run the project is using Docker Compose:

### 1. Clone the Repository

```bash
git clone https://github.com/manaxpow/Booking.git
cd Booking
```

### 2. Configure Environment Variables

Copy the example environment file and customize if needed:

```bash
cp .env.example .env
```

The `.env` file contains:
- Database credentials (PostgreSQL)
- Redis configuration
- ASP.NET Core environment settings

### 3. Start All Services

```bash
docker-compose up -d
```

This will start:
- **PostgreSQL** database (port 5432)
- **PgAdmin** (port 8081) - Database management UI
- **Redis** cache (port 6379)
- **API** (port 8080)

### 4. Verify Services

Check that all containers are running:

```bash
docker-compose ps
```

### 5. Access the Application

- **API**: http://localhost:8080
- **Swagger Documentation**: http://localhost:8080/swagger
- **PgAdmin**: http://localhost:8081 (email: `admin@admin.com`, password: `root`)

## 💻 Local Development (Without Docker)

If you prefer to run the application locally:

### 1. Install Dependencies

Restore NuGet packages:

```bash
dotnet restore
```

### 2. Setup Database

You'll need PostgreSQL running locally. Update the connection string in `Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=BookingDb;Username=your_user;Password=your_password"
  }
}
```

### 3. Apply Database Migrations

```bash
cd DataAccess
dotnet ef database update
cd ..
```

### 4. Run the Application

```bash
dotnet run --project Api
```

The API will be available at: http://localhost:5000

## 📁 Project Structure

The project follows a clean architecture pattern with separated layers:

```
Booking/
├── Api/                    # Presentation layer
│   ├── Controllers/        # API endpoints
│   ├── Validators/         # Request validation
│   ├── Middlewares/        # Custom middleware
│   └── Extensions/         # Service extensions
├── Services/               # Business logic layer
│   ├── Auth/              # Authentication services
│   ├── Car/               # Car management services
│   ├── Driver/            # Driver management services
│   ├── Schedule/          # Schedule management services
│   ├── Destination/       # Destination management services
│   └── User/              # User management services
├── DataAccess/             # Data access layer
│   ├── Data/              # DbContext and factories
│   ├── Migrations/        # EF Core migrations
│   └── Repositories/      # Repository pattern implementations
└── Models/                 # Domain models and DTOs
    ├── Entity/            # Domain entities
    ├── Dtos/              # Data transfer objects
    └── Settings/          # Configuration models
```

## 🔌 API Endpoints

Once the application is running, you can explore all available endpoints through Swagger UI:

### Main Endpoints

| Resource | Base Path | Description |
|----------|-----------|-------------|
| Auth | `/api/auth` | Login and registration |
| Users | `/api/users` | User management |
| Cars | `/api/cars` | Vehicle management |
| Drivers | `/api/drivers` | Driver management |
| Schedules | `/api/schedules` | Schedule management |
| Destinations | `/api/destinations` | Location management |

### Authentication

The API uses JWT Bearer token authentication. To access protected endpoints:

1. Register or login via `/api/auth/register` or `/api/auth/login`
2. Copy the JWT token from the response
3. Include it in the Authorization header: `Bearer YOUR_TOKEN`

## 🗄️ Database Management

### Accessing PostgreSQL Directly

Connect to the database container:

```bash
docker exec -it booking_postgres psql -U admin -d BookingDb
```

### Using PgAdmin

1. Open http://localhost:8081
2. Login with:
   - Email: `admin@admin.com`
   - Password: `root`
3. Add a new server connection:
   - Host: `db`
   - Port: `5432`
   - Username: `admin`
   - Password: `password123`

### Running Migrations

To create new migrations:

```bash
dotnet ef migrations add MigrationName --project DataAccess --startup-project Api
```

To apply migrations:

```bash
dotnet ef database update --project DataAccess --startup-project Api
```

## 🐳 Docker Commands

### Build Containers

```bash
docker-compose build
```

### Start Services

```bash
docker-compose up -d
```

### Stop Services

```bash
docker-compose down
```

### View Logs

```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f api
```

### Remove All Data (Warning: This deletes all data!)

```bash
docker-compose down -v
```

## 🔧 Configuration

### Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `DB_USER` | PostgreSQL username | `admin` |
| `DB_PASSWORD` | PostgreSQL password | `password123` |
| `DB_NAME` | Database name | `BookingDb` |
| `DB_PORT` | PostgreSQL port | `5432` |
| `REDIS_PORT` | Redis port | `6379` |
| `ASPNETCORE_ENVIRONMENT` | ASP.NET environment | `Development` |

### JWT Settings

Configure JWT settings in `Api/appsettings.json`:

```json
{
  "JwtSettings": {
    "SecretKey": "your-secret-key",
    "Issuer": "your-issuer",
    "Audience": "your-audience",
    "ExpirationInMinutes": 60
  }
}
```

## 📝 API Documentation

The project includes interactive API documentation powered by Swagger:

- **Swagger UI**: http://localhost:8080/swagger
- **OpenAPI Spec**: http://localhost:8080/swagger/v1/swagger.json

You can test all endpoints directly from the Swagger UI.

## 🧪 Testing

Run unit tests (if available):

```bash
dotnet test
```

## 🐛 Troubleshooting

### Container Won't Start

Check port conflicts:
```bash
netstat -tuln | grep -E ':(8080|8081|5432|6379)'
```

### Database Connection Issues

Verify database is running:
```bash
docker-compose ps db
```

Check logs:
```bash
docker-compose logs db
```

### Clear Docker Cache

```bash
docker system prune -a
```

### Migration Issues

Reset database (warning: deletes data):
```bash
docker-compose down -v
docker-compose up -d
```

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License.

## 👤 Author

**manaxpow** - [GitHub](https://github.com/manaxpow)

## 🙏 Acknowledgments

- ASP.NET Core team
- Entity Framework Core community
- All contributors