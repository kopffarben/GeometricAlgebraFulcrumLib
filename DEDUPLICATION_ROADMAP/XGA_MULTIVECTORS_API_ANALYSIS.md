# XGa Multivectors API-Unterschiede: Float64 vs Generic

**Analysiert am:** 2025-10-23
**Ziel:** Vollständiger Vergleich aller Multivector-APIs zur Identifikation fehlender Methoden in Generic-Implementierung

---

## Executive Summary

### Hauptunterschiede

1. **Operator-Überladungen:**
   - **Float64**: Hat spezifische Operatoren für `Float64Scalar` und `IFloat64Scalar`
   - **Generic**: Hat zusätzliche Operatoren für `Scalar<T>`, `T`, und `IScalar<T>`

2. **Rückgabetypen:**
   - **Float64**: Gibt primitive `double` zurück (z.B. `Scalar()` → `double`)
   - **Generic**: Gibt `Scalar<T>` zurück (z.B. `Scalar()` → `Scalar<T>`)

3. **MapScalars Methoden:**
   - **Float64**: FEHLT komplett
   - **Generic**: Hat umfangreiche `MapScalars()` Überladungen - KRITISCHER UNTERSCHIED!

4. **Utils-Klassen:**
   - **Float64**: XGaFloat64VectorUtils hat viele Konvertierungsmethoden (LinVector2D, LinVector3D, LinVector4D, MathNet.Numerics)
   - **Generic**: XGaVectorUtils ist minimal (nur OmMap Methoden)

---

## 1. XGaScalar / XGaFloat64Scalar

### Hauptklassen-Methoden (identisch)

✅ **Identische Methoden** (beide Versionen implementiert):
```csharp
// Core methods
public override bool IsValid()
public override IReadOnlyDictionary<IndexSet, T/double> GetIdScalarDictionary()
public override bool ContainsKey(IndexSet key)

// Part extraction
public override XGaScalar<T>/XGaFloat64Scalar GetScalarPart()
public override XGaVector<T>/XGaFloat64Vector GetVectorPart()
public override XGaVector<T>/XGaFloat64Vector GetVectorPart(Func<int, bool> filter)
public override XGaBivector<T>/XGaFloat64Bivector GetBivectorPart()
public override XGaHigherKVector<T>/XGaFloat64HigherKVector GetHigherKVectorPart(int grade)
public override XGaScalar<T>/XGaFloat64Scalar GetPart(Func<IndexSet, bool> filterFunc)
public override XGaScalar<T>/XGaFloat64Scalar GetPart(Func<T/double, bool> filterFunc)
public override XGaScalar<T>/XGaFloat64Scalar GetPart(Func<IndexSet, T/double, bool> filterFunc)

// Scalar access
public override bool TryGetScalarValue(out T/double scalar)
public override bool TryGetBasisBladeScalarValue(IndexSet basisBlade, out T/double scalar)

// Object methods
public override bool Equals(object? obj)
public override int GetHashCode()
public override string ToString()
```

### Unterschiedliche Signatur: Scalar()

**Float64:**
```csharp
public override double Scalar()
```

**Generic:**
```csharp
public override Scalar<T> Scalar()
```

### Unterschiedliche Signatur: GetBasisBladeScalar()

**Float64:**
```csharp
public override double GetBasisBladeScalar(IndexSet basisBladeId)
```

**Generic:**
```csharp
public override Scalar<T> GetBasisBladeScalar(IndexSet basisBladeId)
```

---

## 2. XGaScalarUnaryBinaryOps / XGaFloat64ScalarUnaryBinaryOps

### Float64-Only Operatoren

❌ **NUR in Float64** (fehlt in Generic):
```csharp
// Implicit conversion
public static implicit operator double(XGaFloat64Scalar mv)

// Division with Float64Scalar/IFloat64Scalar
public static XGaFloat64Scalar operator /(XGaFloat64Scalar s1, Float64Scalar s2)
public static XGaFloat64Scalar operator /(XGaFloat64Scalar s1, IFloat64Scalar s2)
```

### Generic-Only Operatoren

✅ **NUR in Generic** (fehlt in Float64):
```csharp
// Commented out implicit operators (not active)
// public static implicit operator T(XGaScalar<T> mv)
// public static implicit operator Scalar<T>(XGaScalar<T> mv)

// Additional scalar type operators
public static XGaScalar<T> operator +(XGaScalar<T> s1, Scalar<T> s2)
public static XGaScalar<T> operator +(Scalar<T> s1, XGaScalar<T> s2)
public static XGaScalar<T> operator +(XGaScalar<T> s1, T s2)
public static XGaScalar<T> operator +(T s1, XGaScalar<T> s2)

public static XGaScalar<T> operator -(XGaScalar<T> s1, Scalar<T> s2)
public static XGaScalar<T> operator -(Scalar<T> s1, XGaScalar<T> s2)
public static XGaScalar<T> operator -(XGaScalar<T> s1, T s2)
public static XGaScalar<T> operator -(T s1, XGaScalar<T> s2)

public static XGaScalar<T> operator *(XGaScalar<T> s1, Scalar<T> s2)
public static XGaScalar<T> operator *(Scalar<T> s1, XGaScalar<T> s2)
public static XGaScalar<T> operator *(XGaScalar<T> s1, T s2)
public static XGaScalar<T> operator *(T s1, XGaScalar<T> s2)

public static XGaScalar<T> operator /(XGaScalar<T> s1, Scalar<T> s2)
public static XGaScalar<T> operator /(Scalar<T> s1, XGaScalar<T> s2)
public static XGaScalar<T> operator /(XGaScalar<T> s1, T s2)
public static XGaScalar<T> operator /(T s1, XGaScalar<T> s2)
```

### Arithmetische Methoden (identisch in Konzept, unterschiedlich in Typen)

✅ **Beide Versionen** (Typen unterschiedlich):
```csharp
// Unary operations
public override XGaScalar<T>/XGaFloat64Scalar Negative()
public override XGaScalar<T>/XGaFloat64Scalar Reverse()
public override XGaScalar<T>/XGaFloat64Scalar GradeInvolution()
public override XGaScalar<T>/XGaFloat64Scalar CliffordConjugate()
public override XGaScalar<T>/XGaFloat64Scalar Conjugate()
public override XGaScalar<T>/XGaFloat64Scalar EInverse()
public override XGaScalar<T>/XGaFloat64Scalar Inverse()
public override XGaScalar<T>/XGaFloat64Scalar PseudoInverse()

// Binary operations
public XGaScalar<T>/XGaFloat64Scalar Add(XGaScalar<T>/XGaFloat64Scalar mv2)
public override XGaMultivector<T>/XGaFloat64Multivector Add(XGaMultivector<T>/XGaFloat64Multivector mv2)
public XGaScalar<T>/XGaFloat64Scalar Subtract(XGaScalar<T>/XGaFloat64Scalar mv2)
public override XGaMultivector<T>/XGaFloat64Multivector Subtract(XGaMultivector<T>/XGaFloat64Multivector mv2)

// Scaling
public override XGaScalar<T>/XGaFloat64Scalar Times(double scalarValue)
public override XGaScalar<T>/XGaFloat64Scalar Divide(double scalarValue)
public override XGaScalar<T>/XGaFloat64Scalar DivideByENorm()
public override XGaScalar<T>/XGaFloat64Scalar DivideByENormSquared()
public override XGaScalar<T>/XGaFloat64Scalar DivideByNorm()
public override XGaScalar<T>/XGaFloat64Scalar DivideByNormSquared()

// Norms
public override Scalar<T>/Float64Scalar ENormSquared()
public override Scalar<T>/Float64Scalar NormSquared()
public override Scalar<T>/Float64Scalar ENorm()
public override Scalar<T>/Float64Scalar Norm()
```

### Generic-Only Überladungen: Times/Divide

✅ **NUR in Generic**:
```csharp
public override XGaScalar<T> Times(int scalar)
public override XGaScalar<T> Times(T scalarValue)
public override XGaScalar<T> Times(Scalar<T> scalar)
public override XGaScalar<T> Times(IScalar<T> scalar)

public override XGaScalar<T> Divide(int scalar)
public override XGaScalar<T> Divide(T scalarValue)
public override XGaScalar<T> Divide(Scalar<T> scalar)  // ← FEHLT in Float64!
public override XGaScalar<T> Divide(IScalar<T> scalar)  // ← FEHLT in Float64!
```

❌ **Float64 hat nur:**
```csharp
public override XGaFloat64Scalar Times(double scalarValue)
public override XGaFloat64Scalar Divide(double scalarValue)
```

---

## 3. XGaVector / XGaFloat64Vector

### Hauptklassen-Methoden

✅ **Identische Struktur, unterschiedliche Typen**:
```csharp
// Properties
public IEnumerable<KeyValuePair<int, T/double>> IndexScalarPairs

// Part extraction
public override XGaVector<T>/XGaFloat64Vector GetVectorPart(Func<int, bool> filter)
```

❌ **Generic hat zusätzlich**:
```csharp
public XGaVector<T> GetVectorPart(Func<T, bool> filterFunc)
public XGaVector<T> GetVectorPart(Func<int, T, bool> filterFunc)
```

❌ **Float64 hat entsprechend**:
```csharp
public override XGaFloat64Vector GetVectorPart(Func<double, bool> filterFunc)
public override XGaFloat64Vector GetVectorPart(Func<int, double, bool> filterFunc)
```

### XGaVectorOperations (Generic) vs XGaFloat64VectorUnaryBinaryOps

### KRITISCHER UNTERSCHIED: MapScalars Methoden

❌ **NUR in Generic (FEHLT komplett in Float64!)**:
```csharp
// Map scalars with same processor
public override XGaVector<T> MapScalars(Func<T, T> scalarMapping)
public override XGaVector<T> MapScalars(Func<IndexSet, T, T> scalarMapping)

// Map scalars to Float64
public override XGaFloat64Vector MapScalars(XGaFloat64Processor processor, Func<T, double> scalarMapping)
public override XGaFloat64Vector MapScalars(XGaFloat64Processor processor, Func<IndexSet, T, double> scalarMapping)

// Map scalars to different generic type
public override XGaVector<T1> MapScalars<T1>(XGaProcessor<T1> processor, Func<T, T1> scalarMapping)
public override XGaVector<T1> MapScalars<T1>(XGaProcessor<T1> processor, Func<IndexSet, T, T1> scalarMapping)

// Vector-specific mapping
public XGaVector<T> MapScalars(Func<int, T, T> scalarMapping)  // Uses int index instead of IndexSet
public XGaVector<T> MapBasisVectors(Func<int, int> basisMapping)
public XGaVector<T> MapBasisVectors(Func<int, T, int> basisMapping)
public XGaVector<T> MapTerms(Func<int, T, KeyValuePair<int, T>> termMapping)
```

**Analyse:** Dies ist ein MASSIVER Unterschied! Die `MapScalars` Familie von Methoden ermöglicht flexible Transformationen und Typ-Konvertierungen in der Generic-Version, die in Float64 komplett fehlen.

### Operatoren (identisch in Struktur)

✅ **Beide Versionen**:
```csharp
// Unary
public static XGaVector<T>/XGaFloat64Vector operator -(XGaVector<T>/XGaFloat64Vector mv1)

// Binary vector ops
public static XGaVector<T>/XGaFloat64Vector operator +(XGaVector<T>/XGaFloat64Vector mv1, XGaVector<T>/XGaFloat64Vector mv2)
public static XGaVector<T>/XGaFloat64Vector operator -(XGaVector<T>/XGaFloat64Vector mv1, XGaVector<T>/XGaFloat64Vector mv2)

// Scalar multiplication
public static XGaVector<T>/XGaFloat64Vector operator *(XGaVector<T>/XGaFloat64Vector mv1, IntegerSign mv2)
public static XGaVector<T>/XGaFloat64Vector operator *(IntegerSign mv1, XGaVector<T>/XGaFloat64Vector mv2)
public static XGaVector<T>/XGaFloat64Vector operator *(XGaVector<T>/XGaFloat64Vector mv1, int/uint/long/ulong/float/double mv2)
public static XGaVector<T>/XGaFloat64Vector operator *(int/uint/long/ulong/float/double mv1, XGaVector<T>/XGaFloat64Vector mv2)

// Scalar division
public static XGaVector<T>/XGaFloat64Vector operator /(XGaVector<T>/XGaFloat64Vector mv1, IntegerSign mv2)
public static XGaVector<T>/XGaFloat64Vector operator /(XGaVector<T>/XGaFloat64Vector mv1, int/uint/long/ulong/float/double mv2)
```

✅ **Generic hat zusätzlich**:
```csharp
// Generic type operators
public static XGaVector<T> operator *(XGaVector<T> mv1, T mv2)
public static XGaVector<T> operator *(T mv1, XGaVector<T> mv2)
public static XGaVector<T> operator *(XGaVector<T> mv1, Scalar<T> mv2)
public static XGaVector<T> operator *(Scalar<T> mv1, XGaVector<T> mv2)
public static XGaVector<T> operator /(XGaVector<T> mv1, T mv2)
public static XGaVector<T> operator /(XGaVector<T> mv1, Scalar<T> mv2)
```

✅ **Scalar-Multivector Operatoren** (beide):
```csharp
public static XGaVector<T>/XGaFloat64Vector operator *(XGaVector<T>/XGaFloat64Vector mv1, XGaScalar<T>/XGaFloat64Scalar mv2)
public static XGaVector<T>/XGaFloat64Vector operator *(XGaScalar<T>/XGaFloat64Scalar mv1, XGaVector<T>/XGaFloat64Vector mv2)
public static XGaVector<T>/XGaFloat64Vector operator /(XGaVector<T>/XGaFloat64Vector mv1, XGaScalar<T>/XGaFloat64Scalar mv2)
```

### Arithmetische Methoden

✅ **Generic hat mehr Überladungen**:
```csharp
// Generic
public override XGaVector<T> Times(int scalar)
public override XGaVector<T> Times(double scalar)
public override XGaVector<T> Times(T scalarValue)
public override XGaVector<T> Times(Scalar<T> scalar)
public override XGaVector<T> Times(IScalar<T> scalar)

public override XGaVector<T> Divide(int scalar)
public override XGaVector<T> Divide(double scalar)
public override XGaVector<T> Divide(T scalarValue)
public override XGaVector<T> Divide(Scalar<T> scalar)
public override XGaVector<T> Divide(IScalar<T> scalar)
```

❌ **Float64 hat nur**:
```csharp
public override XGaFloat64Vector Times(double scalarValue)
public override XGaFloat64Vector Divide(double scalarValue)
```

---

## 4. XGaVectorUtils vs XGaFloat64VectorUtils

### MASSIVER Unterschied in Utility-Funktionen

❌ **Float64 hat VIELE Konvertierungs-Utilities (fehlen in Generic)**:

```csharp
// Float64-specific conversions
public static XGaFloat64Vector CreateXGaVector(this IEnumerable<double> scalarList, XGaFloat64Processor processor)
public static XGaFloat64Vector CreateUnitXGaFloat64Vector(this double angle, int index1, int index2)
public static XGaFloat64Vector CreateXGaPhasor(this double angle, double magnitude, int index1, int index2)

// LinVector2D conversions
public static XGaFloat64Vector ToXGaVector(this LinVector2D vector, XGaFloat64Processor processor)
public static LinVector2D ToLinVector2D(this XGaFloat64Vector vector)

// LinVector3D conversions
public static XGaFloat64Vector ToXGaVector(this LinVector3D vector, XGaFloat64Processor processor)
public static LinVector3D ToLinVector3D(this XGaFloat64Vector vector)

// LinVector4D conversions
public static XGaFloat64Vector ToXGaVector(this LinVector4D vector, XGaFloat64Processor processor)
public static LinVector4D ToLinVector4D(this XGaFloat64Vector vector)

// MathNet.Numerics.LinearAlgebra conversions
public static XGaFloat64Vector ToXGaVector(this Vector vector, XGaFloat64Processor processor)
public static Vector ToMathNetVector(this XGaFloat64Vector vector, int size)
```

✅ **Generic hat nur minimale Utilities**:
```csharp
// Outermorphism mapping
public static IEnumerable<XGaVector<T>> OmMap<T>(this IXGaOutermorphism<T> om, IEnumerable<XGaVector<T>> vectorsList)
public static IEnumerable<XGaVector<T>> OmMapUsing<T>(this IEnumerable<XGaVector<T>> vectorsList, IXGaOutermorphism<T> om)

// Commented out projection methods (not implemented)
```

---

## 5. XGaBivector / XGaFloat64Bivector

### Erwartet identische Pattern wie Vector

Basierend auf dem Vector-Pattern erwarte ich:

❌ **Generic fehlt wahrscheinlich:**
- LinBivector conversions (Float64-specific)
- Specific geometric constructions

✅ **Generic hat wahrscheinlich:**
- MapScalars Methoden Familie
- Mehr Überladungen für Times/Divide

---

## 6. XGaKVector / XGaFloat64KVector

### Erwartete Unterschiede (Pattern basierend auf Scalar/Vector)

❌ **Generic fehlt wahrscheinlich:**
- Float64-spezifische Konvertierungen

✅ **Generic hat wahrscheinlich:**
- MapScalars Methoden
- Generic type operator overloads

---

## 7. XGaMultivector / XGaFloat64Multivector

### Erwartete Unterschiede

Basierend auf dem Pattern der niedrigeren Typen:

✅ **Generic hat wahrscheinlich:**
- MapScalars Methoden (sehr wichtig!)
- Mehr Typ-Flexibilität

❌ **Float64 hat wahrscheinlich:**
- Spezifische Konvertierungs-Utilities
- Integration mit MathNet.Numerics

---

## 8. Composers (XGaKVectorComposer, XGaMultivectorComposer, etc.)

### Erwartet: Sehr ähnliche APIs

Die Composer-Pattern sind normalerweise sehr konsistent über beide Versionen.

**Zu überprüfen:**
- Methoden-Signaturen
- Rückgabetypen
- Builder-Pattern Konsistenz

---

## Zusammenfassung der kritischen Unterschiede

### 1. MapScalars Familie - KRITISCH! ❌

**Problem:** Float64 fehlt die gesamte MapScalars API!

**Generic hat (Float64 fehlt):**
```csharp
MapScalars(Func<T, T>)
MapScalars(Func<IndexSet, T, T>)
MapScalars<T1>(XGaProcessor<T1>, Func<T, T1>)
MapScalars(XGaFloat64Processor, Func<T, double>)
// + viele mehr...
```

**Auswirkung:** Unmöglich, Skalare in Float64-Multivektoren flexibel zu transformieren ohne Konvertierung zu Generic.

**Empfehlung:** MapScalars Methoden zu Float64 hinzufügen!

---

### 2. Operator-Überladungen Unterschiede

**Float64 hat:**
- `implicit operator double` für Scalar
- Operatoren mit `Float64Scalar`, `IFloat64Scalar`

**Generic hat:**
- Operatoren mit `T`, `Scalar<T>`, `IScalar<T>`
- KEINE impliziten Konvertierungen (auskommentiert)

**Auswirkung:** Generic ist typ-sicherer aber weniger convenient.

---

### 3. Times/Divide Überladungen

**Generic hat viel mehr Überladungen:**
```csharp
Times(int/double/T/Scalar<T>/IScalar<T>)
Divide(int/double/T/Scalar<T>/IScalar<T>)
```

**Float64 hat nur:**
```csharp
Times(double)
Divide(double)
```

**Empfehlung:** Float64 sollte mindestens `Divide(Float64Scalar)` und `Divide(IFloat64Scalar)` haben!

---

### 4. Utils-Klassen Diskrepanz - MASSIV! ❌

**Float64 hat umfangreiche Konvertierungen:**
- LinVector2D/3D/4D ↔ XGaFloat64Vector
- MathNet.Numerics.Vector ↔ XGaFloat64Vector
- Spezifische geometrische Konstruktionen (Unit vectors, Phasoren)

**Generic hat fast NICHTS:**
- Nur Outermorphism-Mapping
- Keine Konvertierungen zu Standard-Typen

**Auswirkung:** Generic ist für praktische Anwendungen deutlich weniger nutzbar!

**Empfehlung:** Generic Utils massiv erweitern ODER klar dokumentieren dass Float64 für Praxis bevorzugt wird.

---

### 5. Rückgabetyp-Unterschiede

**Float64:**
```csharp
public override double Scalar()
public override Float64Scalar ENorm()
```

**Generic:**
```csharp
public override Scalar<T> Scalar()
public override Scalar<T> ENorm()
```

**Auswirkung:** API ist nicht direkt austauschbar ohne Wrapper.

---

## Empfehlungen

### Priorität 1 (KRITISCH):

1. **Float64 braucht MapScalars API** - essentiell für flexible Transformationen
2. **Generic braucht mehr Utils** - ohne Konvertierungen zu/von Standard-Typen kaum nutzbar

### Priorität 2 (WICHTIG):

3. **Float64 Times/Divide Überladungen** erweitern um `Float64Scalar` und `IFloat64Scalar`
4. **Dokumentation** der Unterschiede verbessern

### Priorität 3 (NÜTZLICH):

5. Consistent operator overloading patterns
6. Mehr Unit-Tests für API-Equivalenz

---

## Nächste Schritte

Um diese Analyse zu vervollständigen, sollten noch analysiert werden:

- [ ] XGaHigherKVector (erwartet identisches Pattern)
- [ ] XGaGradedMultivector (erwartet identisches Pattern)
- [ ] XGaUniformMultivector (erwartet identisches Pattern)
- [ ] Alle Composer-Klassen (erwartet hohe Ähnlichkeit)
- [ ] Product operations Dateien (Gp, Op, Lcp, etc.)

**Geschätzte verbleibende Unterschiede:** 20-30 Methoden basierend auf Patterns.

