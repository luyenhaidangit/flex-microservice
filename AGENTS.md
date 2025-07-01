# Flex Microservice Project Guidelines

## Directory Overview

- **src/** - Main application source code in C#. Contains services, modules, web apps and shared libraries.
- **docs/** - Documentation for modules and overall architecture.
- **infrastructures/** - Infrastructure as code (Terraform scripts, deployment notes).
- **installs/** - Setup instructions and database scripts.
- **resources/** - Architecture diagrams, requirements and other design resources.
- **secrets/** - Example configuration for secrets (e.g. Oracle wallet). Do not store real credentials.
- **.github/** - Project workflow guidelines.
- **tests/** - *(currently none)* Place unit or integration tests here when added.

## Code Style Rules

- **C#**: Follow .NET naming conventions (PascalCase for classes and methods, camelCase for variables). Use `dotnet format` to apply style where possible.

## Build, Lint and Test

Run the following commands to verify the solution builds and tests pass:

```bash
dotnet format       # Apply code style
dotnet build        # Build all projects
dotnet test         # Execute unit tests
```

## Pull Request Guidelines

1. **Title**: `[Module] Short description`.
2. **Description**:
   - Motivation and summary of changes.
   - Reference related issues or tasks.
3. **Commit Message Format**: `type(scope): short message` (e.g. `feat(ordering): add order API`).

## Pre-PR Checklist

Before opening a pull request, ensure the following commands succeed:

```bash
dotnet format
dotnet build
dotnet test
```

## External Repositories

- **Frontend**: Source code for the UI resides in
  [luyenhaidangit/flex-microfrontend](https://github.com/luyenhaidangit/flex-microfrontend).
  Use this repository to retrieve frontend components and integration details.
- **Database**: Database schemas and scripts are available in the repositories
  under [luyenhaidangit](https://github.com/luyenhaidangit?tab=repositories).
  Refer to those projects when setting up or updating database resources.

