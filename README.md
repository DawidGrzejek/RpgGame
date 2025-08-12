# RPG Game System

A comprehensive RPG game system built with Clean Architecture principles, featuring template-driven character creation, event sourcing, and real-time multiplayer capabilities.

## 🚀 Quick Start

### Prerequisites
- .NET 9 SDK
- Node.js 18+ and npm
- SQL Server (LocalDB or remote)
- Python 3.9+ (for GitGuardian Shield)

### Environment Setup
1. **Copy environment template:**
   ```bash
   cp .env.example .env
   ```

2. **Configure your environment variables in `.env`:**
   ```bash
   CONNECTION_STRING_DEFAULT=your-database-connection-string
   SEED_USER_PASSWORD=your-secure-password
   ```

3. **Install dependencies:**
   ```bash
   # Backend
   dotnet build
   
   # Frontend
   cd src/RpgGame.AngularUI/rpg-game-ui
   npm install
   ```

### Running the Application

1. **Start the API:**
   ```bash
   cd src/RpgGame.WebApi
   dotnet run
   ```

2. **Start the Angular UI:**
   ```bash
   cd src/RpgGame.AngularUI/rpg-game-ui
   npm start
   ```

3. **Access the application:**
   - API: https://localhost:7000
   - Angular UI: http://localhost:4200
   - API Documentation: https://localhost:7000/swagger

## 🔒 Security Features

### GitGuardian Shield Protection
This project is protected by [GitGuardian Shield](https://github.com/GitGuardianHQ/ggshield) to prevent secret leaks:

- **Pre-commit hooks**: Automatically scan commits for secrets before they reach the repository
- **Repository scanning**: Regular scans to detect any exposed credentials
- **Configuration**: See `.ggshield.yaml` for scan settings and exclusions

### Environment-Based Configuration
- **No hardcoded credentials**: All sensitive data configured via environment variables
- **Secure defaults**: Application fails fast if required security configuration is missing
- **Protected files**: Configuration files with credentials are gitignored

### Authentication System
- JWT-based authentication with refresh tokens
- Role-based access control (Player, Moderator, Admin, GameMaster)
- Default development user: `GameMaster` / `gamemaster@rpggame.local`

## 🏗️ Architecture Overview

### Clean Architecture Layers
- **Domain Layer**: Core business logic and entities
- **Application Layer**: CQRS commands/queries and handlers
- **Infrastructure Layer**: Data access, external services
- **Presentation Layers**: Web API, Console UI, Angular SPA

### Key Design Patterns
- **Template-Driven Architecture**: Database-driven character/item configuration
- **Event Sourcing**: Character state reconstruction from event streams
- **CQRS**: Separate command/query handlers using MediatR
- **Domain Events**: Event-driven architecture with SignalR notifications

## 🛠️ Development

### Testing
```bash
# Unit tests
dotnet test tests/RpgGame.UnitTests/

# Integration tests
dotnet test tests/RpgGame.IntegrationTests/
```

### Database Migrations
```bash
# Add migration
dotnet ef migrations add MigrationName --project src/RpgGame.Infrastructure --startup-project src/RpgGame.WebApi

# Update database
dotnet ef database update --project src/RpgGame.Infrastructure --startup-project src/RpgGame.WebApi
```

### Security Scanning
```bash
# Scan repository for secrets (requires GitGuardian API key)
python -m ggshield secret scan repo .

# Authenticate with GitGuardian
python -m ggshield auth login
```

## 📁 Project Structure

```
src/
├── RpgGame.Domain/           # Core business entities and logic
├── RpgGame.Application/      # CQRS handlers and application services
├── RpgGame.Infrastructure/   # Data access and external services
├── RpgGame.WebApi/          # REST API endpoints
├── RpgGame.Presentation/    # Console application
└── RpgGame.AngularUI/       # Angular frontend
```

## 🔧 Configuration

### Required Environment Variables
- `CONNECTION_STRING_DEFAULT`: Database connection string
- `SEED_USER_PASSWORD`: Password for development user seeding

### Optional Configuration
- JWT settings in `appsettings.json` (non-sensitive parts)
- Logging configuration via NLog
- CORS settings for frontend integration

## 📄 Documentation

For detailed development guidelines, architectural decisions, and coding standards, see [CLAUDE.md](CLAUDE.md).

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes (ggshield will scan for secrets automatically)
4. Run tests to ensure everything works
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.