# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Essential Commands

### .NET Backend Development
```bash
# Build the entire solution
dotnet build

# Run the Web API (from src/RpgGame.WebApi)
dotnet run

# Run unit tests
dotnet test tests/RpgGame.UnitTests/

# Run integration tests  
dotnet test tests/RpgGame.IntegrationTests/

# Database migrations (from src/RpgGame.Infrastructure)
dotnet ef migrations add MigrationName --project src/RpgGame.Infrastructure --startup-project src/RpgGame.WebApi
dotnet ef database update --project src/RpgGame.Infrastructure --startup-project src/RpgGame.WebApi
```

### Angular Frontend Development
```bash
# Navigate to Angular project
cd src/RpgGame.AngularUI/rpg-game-ui

# Install dependencies
npm install

# Start development server
npm start
# or
ng serve

# Build for production
npm run build
# or
ng build

# Run unit tests
npm test
# or
ng test
```

### Running the Full Application
1. **Set up environment variables**: Copy `.env.example` to `.env` and configure your database connection and seed password
2. Start the .NET Web API: `dotnet run` from `src/RpgGame.WebApi`
3. Start the Angular UI: `npm start` from `src/RpgGame.AngularUI/rpg-game-ui`
4. API runs on `https://localhost:7000`
5. Angular app runs on `http://localhost:4200`

## Architecture Overview

This is a comprehensive RPG game system built using **Clean Architecture** with the following layers:

### Core Architecture
- **Domain Layer** (`RpgGame.Domain`): Core business logic, entities, and domain events
- **Application Layer** (`RpgGame.Application`): CQRS commands/queries, handlers, and application services  
- **Infrastructure Layer** (`RpgGame.Infrastructure`): Data persistence, event store, external services
- **Presentation Layers**:
  - **Web API** (`RpgGame.WebApi`): REST API with JWT authentication
  - **Console UI** (`RpgGame.Presentation`): Text-based interface
  - **Angular UI** (`RpgGame.AngularUI`): Modern SPA frontend

### Key Design Patterns Implemented
- **Template-Driven Architecture**: Single `Character` entity with database-driven templates instead of inheritance hierarchy
- **Event Sourcing**: Character state reconstructed from event streams
- **CQRS**: Separate command/query handlers using MediatR
- **Domain Events**: Event-driven architecture with SignalR real-time notifications
- **Factory Pattern**: `CharacterFactory` creates characters from templates

### Template System (Revolutionary Feature)
The system uses a **template-driven approach** where:
- Single `Character` entity supports all types (Players, NPCs, Enemies) via composition
- `CharacterTemplate` and `AbilityTemplate` entities store configuration in database
- Content creators can add unlimited character types without code changes
- Eliminates complex inheritance hierarchy in favor of data-driven design

## Database Configuration

### SQL Server Setup
- **Technology**: Entity Framework Core with SQL Server
- **Development**: Local SQL Server or connection to MonsterASP.NET hosting
- **Migrations**: Located in `src/RpgGame.Infrastructure/Migrations/`

### Authentication System
- **Framework**: ASP.NET Identity with JWT tokens
- **Default Development User**:
  - Username: `GameMaster`
  - Email: `gamemaster@rpggame.local`
  - Password: `GameMaster123!`
  - Roles: GameMaster, Admin (full access)

### Database Contexts
- **GameDbContext**: Game data (characters, templates, saves)
- **IdentityDataContext**: User authentication and roles

## Key Technologies

### Backend Stack
- **.NET 9**: Primary framework
- **Entity Framework Core**: ORM with SQL Server
- **MediatR**: CQRS implementation
- **AutoMapper**: Object mapping
- **SignalR**: Real-time communication
- **Swagger**: API documentation
- **NLog**: Logging
- **xUnit**: Unit testing framework

### Frontend Stack
- **Angular 19**: Frontend framework
- **TypeScript**: Type-safe JavaScript
- **RxJS**: Reactive programming
- **Angular Material** (planned): UI components

## Project Structure

```
src/
├── RpgGame.Domain/           # Core business entities and logic
│   ├── Entities/            # Domain entities (Character, Templates)
│   ├── ValueObjects/        # Immutable objects (CharacterStats)
│   ├── Events/              # Domain events
│   ├── Factories/           # Creation patterns
│   └── Interfaces/          # Domain contracts
├── RpgGame.Application/     # Application orchestration
│   ├── Commands/            # Write operations (CQRS)
│   ├── Queries/             # Read operations (CQRS)
│   ├── Handlers/            # Command/Query handlers
│   └── Services/            # Application services
├── RpgGame.Infrastructure/  # External concerns
│   ├── Persistence/         # Data access layer
│   ├── Services/            # External services
│   └── Migrations/          # EF Core migrations
├── RpgGame.WebApi/         # REST API layer
├── RpgGame.Presentation/   # Console application
└── RpgGame.AngularUI/      # Angular frontend
```

## Environment Configuration

### Required Environment Variables
The application requires these environment variables for secure operation:

- `CONNECTION_STRING_DEFAULT`: Database connection string
- `SEED_USER_PASSWORD`: Password for default GameMaster user seeding

### Setup Instructions
1. Copy `.env.example` to `.env` in the project root
2. Update the values in `.env` with your actual credentials
3. The `.env` file is automatically loaded on application startup
4. Never commit `.env` files to version control (they are gitignored)

### Fallback Behavior
- If `CONNECTION_STRING_DEFAULT` is not set, uses LocalDB connection from `appsettings.Development.json`
- If `SEED_USER_PASSWORD` is not set, application throws an error during seeding to prevent insecure defaults

## Development Workflow

### Character System Development
The system uses a **unified Character entity** instead of separate classes. When working with characters:
- All character types use the same `Character` class
- Templates (`CharacterTemplate`) define character configurations
- Use `CharacterFactory.CreateFromTemplateAsync()` for template-based creation
- Character behavior is determined by `CharacterType` and `NPCBehavior` enums

### Adding New Features
1. **Domain First**: Add entities/value objects to `RpgGame.Domain`
2. **Application Layer**: Create commands/queries and handlers
3. **Infrastructure**: Implement repositories and data configurations
4. **API Layer**: Add controllers and DTOs
5. **Frontend**: Create Angular components and services

### Event-Driven Development
- Domain entities raise events for significant business operations
- Event handlers in Application layer process cross-cutting concerns
- Events are persisted for audit trail and event sourcing
- SignalR broadcasts real-time notifications to connected clients

## Architectural Principles & Guidelines

This codebase strictly follows these core principles. **Always adhere to these when making changes:**

### 🏛️ Domain-Driven Design (DDD)
- **Entities**: Have identity, mutable state, business behavior (e.g., `Character`)
  - Handle business actions that can raise domain events
  - Example: `Character.TakeDamage()` - business action with side effects
- **Value Objects**: Immutable, no identity, pure calculations (e.g., `CharacterStats`)
  - Only pure functions with no side effects
  - Example: `CharacterStats.IsAlive` - pure calculation, not a business action
- **Aggregates**: Consistency boundaries for business operations
- **Domain Events**: Represent significant business occurrences

### ⚡ Event-Driven Architecture
- **Domain Events**: All significant business operations raise events
- **Event Sourcing**: Character state reconstructed from event streams
- **Event Handlers**: Process cross-cutting concerns (notifications, projections)
- **Event Store**: Persistent audit trail of all domain changes

### 🎯 Template-Driven Design
- **Configuration over Code**: Use database templates instead of inheritance
- **Single Entity Pattern**: Unified entities (`Character`, `Item`) vs multiple classes
- **Data-Driven Behavior**: Abilities, items, characters defined in database
- **Content Creator Independence**: Game designers work without developers
- **Template Consolidation**: No separate `EnemyTemplate` - use `CharacterTemplate` with `CharacterType.NPC`
- **Unified Item System**: Single `Item` entity uses `ItemTemplate` for all item types

### 🧱 Clean Architecture
- **Dependency Inversion**: Dependencies point inward toward domain
- **Layer Separation**: Domain → Application → Infrastructure → Presentation
- **Interface Segregation**: Small, focused interfaces per concern
- **Single Responsibility**: Each class has one reason to change

### ⚙️ SOLID Principles
- **S**ingle Responsibility: Each class has one job
- **O**pen/Closed: Extend via composition/templates, not inheritance
- **L**iskov Substitution: Subtypes must be substitutable
- **I**nterface Segregation: Many specific interfaces vs few general ones
- **D**ependency Inversion: Depend on abstractions, not concretions

### 🎨 Clean Code Practices
- **Meaningful Names**: Express intent clearly
- **Small Functions**: Do one thing well
- **No Side Effects**: Pure functions when possible
- **Immutable Value Objects**: Use records, avoid mutation
- **Fail Fast**: Validate early, throw meaningful exceptions

### 📐 CQRS Pattern
- **Commands**: Write operations that modify state
- **Queries**: Read operations that return data
- **Separation**: Different models for reads vs writes
- **Pipeline Behaviors**: Cross-cutting concerns (validation, logging)

## Important Notes

### DDD Boundary Guidelines
**Value Objects (CharacterStats)**:
```csharp
// ✅ DO - Pure calculations
public bool IsAlive => CurrentHealth > 0;
public CharacterStats WithHealth(int health);

// ❌ DON'T - Business actions belong in entities
public void TakeDamage(int damage); // Business action!
public void Heal(int amount);       // Business action!
```

**Entities (Character)**:
```csharp
// ✅ DO - Business behaviors with side effects
public void TakeDamage(int damage) { /* Logic + Events */ }
public void Heal(int amount) { /* Logic + Events */ }
public bool IsAlive => Stats.IsAlive; // Delegate to value object
```

### Template System Benefits
- **Infinite Scalability**: Add character/item types via database without code deployment
- **Content Creator Friendly**: Game designers create content independently
- **Performance**: Single code path, efficient memory usage
- **Maintainability**: Eliminates complex inheritance hierarchies
- **Unified Architecture**: Characters and Items use same template-driven pattern
- **No Code Duplication**: Single `Character` handles players/NPCs, single `Item` handles all item types

### Authentication & Authorization
- JWT-based authentication with refresh tokens
- Role-based access control (Player, Moderator, Admin, GameMaster)
- Enhanced email validation requiring proper domain format
- Automatic user/role seeding in development environment

### Testing Strategy
- Unit tests use xUnit with Moq for mocking
- Integration tests with in-memory database
- Test projects follow naming convention: `RpgGame.[LayerName]Tests`
- **Test Value Objects**: Focus on pure calculations and immutability
- **Test Entities**: Focus on business behaviors and event generation

## Documentation Maintenance Guidelines

### Critical Documentation Requirements
**MANDATORY**: All documentation agents MUST follow these requirements when updating project documentation:

#### Timestamp Requirements
1. **Development-Backlog.md**:
   - Update "Last Updated" field with current date and time: `YYYY-MM-DD HH:MM`
   - Each new task/completion MUST have a dated header: `## ✅ COMPLETED - YYYY-MM-DD HH:MM` or `## 📋 TASKS - YYYY-MM-DD HH:MM`
   - Individual tasks MUST include both Created and Completed timestamps

2. **Readme.md**:
   - Update "Last Updated" field in Recent Updates section with current date and time
   - Each update entry MUST include date: `### Title - YYYY-MM-DD HH:MM`
   - Group related changes under single dated section

#### Content Organization
- **Separate sections by date**: Each development session gets its own dated section
- **Clear hierarchical structure**: Use consistent header levels and formatting  
- **Descriptive change summaries**: Include what was changed, why, and impact
- **Technical details**: Document architectural decisions and their rationale

#### Maintenance Triggers
Documentation agents MUST proactively update documentation when:
- New domain entities or services are created
- Architectural patterns are changed or enhanced  
- Build errors are resolved with significant changes
- Major features are completed or refactored
- Clean Architecture or DDD boundaries are modified

#### Quality Standards
- **Accurate timestamps**: Use current date/time for all updates
- **Comprehensive coverage**: Document all significant changes
- **Clear navigation**: Enable easy scrolling and finding of specific changes
- **Professional formatting**: Consistent markdown, proper emoji usage, clear structure

## Agent Usage Guidelines

### Mandatory Agent Usage
**CRITICAL**: Always use the appropriate specialized agent when tasks match their expertise:

#### Required Agent Usage Scenarios
1. **testing-specialist**: MANDATORY for ALL testing tasks
   - Creating/updating unit tests, integration tests, performance tests
   - Testing event sourcing, snapshots, CQRS handlers
   - Validating DDD principles through tests
   - Any template-driven architecture testing

2. **documentation-maintainer**: MANDATORY after significant changes
   - New features, entities, services added
   - Architectural refactoring or Clean Architecture changes
   - Major bug fixes or build error resolutions
   - Development milestone completions
   - **NEW**: Comprehensive test suite additions (>20 new tests)
   - **NEW**: Quality milestones achieved (e.g., 100+ tests passing)
   - **NEW**: Performance testing or major code coverage improvements

3. **architect**: MANDATORY for architecture decisions
   - Adding new entities, design patterns, system structure changes
   - Technology choices and architectural refactoring
   - Clean Architecture, DDD, CQRS, Event Sourcing decisions
   - Cross-cutting concerns and structural modifications

4. **angular-ui-developer**: For frontend development
   - Angular components, services, reactive programming
   - UI/UX improvements and modern web patterns

### Agent Usage Best Practices
- **Use multiple agents concurrently** when possible for optimal performance
- **Provide detailed task descriptions** since agent invocations are stateless
- **Specify expected outputs** clearly for each agent
- **Trust agent outputs** and incorporate their recommendations

### Proactive Documentation Protocol
**CRITICAL**: After completing any significant work, I must AUTOMATICALLY:

1. **Assess Documentation Impact**: Determine if the work meets any documentation triggers
2. **Use documentation-maintainer**: If work qualifies, proactively use the agent
3. **Provide Status Update**: Always report documentation status to user

#### Documentation Triggers (Auto-Check Required):
- ✅ Major code changes (new entities, services, components)
- ✅ Comprehensive testing work (>20 new tests, quality milestones)
- ✅ Architectural changes or refactoring completion  
- ✅ Feature implementation or API changes
- ✅ Build fixes or performance improvements
- ✅ Development milestones achieved

#### Status Report Format:
```markdown
## 📚 Documentation Status
- ✅ **Updated**: Documentation has been updated to reflect recent changes
- ℹ️ **Current**: Documentation is up-to-date, no changes needed
- ⚠️ **Pending**: Significant changes made, updating documentation now...
```

**No More Reminders Needed**: I will proactively check and update documentation after every significant development task.

## File Change Documentation

### Required File Change Summary
**MANDATORY**: At the end of any development session that modifies files, provide a simple list format:

```
## Files Modified:
- `file/path/name.ext`, created
- `file/path/name.ext`, edited  
- `file/path/name.ext`, removed
```

### Change Documentation Standards
- **List all files** that were created, modified, or removed
- **Use relative paths** from project root when possible
- **Simple action verbs**: created, edited, removed
- **Chronological order** when multiple change sessions occur