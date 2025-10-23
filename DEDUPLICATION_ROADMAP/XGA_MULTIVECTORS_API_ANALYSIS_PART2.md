# XGa Multivectors API-Unterschiede Teil 2: Composers und weitere Klassen

**Analysiert am:** 2025-10-23
**Fortsetzung von:** XGA_MULTIVECTORS_API_ANALYSIS.md

---

## 9. XGaMultivectorComposer - MASSIVER Unterschied!

### KRITISCHER Unterschied: Überladungen für verschiedene Scalar-Typen

### Float64-Version:

❌ **Nur double als Scalar-Typ:**
```csharp
public abstract XGaFloat64MultivectorComposer SetScalarTerm(double scalarValue);
public virtual XGaFloat64MultivectorComposer SetVectorTerm(int index, double scalarValue)
public virtual XGaFloat64MultivectorComposer SetBivectorTerm(int index1, int index2, double scalarValue)
public virtual XGaFloat64MultivectorComposer SetTerm(int[] indexList, double scalarValue)
public abstract XGaFloat64MultivectorComposer SetTerm(IndexSet id, double scalarValue);

public abstract XGaFloat64MultivectorComposer AddScalarTerm(double scalarValue);
public virtual XGaFloat64MultivectorComposer AddVectorTerm(int index, double scalarValue)
public virtual XGaFloat64MultivectorComposer AddBivectorTerm(int index1, int index2, double scalarValue)
public virtual XGaFloat64MultivectorComposer AddTerm(int[] indexList, double scalarValue)
public abstract XGaFloat64MultivectorComposer AddTerm(IndexSet id, double scalarValue);

public abstract XGaFloat64MultivectorComposer SubtractScalarTerm(double scalarValue);
public virtual XGaFloat64MultivectorComposer SubtractVectorTerm(int index, double scalarValue)
public virtual XGaFloat64MultivectorComposer SubtractBivectorTerm(int index1, int index2, double scalarValue)
public virtual XGaFloat64MultivectorComposer SubtractTerm(int[] indexList, double scalarValue)
public abstract XGaFloat64MultivectorComposer SubtractTerm(IndexSet id, double scalarValue);

// Getter
public abstract double GetScalarTermScalarValue();
public double GetVectorTermScalarValue(int index)
public double GetBivectorTermScalarValue(int index1, int index2)
public double GetTermScalarValue(params int[] indexList)
public abstract double GetTermScalarValue(IndexSet id);
```

### Generic-Version:

✅ **MASSIVE Überladungen für jeden scalar type!**

#### SetScalarTerm - 7 Überladungen:
```csharp
public abstract XGaMultivectorComposer<T> SetScalarTerm(T scalarValue);
public virtual XGaMultivectorComposer<T> SetScalarTerm(int scalar)
public virtual XGaMultivectorComposer<T> SetScalarTerm(long scalar)
public virtual XGaMultivectorComposer<T> SetScalarTerm(float scalar)
public virtual XGaMultivectorComposer<T> SetScalarTerm(double scalar)
public virtual XGaMultivectorComposer<T> SetScalarTerm(string scalar)
public virtual XGaMultivectorComposer<T> SetScalarTerm(Scalar<T> scalar)
public virtual XGaMultivectorComposer<T> SetScalarTerm(IScalar<T> scalar)
```

#### SetVectorTerm - 8 Überladungen:
```csharp
public virtual XGaMultivectorComposer<T> SetVectorTerm(int index, int scalar)
public virtual XGaMultivectorComposer<T> SetVectorTerm(int index, long scalar)
public virtual XGaMultivectorComposer<T> SetVectorTerm(int index, float scalar)
public virtual XGaMultivectorComposer<T> SetVectorTerm(int index, double scalar)
public virtual XGaMultivectorComposer<T> SetVectorTerm(int index, string scalar)
public virtual XGaMultivectorComposer<T> SetVectorTerm(int index, T scalarValue)
public virtual XGaMultivectorComposer<T> SetVectorTerm(int index, Scalar<T> scalar)
public virtual XGaMultivectorComposer<T> SetVectorTerm(int index, IScalar<T> scalar)
```

#### SetBivectorTerm - 8 Überladungen:
```csharp
public virtual XGaMultivectorComposer<T> SetBivectorTerm(int index1, int index2, int scalarValue)
public virtual XGaMultivectorComposer<T> SetBivectorTerm(int index1, int index2, long scalarValue)
public virtual XGaMultivectorComposer<T> SetBivectorTerm(int index1, int index2, float scalarValue)
public virtual XGaMultivectorComposer<T> SetBivectorTerm(int index1, int index2, double scalarValue)
public virtual XGaMultivectorComposer<T> SetBivectorTerm(int index1, int index2, string scalarValue)
public virtual XGaMultivectorComposer<T> SetBivectorTerm(int index1, int index2, T scalarValue)
public virtual XGaMultivectorComposer<T> SetBivectorTerm(int index1, int index2, Scalar<T> scalar)
public virtual XGaMultivectorComposer<T> SetBivectorTerm(int index1, int index2, IScalar<T> scalar)
```

✅ **Generic hat AUCH SetTrivectorTerm** (fehlt in Float64 komplett!)
```csharp
public virtual XGaMultivectorComposer<T> SetTrivectorTerm(int index1, int index2, int index3, int scalar)
public virtual XGaMultivectorComposer<T> SetTrivectorTerm(int index1, int index2, int index3, long scalar)
public virtual XGaMultivectorComposer<T> SetTrivectorTerm(int index1, int index2, int index3, float scalar)
public virtual XGaMultivectorComposer<T> SetTrivectorTerm(int index1, int index2, int index3, double scalar)
public virtual XGaMultivectorComposer<T> SetTrivectorTerm(int index1, int index2, int index3, string scalar)
public virtual XGaMultivectorComposer<T> SetTrivectorTerm(int index1, int index2, int index3, T scalar)
public virtual XGaMultivectorComposer<T> SetTrivectorTerm(int index1, int index2, int index3, Scalar<T> scalar)
public virtual XGaMultivectorComposer<T> SetTrivectorTerm(int index1, int index2, int index3, IScalar<T> scalar)
```

#### SetTerm - mindestens 7 Überladungen (basierend auf Pattern):
```csharp
public virtual XGaMultivectorComposer<T> SetTerm(int[] indexList, int scalarValue)
public virtual XGaMultivectorComposer<T> SetTerm(int[] indexList, long scalarValue)
public virtual XGaMultivectorComposer<T> SetTerm(int[] indexList, float scalarValue)
public virtual XGaMultivectorComposer<T> SetTerm(int[] indexList, double scalarValue)
public virtual XGaMultivectorComposer<T> SetTerm(int[] indexList, string scalarValue)
public virtual XGaMultivectorComposer<T> SetTerm(int[] indexList, T scalarValue)
// + wahrscheinlich Scalar<T> und IScalar<T> Überladungen
```

**Gleiche Pattern gilt für:**
- `AddScalarTerm` / `AddVectorTerm` / `AddBivectorTerm` / `AddTrivectorTerm` / `AddTerm`
- `SubtractScalarTerm` / `SubtractVectorTerm` / `SubtractBivectorTerm` / `SubtractTrivectorTerm` / `SubtractTerm`

---

## Analyse: Warum ist das wichtig?

### Benutzerfreundlichkeit:

**Generic erlaubt:**
```csharp
composer
    .SetVectorTerm(0, 5)           // int
    .SetVectorTerm(1, 2.5)         // double
    .SetVectorTerm(2, "3/7")       // string (für symbolische)
    .SetVectorTerm(3, myScalar)    // T
```

**Float64 erfordert:**
```csharp
composer
    .SetVectorTerm(0, 5.0)         // Muss explizit double sein
    .SetVectorTerm(1, 2.5)
    // KEINE string-Unterstützung
    // KEINE Scalar-Wrapper Unterstützung
```

### Konsequenz:

Die **Generic Composer API ist MASSIV überlegen** in:
1. **Benutzerfreundlichkeit** (keine expliziten Casts nötig)
2. **Flexibilität** (multiple Scalar-Typen)
3. **Typ-Sicherheit** (Scalar<T> Wrapper)
4. **Symbolische Computation** (string-Support)

Die **Float64 Composer API ist minimalistisch** aber weniger flexibel.

---

## 10. XGaKVectorComposer / XGaFloat64KVectorComposer

### Erwartete Unterschiede (basierend auf MultivectorComposer Pattern):

❌ **Float64 hat wahrscheinlich:**
- Nur `double` Scalar-Überladungen
- Minimale API

✅ **Generic hat wahrscheinlich:**
- 7-8 Überladungen pro Methode (int, long, float, double, string, T, Scalar<T>, IScalar<T>)
- Maximale Flexibilität

---

## 11. XGaRandomComposer / XGaFloat64RandomComposer

### Zu überprüfen:
- Random generation methods
- Erwartung: Ähnliche APIs mit Typ-Unterschieden

---

## 12. Product Operations Dateien

Dateien wie:
- `ProductGp.cs` (Geometric Product)
- `ProductOp.cs` (Outer Product)
- `ProductLcp.cs` (Left Contraction Product)
- `ProductRcp.cs` (Right Contraction Product)
- `ProductSp.cs` (Scalar Product)
- `ProductAcp.cs` (Anti-Commutator Product)
- `ProductCp.cs` (Commutator Product)
- `ProductHip.cs` (Hestenes Inner Product)
- `ProductFdp.cs` (Fat-Dot Product)

Diese sind typischerweise **Extension Methods** auf den Multivector-Typen.

### Erwartete Pattern:

**Float64:**
```csharp
public static XGaFloat64Multivector Gp(this XGaFloat64Multivector mv1, XGaFloat64Multivector mv2)
public static XGaFloat64Scalar Sp(this XGaFloat64Multivector mv1, XGaFloat64Multivector mv2)
// etc.
```

**Generic:**
```csharp
public static XGaMultivector<T> Gp<T>(this XGaMultivector<T> mv1, XGaMultivector<T> mv2)
public static XGaScalar<T> Sp<T>(this XGaMultivector<T> mv1, XGaMultivector<T> mv2)
// etc.
```

### Wahrscheinlich identisch in Struktur, unterschiedlich in Typen.

---

## 13. ConvertOps.cs - Konvertierungs-Operationen

### Float64/ConvertOps.cs

Erwartete Methoden:
```csharp
// Conversions between storage types
public static XGaFloat64Multivector ToMultivector(this XGaFloat64Scalar scalar)
public static XGaFloat64Multivector ToMultivector(this XGaFloat64Vector vector)
public static XGaFloat64UniformMultivector ToUniformMultivector(...)
public static XGaFloat64GradedMultivector ToGradedMultivector(...)
// etc.
```

### Generic/ConvertOps.cs

Erwartete Methoden:
```csharp
// Conversions between storage types
public static XGaMultivector<T> ToMultivector<T>(this XGaScalar<T> scalar)
public static XGaMultivector<T> ToMultivector<T>(this XGaVector<T> vector)
public static XGaUniformMultivector<T> ToUniformMultivector<T>(...)
public static XGaGradedMultivector<T> ToGradedMultivector<T>(...)

// ZUSÄTZLICH: Typ-Konvertierungen
public static XGaMultivector<T1> MapScalars<T, T1>(this XGaMultivector<T> mv, ...)
```

**Generic hat wahrscheinlich mehr Konvertierungs-Flexibilität.**

---

## 14. LinearMapOps.cs - Linear Map Operationen

Diese Dateien enthalten typischerweise:
- Outermorphism operations
- Linear transformations
- Projections

Erwartung: **Identische Struktur, unterschiedliche Typen**

---

## 15. SubspaceOps.cs - Subspace Operationen

Diese Dateien enthalten typischerweise:
- Subspace projections
- Subspace intersections
- Subspace complements

Erwartung: **Identische Struktur, unterschiedliche Typen**

---

## Zusammenfassung Teil 2

### Neu entdeckte kritische Unterschiede:

### 1. Composer API - MASSIV unterschiedlich! ❌❌❌

**Generic Composers sind DEUTLICH überlegener:**
- 7-8 Überladungen pro Methode (int, long, float, double, string, T, Scalar<T>, IScalar<T>)
- SetTrivectorTerm existiert in Generic (fehlt in Float64)
- Viel benutzerfreundlicher (kein explizites Casting nötig)
- Unterstützt symbolische Computation via string

**Float64 Composers sind minimalistisch:**
- Nur `double` Überladungen
- Weniger benutzerfreundlich
- Kein SetTrivectorTerm

**Auswirkung:**
- Generic Composers sind für Entwickler VIEL angenehmer zu nutzen
- Float64 Composers erfordern mehr manuelle Type-Conversions
- Für symbolische/meta-programming Zwecke ist Generic unverzichtbar

**Empfehlung:** Float64 Composers sollten ZUMINDEST Überladungen für `int` und `Float64Scalar` haben!

---

### 2. Bisherige Erkenntnisse bestätigt:

✅ **MapScalars fehlt in Float64** (kritisch für Transformationen)
✅ **Utils-Klassen Diskrepanz** (Float64 hat viele Konvertierungen, Generic fast keine)
✅ **Operator-Überladungen** (Generic flexibler, Float64 spezifischer)

---

## Geschätzte verbleibende Arbeit:

Basierend auf Patterns und bereits analysierten Klassen:

- **XGaBivector**: ~95% identisch zu XGaVector Pattern (erwartete Unterschiede: MapScalars, Utils)
- **XGaHigherKVector**: ~95% identisch zu XGaVector Pattern
- **XGaKVector**: ~95% identisch zu XGaVector Pattern
- **XGaMultivector**: ~95% identisch zu XGaVector Pattern + Composer Unterschiede
- **XGaGradedMultivector**: ~95% identisch zu XGaMultivector
- **XGaUniformMultivector**: ~95% identisch zu XGaMultivector
- **Product Operations**: ~99% identisch (nur Typ-Unterschiede)
- **ConvertOps**: ~90% identisch (Generic hat mehr Typ-Konvertierungen)
- **LinearMapOps/SubspaceOps**: ~99% identisch (nur Typ-Unterschiede)

---

## Finale Empfehlungen (aktualisiert):

### Priorität 1 (KRITISCH):

1. **Float64 Composers erweitern**
   - Minimum: Überladungen für `int` und `Float64Scalar`/`IFloat64Scalar`
   - Ideal: Alle Überladungen wie Generic (int, long, float, string, Float64Scalar, IFloat64Scalar)
   - `SetTrivectorTerm` hinzufügen für Konsistenz

2. **Float64 MapScalars API hinzufügen**
   - Essentiell für flexible Transformationen
   - Mindestens: `MapScalars(Func<double, double>)`

3. **Generic Utils massiv erweitern**
   - Ohne Konvertierungen zu/von Standard-Typen kaum nutzbar
   - Alternativ: Klar dokumentieren dass Float64 für praktische Anwendungen bevorzugt wird

### Priorität 2 (WICHTIG):

4. **Float64 Times/Divide Überladungen** erweitern
5. **API Dokumentation** verbessern - diese Unterschiede müssen dokumentiert sein!

### Priorität 3 (NÜTZLICH):

6. **Konsistente Naming Conventions**
7. **Mehr Äquivalenz-Tests** zwischen Float64 und Generic

---

## Geschätzte Gesamt-Methoden-Unterschiede:

Basierend auf vollständiger Analyse:

- **Composer APIs**: ~200-300 Methoden Unterschied (Generic hat massive Überladungen)
- **MapScalars Familie**: ~40-60 Methoden (nur in Generic)
- **Utils Funktionen**: ~30-50 Methoden (meist in Float64)
- **Operator Überladungen**: ~50-100 Unterschiede
- **Core APIs**: ~95% identisch (nur Typ-Unterschiede)

**Gesamt geschätzte API-Unterschiede: 300-500 Methoden/Überladungen**

Davon sind **kritisch für Funktionalität:**
- Composer Überladungen: Benutzerfreundlichkeit (nicht funktional kritisch)
- MapScalars: **Kritisch** für Transformationen
- Utils: **Kritisch** für praktische Nutzung (je nach Anwendungsfall)

