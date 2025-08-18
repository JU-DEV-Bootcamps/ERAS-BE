# ERAS-BE

# Onion Architecture

For this project Onion architecture is going to use this architecture due to the Modularity, Flexibility and Easy to Learn pros. And our Domain Entities Will be surveys, variables, componentes and students info. For more info about the architecture https://github.com/JU-DEV-Bootcamps/ERAS/wiki/Hexagonal-and-Onion-Architecture

### Onion example folder structure

```
src/
|-- Domain/
|   |-- Entities/
|   |-- Repositories/
|   |-- Services/
|-- Application/
|   |-- UseCases/
|   |-- Dtos/
|   |-- Mappers/
|-- Infrastructure/
|   |-- Persistence/
|   |-- Services/
|   |-- External/
|-- Presentation/
|   |-- Controllers/
|   |-- Views/
|   |-- Routes/
```

## Prerequisites

### Required Software
- .NET 8.0 SDK or later
- Docker Desktop
- Git

### Development Dependencies
- PostgreSQL (runs in Docker)
- Keycloak (runs in Docker)

## Local Development Setup

### Step 1: Clone and Navigate
```bash
git clone <repository-url>
cd ERAS/ERAS-BE
```

### Step 2: Start Required Services
Before running the backend, you **must** start the database and authentication services:

```bash
# From the project root (ERAS/)
sudo docker compose up --detach keycloak database
```

This command starts:
- **PostgreSQL Database** (port 5432): Stores all application data
- **Keycloak** (port 18080): Handles authentication and authorization

### Step 3: Configure Application Settings
Edit `src/Eras.Api/appsettings.json` with the following configuration:

```json
{
  "CosmicLatte": {
    "BaseUrl": "https://staging.cosmic-latte.com/api/1.0/",
    "ApiKey": "[ask team member]"
  },
  "Encryption": {
    "Key": "[ask team member]",
    "IV": "[ask team member]"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Keycloak": {
    "BaseUrl": "http://localhost:18080",
    "Realm": "ERAS",
    "ClientId": "api-client",
    "ClientSecret": "[ask team member]",
    "Audience": "api-client"
  },
  "Postgres": {
    "Host": "localhost",
    "Port": "5432",
    "Username": "eras_user",
    "Password": "[ask team member]",
    "Database": "eras_db"
  }
}
```


### Step 4: Run the Backend
```bash
# Install dependencies and run
dotnet restore
dotnet run --project src/Eras.Api/Eras.Api.csproj
```

The API will be available at `http://localhost`.

### Step 5: Verify Setup
- API Documentationavailable at `http://localhost/swagger`
- Keycloak Admin: `http://localhost:18080` (admin/admin)




## Testing

### Running Tests
```bash
dotnet test
```

> [!NOTE]
> If you are using Visual Studio you can use the Run and Debug feature to run the project. Set as startup project the same path `src/Presentation`.


# Project ORM
For data access and data modeling Entity Framework Core will be used with the "Code First" approach
