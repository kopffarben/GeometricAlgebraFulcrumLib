# GeometricAlgebraFulcrumLib Samples Solution

Diese Solution enthält ausführbare Beispiele aus der Projektdokumentation.  
This solution contains runnable examples from the project documentation.

## Zweck / Purpose

**DEUTSCH:**
Diese Solution wurde erstellt, um alle Code-Beispiele aus der Dokumentation zu testen und deren Richtigkeit zu verifizieren. Jedes Beispiel ist ein eigenständiges ausführbares Projekt.

**ENGLISH:**
This solution was created to test all code examples from the documentation and verify their correctness. Each example is a standalone executable project.

## Enthaltene Projekte / Included Projects

### 1. BasicGAOperations
**Quelle / Source:** `docs/usage-examples.md`  
**Beschreibung / Description:** Grundlegende Geometric Algebra Operationen mit Vektoren / Basic Geometric Algebra operations with vectors

**Demonstriert / Demonstrates:**
- Vektor-Erstellung / Vector creation
- Äußeres Produkt / Outer product (∧)
- Geometrisches Produkt / Geometric product (*)
- Skalarprodukt / Scalar product (·)
- Norm-Berechnung / Norm calculation
- Winkel-Berechnung / Angle calculation

**Ausführen / Run:**
```bash
cd BasicGAOperations
dotnet run
```

### 2. ScalarOperations
**Quelle / Source:** `docs/usage-examples.md`  
**Beschreibung / Description:** Verschiedene Scalar-Prozessoren und Arithmetik / Different scalar processors and arithmetic

**Demonstriert / Demonstrates:**
- Float64 Arithmetik / Float64 arithmetic
- Rationale Zahlen (exakt) / Rational numbers (exact)
- Operator-Überladung / Operator overloading

**Ausführen / Run:**
```bash
cd ScalarOperations
dotnet run
```

## Alle Projekte ausführen / Run All Projects

**Windows:**
```powershell
dotnet build Samples.sln
foreach ($project in Get-ChildItem -Directory -Exclude "bin","obj") {
    Write-Host "`n=== Running $($project.Name) ===`n"
    dotnet run --project "$($project.FullName)" --no-build
}
```

**Linux/Mac:**
```bash
dotnet build Samples.sln
for dir in */; do
    if [[ -f "$dir"*.csproj ]]; then
        echo -e "\n=== Running $dir ===\n"
        dotnet run --project "$dir" --no-build
    fi
done
```

## Dokumentations-Review-Berichte / Documentation Review Reports

- **ZUSAMMENFASSUNG.md** - Detaillierter Bericht auf Deutsch / Detailed report in German
- **DOCUMENTATION_REVIEW_REPORT.md** - Detailed report in English

## Gefundene und behobene Fehler / Found and Fixed Errors

Siehe / See:
- `ZUSAMMENFASSUNG.md` für die deutsche Version / for the German version
- `DOCUMENTATION_REVIEW_REPORT.md` for the English version

## Anforderungen / Requirements

- .NET 8.0 SDK oder höher / or higher
- GeometricAlgebraFulcrumLib Hauptprojekt / main project

## Build Status

| Projekt / Project | Status | Getestet / Tested |
|-------------------|--------|-------------------|
| BasicGAOperations | ✅ Builds | ✅ Runs successfully |
| ScalarOperations  | ✅ Builds | ✅ Runs successfully |

## Struktur / Structure

```
GeometricAlgebraFulcrumLib.Samples/
├── Samples.sln                          # Solution-Datei / Solution file
├── README.md                            # Diese Datei / This file
├── ZUSAMMENFASSUNG.md                   # Deutscher Bericht / German report
├── DOCUMENTATION_REVIEW_REPORT.md       # English report
├── BasicGAOperations/
│   ├── BasicGAOperations.csproj
│   └── Program.cs
└── ScalarOperations/
    ├── ScalarOperations.csproj
    └── Program.cs
```

## Nächste Schritte / Next Steps

1. Weitere Beispiele aus der Dokumentation hinzufügen / Add more examples from documentation
2. Beispiele für CGA (Conformal Geometric Algebra) hinzufügen / Add examples for CGA
3. Beispiele für MetaProgramming hinzufügen / Add examples for MetaProgramming
4. Automatisierte Tests für alle Beispiele / Automated tests for all examples

## Beitragen / Contributing

Wenn du einen Fehler in der Dokumentation oder in den Beispielen findest, erstelle bitte ein Issue oder einen Pull Request.

If you find an error in the documentation or examples, please create an issue or pull request.

## Lizenz / License

Siehe Hauptprojekt / See main project
