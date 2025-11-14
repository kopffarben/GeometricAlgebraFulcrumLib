# Suggested Commands for Development (Bash-first)

The repository automation expects a Unix-like shell (Git Bash, WSL, Linux, or
macOS Terminal). Equivalent PowerShell commands are noted where it makes sense.
All `dotnet` commands are identical across shells; only the syntax for setting
environment variables differs.

## Common shell commands
- `ls -lah` / `dir`: list files
- `cd <path>`: change directory
- `cat <file>` / `type <file>`: display file contents
- `rg <pattern>` / `Select-String`: search in files
- `find . -name "*.cs"` / `Get-ChildItem -Recurse -Filter "*.cs"`

## Building the Project

All samples use Bash; replace `export VAR=value` with `$env:VAR="value"` in
PowerShell if needed.

### Build entire solution
```bash
DOTNET_ROOT="$(pwd)/.dotnet8"
$DOTNET_ROOT/dotnet build GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.sln
```

### Build specific project
```bash
$DOTNET_ROOT/dotnet build GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.Algebra/GeometricAlgebraFulcrumLib.Algebra.csproj
```

### Build for Release
```bash
$DOTNET_ROOT/dotnet build GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.sln --configuration Release
```

### Build for specific platform
```bash
$DOTNET_ROOT/dotnet build GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.sln --configuration Debug --arch x64
```

## Testing

### Run all tests
```bash
$DOTNET_ROOT/dotnet test GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.sln
```

### Run tests for specific project
```bash
$DOTNET_ROOT/dotnet test GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.UnitTests/GeometricAlgebraFulcrumLib.UnitTests.csproj
```

### Run tests with verbose output
```bash
$DOTNET_ROOT/dotnet test GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.sln --verbosity normal
```

### Run specific test
```bash
$DOTNET_ROOT/dotnet test GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.UnitTests/GeometricAlgebraFulcrumLib.UnitTests.csproj --filter "FullyQualifiedName~BasisBladeTests"
```

## Running Applications

### Run specific application project
```bash
$DOTNET_ROOT/dotnet run --project GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.Applications/GeometricAlgebraFulcrumLib.Applications.csproj
```

### Run with arguments
```bash
$DOTNET_ROOT/dotnet run --project <project-path> -- <arguments>
```

## Cleaning and Restoring

### Clean build artifacts
```bash
$DOTNET_ROOT/dotnet clean GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.sln
```

### Restore NuGet packages
```bash
$DOTNET_ROOT/dotnet restore GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.sln
```

## Publishing

### Publish solution
```bash
$DOTNET_ROOT/dotnet publish GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.sln --configuration Release
```

## Git Operations

### Check status
```bash
git status
```

### View recent commits
```bash
git log --oneline -10
```

### Create a commit
```bash
git add .
git commit -m "Your commit message"
```

## Benchmarking

### Run benchmarks
```bash
$DOTNET_ROOT/dotnet run --project GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.Benchmarks/GeometricAlgebraFulcrumLib.Benchmarks.csproj --configuration Release
```

## VS Code Tasks
The project includes VS Code tasks in `.vscode/tasks.json`:
- **build**: Build the solution
- **publish**: Publish the solution
- **watch**: Watch for changes and rebuild automatically

Run these from VS Code's Task Runner (Ctrl+Shift+B).

## Notes
- **No Linting/Formatting Tools Configured**: Use your IDE formatter (VS, Rider, VS Code) or add `dotnet format` locally.
- **Platform**: Commands assume a Bash-style shell. Swap `export VAR=value` for `$env:VAR="value"` if you prefer PowerShell.
- **Path Separator**: Stick to `/` in commands and code for portability.
