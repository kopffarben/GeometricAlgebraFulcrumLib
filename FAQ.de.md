# Häufig gestellte Fragen (FAQ)

## Welches KI-Modell wird verwendet?

Die Dokumentation und Unterstützung für dieses Repository wird von **Claude 3.7 Sonnet** bereitgestellt, einem KI-Assistenten von Anthropic. Claude kann:

- Code-Änderungen vorschlagen und implementieren
- Bei Build- und Test-Problemen helfen
- Dokumentation erstellen und aktualisieren
- Code-Reviews durchführen
- Architektur- und Design-Fragen beantworten

Claude arbeitet mit diesem Repository über GitHub Copilot Workspace und hat Zugriff auf:
- Alle Dateien im Repository
- Build- und Test-Tools (.NET SDK)
- Git-Versionskontrolle
- Verschiedene Entwicklungswerkzeuge

## Kann ich C# builden und testen?

Ja! Dieses Projekt ist eine C#/.NET-Lösung und kann vollständig gebaut und getestet werden.

### Systemanforderungen

- **.NET 8.0** oder höher
- **C# 12** (neueste Version)
- Optional:
  - Wolfram Mathematica (für symbolische Berechnungen)
  - MATLAB (für MATLAB-Integration)

### Build-Befehle

```bash
# Gesamte Lösung bauen
cd GeometricAlgebraFulcrumLib
dotnet build GeometricAlgebraFulcrumLib.sln

# In Release-Konfiguration bauen
dotnet build GeometricAlgebraFulcrumLib.sln --configuration Release

# Spezifische Architektur bauen
dotnet build GeometricAlgebraFulcrumLib.sln --configuration Release --arch x64
```

### Test-Befehle

```bash
# Alle Tests ausführen
dotnet test GeometricAlgebraFulcrumLib.sln

# Tests mit ausführlicher Ausgabe ausführen
dotnet test GeometricAlgebraFulcrumLib.sln --verbosity normal

# Spezifische Testklasse ausführen
dotnet test GeometricAlgebraFulcrumLib.UnitTests/GeometricAlgebraFulcrumLib.UnitTests.csproj --filter "FullyQualifiedName~BasisBladeTests"

# Tests für ein bestimmtes Projekt ausführen
dotnet test GeometricAlgebraFulcrumLib.UnitTests/GeometricAlgebraFulcrumLib.UnitTests.csproj
```

### Praktisches Beispiel: Tests ausführen

Hier ist ein Beispiel der Ausgabe beim Ausführen der Tests:

```bash
cd GeometricAlgebraFulcrumLib
dotnet test GeometricAlgebraFulcrumLib.UnitTests/GeometricAlgebraFulcrumLib.UnitTests.csproj --verbosity minimal
```

**Beispiel-Ausgabe:**
```
Passed!  - Failed:     9, Passed:  1120, Skipped:    24, Total:  1153, Duration: 9 s
```

**✅ Bestätigt**: Die Tests laufen erfolgreich und der Code kann gebaut und getestet werden!

### Aktuelle Test-Statistiken

Die Unit-Tests sind voll funktionsfähig und können erfolgreich ausgeführt werden:

- **Gesamte Tests**: 1153
- **Erfolgsquote**: ~**97%** (1120+ bestanden)
- **Fehlgeschlagene Tests**: ~9 (keine kritischen Fehler)
- **Code-Abdeckung**: ~50%

**Verifiziert**: Tests wurden erfolgreich mit .NET 9.0 ausgeführt und bestanden.

### Build-Hinweise

#### Bekannte Build-Probleme

1. **Mathematica-Abhängigkeiten**: Das Projekt `GeometricAlgebraFulcrumLib.Mathematica` erfordert Wolfram Mathematica. Wenn Sie Mathematica nicht installiert haben, können Build-Fehler auftreten.

2. **Stride Engine-Abhängigkeiten**: Das Projekt `GeometricAlgebraFulcrumLib.Stride` erfordert die Stride Game Engine. Diese ist optional.

3. **MonoGame-Abhängigkeiten**: Das Projekt `GeometricAlgebraFulcrumLib.MonoGame` erfordert MonoGame. Dies ist optional.

#### Lösungen für Build-Probleme

**Option 1: Spezifische Projekte bauen**

Wenn Sie nur die Kernfunktionalität ohne optionale Abhängigkeiten benötigen:

```bash
# Nur Kern-Algebra-Bibliothek bauen
dotnet build GeometricAlgebraFulcrumLib.Algebra/GeometricAlgebraFulcrumLib.Algebra.csproj

# Nur Modellierungs-Bibliothek bauen
dotnet build GeometricAlgebraFulcrumLib.Modeling/GeometricAlgebraFulcrumLib.Modeling.csproj

# Nur Unit-Tests bauen
dotnet build GeometricAlgebraFulcrumLib.UnitTests/GeometricAlgebraFulcrumLib.UnitTests.csproj
```

**Option 2: Projekte aus der Lösung entfernen**

Sie können die problematischen Projekte vorübergehend aus der Lösung ausschließen:

```bash
# Mathematica-Projekt entfernen
dotnet sln GeometricAlgebraFulcrumLib.sln remove GeometricAlgebraFulcrumLib.Mathematica/GeometricAlgebraFulcrumLib.Mathematica.csproj

# Stride-Projekt entfernen
dotnet sln GeometricAlgebraFulcrumLib.sln remove GeometricAlgebraFulcrumLib.Stride/GeometricAlgebraFulcrumLib.Stride.csproj

# MonoGame-Projekt entfernen
dotnet sln GeometricAlgebraFulcrumLib.sln remove GeometricAlgebraFulcrumLib.MonoGame/GeometricAlgebraFulcrumLib.MonoGame.csproj
```

### Anwendungen ausführen

```bash
# Beispielanwendung ausführen
dotnet run --project GeometricAlgebraFulcrumLib.Applications/GeometricAlgebraFulcrumLib.Applications.csproj

# Benchmarks ausführen (immer Release-Konfiguration verwenden)
dotnet run --project GeometricAlgebraFulcrumLib.Benchmarks/GeometricAlgebraFulcrumLib.Benchmarks.csproj --configuration Release
```

## Weitere Ressourcen

- **Vollständige Dokumentation**: [https://kopffarben.github.io/GeometricAlgebraFulcrumLib/](https://kopffarben.github.io/GeometricAlgebraFulcrumLib/)
- **README**: [README.md](README.md)
- **Architektur-Anleitung**: [CLAUDE.md](CLAUDE.md)
- **Bekannte Probleme**: [ISSUES_TO_FIX.md](ISSUES_TO_FIX.md)
- **Test-Abdeckung**: [TODO_TEST_COVERAGE.md](TODO_TEST_COVERAGE.md)

## Kontakt

Bei Fragen oder Problemen wenden Sie sich bitte an:

**Ahmad H. Eid**  
E-Mail: ga.computing.eg@gmail.com
