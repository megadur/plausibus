# Plausibus: eRezept-Validator

Eine .NET 10 REST-API zur Validierung elektronischer Rezepte gemäß gematik, SGB V, AMVV und BtMG.

## Features
- PZN-Validierung via ABDA-API
- Dosierungsprüfungen
- BtM-Spezialregeln
- FHIR-kompatibel

## Setup
```bash
dotnet restore
dotnet build
dotnet run
```

## CI/CD Workflows

This project includes several GitHub Actions workflows for automated testing, building, and deployment:

### Main CI/CD Pipeline (`ci-cd.yml`)
- **Triggers**: Push to main/develop, pull requests, manual dispatch
- **Features**:
  - Build and test the application
  - Security scanning with CodeQL
  - Docker image building and publishing to GitHub Container Registry
  - Deployment to staging (develop branch) and production (main branch)
  - Automated notifications

### Pull Request Validation (`pr-validation.yml`)
- **Triggers**: Pull requests to main/develop branches
- **Features**:
  - Code formatting validation
  - Security vulnerability checks
  - PR size validation
  - SonarCloud integration (if configured)

### Dependency Updates (`dependency-update.yml`)
- **Triggers**: Weekly schedule (Mondays at 9 AM UTC), manual dispatch
- **Features**:
  - Automated NuGet package updates
  - Testing after updates
  - Automatic PR creation for dependency updates

### Release Management (`release.yml`)
- **Triggers**: Version tags (v*), manual dispatch
- **Features**:
  - Multi-platform builds (Linux, Windows, macOS)
  - GitHub release creation with artifacts
  - Changelog generation

## Testing
```bash
dotnet test
```

## Docker
```bash
docker build -t erezept-validator ./ErezeptValidator
docker run -p 8080:8080 erezept-validator
```