# Suggested Commands for Development

## Windows System Commands
Since this project is developed on Windows, use PowerShell or Command Prompt for these operations:
- `dir` or `ls`: List directory contents (PowerShell supports both)
- `cd <path>`: Change directory
- `type <file>` or `cat <file>`: Display file contents (PowerShell supports both)
- `git <command>`: Git operations

## Building the Project

### Build entire solution
```powershell
dotnet build GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.sln
```

### Build specific project
```powershell
dotnet build GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.Algebra/GeometricAlgebraFulcrumLib.Algebra.csproj
```

### Build for Release
```powershell
dotnet build GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.sln --configuration Release
```

### Build for specific platform
```powershell
dotnet build GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.sln --configuration Debug --arch x64
```

## Testing

### Run all tests
```powershell
dotnet test GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.sln
```

### Run tests for specific project
```powershell
dotnet test GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.UnitTests/GeometricAlgebraFulcrumLib.UnitTests.csproj
```

### Run tests with verbose output
```powershell
dotnet test GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.sln --verbosity normal
```

### Run specific test
```powershell
dotnet test GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.UnitTests/GeometricAlgebraFulcrumLib.UnitTests.csproj --filter "FullyQualifiedName~BasisBladeTests"
```

## Running Applications

### Run specific application project
```powershell
dotnet run --project GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.Applications/GeometricAlgebraFulcrumLib.Applications.csproj
```

### Run with arguments
```powershell
dotnet run --project <project-path> -- <arguments>
```

## Cleaning and Restoring

### Clean build artifacts
```powershell
dotnet clean GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.sln
```

### Restore NuGet packages
```powershell
dotnet restore GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.sln
```

## Publishing

### Publish solution
```powershell
dotnet publish GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.sln --configuration Release
```

## Git Operations

### Check status
```powershell
git status
```

### View recent commits
```powershell
git log --oneline -10
```

### Create a commit
```powershell
git add .
git commit -m "Your commit message"
```

## Benchmarking

### Run benchmarks
```powershell
dotnet run --project GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.Benchmarks/GeometricAlgebraFulcrumLib.Benchmarks.csproj --configuration Release
```

## VS Code Tasks
The project includes VS Code tasks in `.vscode/tasks.json`:
- **build**: Build the solution
- **publish**: Publish the solution
- **watch**: Watch for changes and rebuild automatically

Run these from VS Code's Task Runner (Ctrl+Shift+B).

## Notes
- **No Linting/Formatting Tools Configured**: The project does not appear to have explicit linting or formatting tools configured. Use Visual Studio or Rider's built-in formatting (Ctrl+K, Ctrl+D) or configure tools like dotnet-format if needed.
- **Platform**: Commands assume Windows environment with PowerShell or Command Prompt
- **Path Separator**: Use backslash `\` on Windows or forward slash `/` (PowerShell handles both)
