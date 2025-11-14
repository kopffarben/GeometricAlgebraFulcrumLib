# Repository Guidelines

## Project Structure & Module Organization
Main source lives under `GeometricAlgebraFulcrumLib/`, split into focused libraries such as `.Algebra` (core GA kernels), `.Modeling` (CGA/PGA utilities), `.MetaProgramming` (code generators), and `.Applications` (sample consoles in `Program.cs`). GPU, Matlab, and Stride integrations occupy sibling folders. Shared assets are under `assets/`, extra documentation under `docs/`, and benchmarking artifacts in `BenchmarkDotNet.Artifacts/`. Unit tests are isolated in `GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.UnitTests`, so keep new fixtures there unless a project needs specialized smoke tests beside its `.csproj`.

## Build, Test, and Development Commands
Use .NET 8 SDK everywhere. On shared machines without a global SDK, install a local copy and point the CLI at writable folders:

```bash
# Install SDK 8 into a repo-local folder (run once per machine)
curl -sSL https://dot.net/v1/dotnet-install.sh -o dotnet-install.sh
bash dotnet-install.sh --channel 8.0 --install-dir .dotnet8

# Export env vars before restore/build/test
export DOTNET_ROOT="$(pwd)/.dotnet8"
export PATH="$DOTNET_ROOT:$PATH"
export DOTNET_CLI_HOME="$(pwd)/.dotnet-cli-home"
export NUGET_HTTP_CACHE_PATH="$DOTNET_CLI_HOME/.nuget/http-cache"
export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1

# Restore/build/test using the local SDK
$DOTNET_ROOT/dotnet restore GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.sln
$DOTNET_ROOT/dotnet build   GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.sln -c Release
$DOTNET_ROOT/dotnet test    GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.UnitTests/GeometricAlgebraFulcrumLib.UnitTests.csproj -c Release
$DOTNET_ROOT/dotnet run     --project GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.Applications/GeometricAlgebraFulcrumLib.Applications.csproj --no-build
```

Tips:
- Keep the custom `DOTNET_CLI_HOME` and `NUGET_HTTP_CACHE_PATH` under the repo (or another writable path) to avoid permission issues during package restore.
- After a successful `dotnet build -c Release`, subsequent test iterations can use `dotnet test ... --no-build` to skip recompilation.
- Prefer `dotnet watch test` when iterating on dense algebra logic; it catches regressions before code-gen outputs are touched.

## Coding Style & Naming Conventions
The repo targets C# 12 with nullable reference types. Stick to file-scoped namespaces, four-space indentation, and braces on new lines as seen in `GeometricAlgebraFulcrumLib.UnitTests/AutoDiff/TermOperatorContractTests.cs`. Types, records, and enums use `PascalCase`; locals and private fields use `camelCase`, while interfaces keep the `I` prefix (e.g., `IMultivector`). Keep methods side-effect free unless clearly marked `Create`/`Build`. Run `dotnet format GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.sln` before pushing to align analyzers.

## Testing Guidelines
NUnit provides the primary surface (`[TestFixture]`, `[Test]`), with selected xUnit helpers referenced for legacy suites. Create new files named `<Feature>Tests.cs`, mirror the namespace under test, and assert numerical tolerances explicitly (e.g., `Assert.That(value, Is.EqualTo(expected).Within(1e-12))`). Long-running GPU or symbolic tests should be tagged with `[Category("Slow")]` to allow `dotnet test --filter Category!=Slow`. When altering generated code, add a regression covering both single-precision and float64 pathways where possible.

## Commit & Pull Request Guidelines
Follow the existing `type(scope): summary` style (`fix(MetaProgramming): ...`, `docs(...): ...`). Write imperative verbs, keep subjects under ~72 characters, and reference issue IDs with `Refs #123` when applicable. Pull requests must describe the change, list affected modules, note any doc/test TODOs, and attach screenshots or logs when touching visualization or benchmarking output. Coordinate large refactors by opening a draft PR linked from `KNOWN_ISSUES_AND_SOLUTIONS.md`.

## Security & Configuration Tips
The repo references ILGPU, Mathematica, and Matlab bindings; guard platform-specific code with feature flags and never hard-code licenses. Store API keys or dataset paths in user secrets, not source. When contributing new generators, record reproducible settings in `DOCUMENTATION_INDEX.md` and keep binary artifacts out of Git—use `BenchmarkDotNet.Artifacts/` or `build_output.txt` for transient data only.
