# Documentation Index

_Last updated: 2025-11-14_

This page replaces the sprawling legacy index with a short list of the files you
actually need.

## 📌 Quick Links

- `README.md` — overview, quick start, and high-level positioning.
- `docs/guides/DEVELOPMENT_GUIDE.md` — architecture + agent playbook.
- `docs/status/ISSUES_TO_FIX.md` — active build/test blockers.
- `docs/status/TODO_TEST_COVERAGE.md` — coverage roadmap.
- `docs/status/KNOWN_ISSUES_AND_SOLUTIONS.md` — accepted warnings.
- `docs/performance/*.md` — Float64 vs Generic<T> benchmark reports.
- `docs/guides/FLOAT32_COMPATIBILITY_GUIDE.md` — float32 abstraction tips.
- Documentation site: <https://kopffarben.github.io/GeometricAlgebraFulcrumLib/>

## 🗂️ Repo Landmarks

| Path | Purpose |
|------|---------|
| `GeometricAlgebraFulcrumLib/` | Solution, projects, and source code. |
| `GeometricAlgebraFulcrumLib.UnitTests/` | NUnit suites, equivalence tests, harness README. |
| `docs/` | Local documentation (status, performance, guides, archives). |
| `docs/status/archive/` | Full historical logs (old TODO/issue trackers). |
| `GeometricAlgebraFulcrumLib.Documentation/` | Material for the GitHub Pages site. |
| `assets/`, `GeometricAlgebraFulcrumLib.Visualizations/` | Shared assets and renderers. |

## 🚦 Status Snapshot

- `DOTNET_ROOT="$(pwd)/.dotnet8" DOTNET_SYSTEM_GLOBALIZATION_CULTURE=de-DE DOTNET_SYSTEM_GLOBALIZATION_UI_CULTURE=de-DE \`
  `$DOTNET_ROOT/dotnet test GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.UnitTests/GeometricAlgebraFulcrumLib.UnitTests.csproj -c Release`
  runs successfully but currently reports **24 failing tests** (pure rotor,
  adaptive sampler, trajectory equivalence, and text helper suites). Details:
  `docs/status/ISSUES_TO_FIX.md`.
- Coverage work is focused on resolving those failures before adding new CGa/PGA
  regression grids. Follow along in `docs/status/TODO_TEST_COVERAGE.md`.
- Expect nullability warnings from `ThreeJsObjectFactory.cs` and NuGet
  vulnerability-check warnings until the sandboxed CLI cache is writable; see
  `docs/status/KNOWN_ISSUES_AND_SOLUTIONS.md`.

## 🧾 Archives

Need the old, verbose reports? They moved to `docs/status/archive/` and remain
accessible through git history. Pointers:

- `docs/status/archive/ISSUES_TO_FIX.archive.md`
- `docs/status/archive/TODO_TEST_COVERAGE.archive.md`
- `docs/status/archive/KNOWN_ISSUES_AND_SOLUTIONS.archive.md`
