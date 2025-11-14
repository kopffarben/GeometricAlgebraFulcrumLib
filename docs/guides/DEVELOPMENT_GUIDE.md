# GA-FuL Development Guide (Agents + Humans)

_Last updated: 2025-11-14_

This document condenses the old `CLAUDE.md` playbook into a concise reference
for anyone working in the Codex/Serena toolchain.

## 1. Tooling & Onboarding

1. Always call `mcp__serena__initial_instructions` before touching the repo.
2. Run `mcp__serena__check_onboarding_performed`; if it returns `false`, follow
   `mcp__serena__onboarding`.
3. Prefer symbol-aware operations (`find_symbol`, `insert_after_symbol`, etc.)
   over raw text editing.
4. Use `context7` when third-party API docs are needed.
5. Keep `sequential thinking` enabled for every non-trivial change.

### Local Build/Test Commands

```bash
# Export sandbox-friendly .NET 8
export DOTNET_ROOT="$(pwd)/.dotnet8"
export PATH="$DOTNET_ROOT:$PATH"
export DOTNET_CLI_HOME="$(pwd)/.dotnet-cli-home"
export NUGET_HTTP_CACHE_PATH="$DOTNET_CLI_HOME/.nuget/http-cache"
export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1

$DOTNET_ROOT/dotnet restore GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.sln
$DOTNET_ROOT/dotnet build   GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.sln -c Release
$DOTNET_ROOT/dotnet test    GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.UnitTests/GeometricAlgebraFulcrumLib.UnitTests.csproj -c Release
```

Use `--no-build` after a successful Release build and prefer
`dotnet watch test --filter Modeling` when iterating on tricky algebra logic.

## 2. Architecture Cheat Sheet

```
Applications → Modeling → Algebra
                      ↘
                MetaProgramming → Utilities
```

- **Algebra**: Core GA operations, processors, multivectors, sparse storage.
- **Modeling**: CGA/PGA utilities, trajectories, visualization helpers.
- **MetaProgramming**: Symbolic code generators plus optimization passes.
- **Utilities**: Numerics, text/code generation, data-orientated helpers.

## 3. Processor & Scalar Patterns

`XGaProcessor<T>` (and the float64 specializations) are the factory +
metric + dispatcher for every multivector operation.

```csharp
var processor = XGaFloat64Processor.Euclidean;
var custom = XGaProcessor<T>.CreateEuclidean(scalarProcessor);
```

Rules of thumb:

- Never hard-code `double`. Use the provided `IScalarProcessor<T>` instead.
- Maintain strict API parity between `Float64*` classes and their generic
  counterparts; every factory/helper on the float64 side needs a matching
  generic overload that accepts `IScalarProcessor<T>`.
- When adding new generic features, write a regression test that compares the
  generic result to the float64 baseline (see the `Modeling/Trajectories`
  equivalence tests).

## 4. Composer Pattern

Multivectors are immutable. Use composers to build them efficiently:

```csharp
var mv = processor
    .CreateMultivectorComposer()
    .SetTerm(id1, scalar1)
    .AddGpTerms(mvA, mvB)
    .GetMultivector();
```

Specialized composers exist for scalars, vectors, bivectors, and arbitrary
grades; prefer them when the grade is known at compile time.

## 5. Multivector Storage

- `XGaUniformMultivector<T>` → sparse dictionary.
- `XGaGradedMultivector<T>` → grade-indexed dictionaries.
- `RGaFloat64Multivector` → dense array (fast for ≤64 dimensions).

Pick the representation that matches your sparsity/performance needs. All
storage layers implement the same high-level API, so swapping them should be
transparent.

## 6. Additional References

- Status dashboards: `docs/status/*.md`
- Performance studies: `docs/performance/`
- Float32 compatibility: `docs/guides/FLOAT32_COMPATIBILITY_GUIDE.md`
- Documentation hub: `DOCUMENTATION_INDEX.md`
