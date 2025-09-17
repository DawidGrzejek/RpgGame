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

## Jira Integration & Documentation Workflow

### Jira Ticket Management
**MANDATORY**: All development work is organized through Jira tickets with clear hierarchy and branch management:

#### Ticket Hierarchy Structure
1. **Parent Tasks** (e.g., RPGGAME-18): Major features or epics
2. **Child Tasks** (e.g., RPGGAME-42): Specific implementation tasks under parent
3. **Branch Naming**: Use ticket number as branch name (e.g., `rpggame-42`)

#### Development Workflow
1. **Start Work**: Create branch from parent ticket branch (e.g., `RPGGAME-18-phase-2-infrastructure-layer-repository-implement`)
2. **Track Progress**: Use Development-Backlog.md to document implementation details locally
3. **Complete Task**:
   - Push branch for review (e.g., `rpggame-42`)
   - Close child ticket with summary from Development-Backlog.md
   - Update parent ticket status accordingly

#### Documentation Requirements for Jira Integration
**MANDATORY**: All documentation agents MUST follow these requirements:

##### Current Active Tasks
- **Parent Task**: RPGGAME-18 (Phase 2 Infrastructure Layer Repository)
- **Current Child**: RPGGAME-42 (CharacterTemplateRepository implementation with tests)
- **Branch Strategy**: Child task branches created from parent task branch

##### Documentation Agents Responsibilities
1. **Track Implementation**: Document all changes in Development-Backlog.md during development
2. **Jira Integration**: When task is complete, provide summary for Jira ticket closure
3. **Branch Management**: Ensure documentation reflects proper branch and ticket relationships
4. **Quality Reporting**: Include test results, coverage, and architectural compliance

##### Content Organization
- **Implementation Details**: Document what was built and how it integrates
- **Technical Decisions**: Record architectural choices and their rationale
- **Testing Coverage**: Detail test implementation and coverage achieved
- **Integration Notes**: How changes affect existing codebase and other components

##### Maintenance Triggers for Jira Workflow
Documentation agents MUST proactively update when:
- Child task implementation is complete (prepare for Jira closure)
- Parent task milestones are reached
- Cross-ticket dependencies are resolved
- Major architectural decisions affect multiple tickets

## Agent Usage Guidelines

### Mandatory Agent Usage
**CRITICAL**: Always use the appropriate specialized agent when tasks match their expertise:

#### Required Agent Usage Scenarios
1. **testing-specialist**: MANDATORY for ALL testing tasks
   - Creating/updating unit tests, integration tests, performance tests
   - Testing event sourcing, snapshots, CQRS handlers
   - Validating DDD principles through tests
   - Any template-driven architecture testing

2. **documentation-maintainer**: MANDATORY for Jira workflow integration
   - **Jira Task Completion**: When child tasks (e.g., RPGGAME-42) are ready for closure
   - **Implementation Documentation**: Track changes in Development-Backlog.md during development
   - **Branch Preparation**: Prepare branches for review with proper documentation
   - **Parent Task Updates**: Update parent task status when milestones are reached
   - **Quality Reporting**: Document test coverage, architectural compliance, and integration notes
   - **Cross-Ticket Dependencies**: When changes affect multiple Jira tickets

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

#### Documentation Triggers for Jira Integration (Auto-Check Required):
- ✅ **Jira Task Completion**: Child task ready for closure and branch review
- ✅ **Implementation Milestones**: Significant progress on parent tasks
- ✅ **Testing Coverage**: Comprehensive test implementation for tasks
- ✅ **Architectural Integration**: How changes align with Clean Architecture/DDD
- ✅ **Cross-Task Dependencies**: Changes affecting multiple Jira tickets
- ✅ **Quality Gates**: Performance, coverage, and compliance achievements

#### Status Report Format for Jira Integration:
```markdown
## 📚 Jira Documentation Status
- ✅ **Task Ready**: Child task [TICKET-ID] documented and ready for closure
- 📋 **Progress Tracked**: Implementation details logged in Development-Backlog.md
- 🔗 **Parent Updated**: Parent task [TICKET-ID] progress documented
- ⚠️ **Review Pending**: Branch prepared for review with documentation
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