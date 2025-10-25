# Post-Phase-2 Test-Strategie

**Erstellt:** 2025-10-25
**Status:** 🎯 Validierungs-Plan für Phase 2 Thin Wrapper Migration

---

## 🎯 Problem Statement

**Aktuell:**
- Float64-spezifische Tests: Testen nur `LinFloat64Vector3D`, etc.
- Equivalence Tests: Vergleichen `LinFloat64Vector3D` **vs** `LinVector3D<double>`

**Nach Phase 2 (Float64 = Wrapper um Generic):**
- ❌ Equivalence Tests werden **nutzlos**: Sie testen dieselbe Implementation!
- ❌ Float64-Tests decken Generic<double> nicht ab

**Lösung:**
1. **Float64 Tests erweitern:** Beide Implementierungen testen (Float64 + Generic<double>)
2. **Equivalence Tests umstellen:** Generic<double> via verschiedene Methoden testen

---

## 📊 Test-Inventar (Aktuelle Situation)

### Float64-spezifische Tests (10 Dateien, ~100 Tests):

| Datei | Tests | Zeilen | Status |
|-------|-------|--------|--------|
| `LinFloat64Vector2DTests.cs` | ~8 | ~150 | ⚠️ Nur Float64 |
| `LinFloat64Vector3DTests.cs` | 10 | 200 | ⚠️ Nur Float64 |
| `LinFloat64BivectorTests.cs` | ~8 | ~180 | ⚠️ Nur Float64 |
| `LinFloat64QuaternionTests.cs` | ~12 | ~250 | ⚠️ Nur Float64 |
| `LinFloat64QuaternionSystemNumericsTests.cs` | ~5 | ~100 | ⚠️ Nur Float64 |
| `LinFloat64AngleTests.cs` | ~6 | ~120 | ⚠️ Nur Float64 |
| `Modeling/.../LinFloat64VectorTests.cs` | ~10 | ~200 | ⚠️ Nur Float64 |
| `Modeling/.../LinFloat64BivectorTests.cs` | ~8 | ~180 | ⚠️ Nur Float64 |
| `Modeling/.../LinFloat64QuaternionTests.cs` | ~10 | ~200 | ⚠️ Nur Float64 |
| `Float64SignalInterpolationTests.cs` | ~15 | ~300 | ⚠️ Nur Float64 |

**Gesamt:** ~100 Tests, ~1880 LOC

### Equivalence Tests (15 Dateien, 260 Tests):

| Datei | Tests | Status |
|-------|-------|--------|
| `LinVector2DEquivalenceTests.cs` | ~20 | ✅ 100% passing |
| `LinVector3DEquivalenceTests.cs` | ~20 | ✅ 100% passing |
| `LinBivectorEquivalenceTests.cs` | ~18 | ✅ 100% passing |
| `LinQuaternionEquivalenceTests.cs` | ~25 | ✅ 100% passing |
| `ComplexNumberEquivalenceTests.cs` | ~15 | ✅ 100% passing |
| `VGaEquivalenceTests.cs` | ~10 | ⚠️ 5 failing (VGa2D Bug) |
| `XGaComposerEquivalenceTests.cs` | ~30 | ✅ 100% passing |
| `XGaMapBasisEquivalenceTests.cs` | ~20 | ✅ 100% passing |
| `XGaMapScalarsEquivalenceTests.cs` | ~20 | ✅ 100% passing |
| `XGaMapTermsEquivalenceTests.cs` | ~20 | ✅ 100% passing |
| `XGaOutermorphismComposerUtilsEquivalenceTests.cs` | ~15 | ✅ 100% passing |
| `XGaGramSchmidtFrameEquivalenceTests.cs` | ~12 | ✅ 100% passing |
| `XGaConformalComposerUtilsEquivalenceTests.cs` | ~15 | ✅ 100% passing |
| `XGaAngleVectorEquivalenceTests.cs` | ~10 | ✅ 100% passing |
| `XGaComputedOutermorphismEquivalenceTests.cs` | ~10 | ✅ 100% passing |

**Gesamt:** 260 Tests, 255 passing (98.1%)

---

## 🔄 Migrations-Strategie

### Strategie 1: Float64 Tests erweitern (Empfohlen: Hoch)

**Ziel:** Beide Implementierungen in denselben Tests validieren

**Vorher:**
```csharp
[Test]
public void VectorAddition_ShouldAddComponents()
{
    // Arrange
    var v1 = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);
    var v2 = LinFloat64Vector3D.Create(4.0, 5.0, 6.0);

    // Act
    var result = v1 + v2;

    // Assert
    Assert.That(result.X.ScalarValue, Is.EqualTo(5.0).Within(Tolerance));
}
```

**Nachher (Parametrisiert):**
```csharp
[TestCase(false, Description = "Float64 Implementation")]
[TestCase(true, Description = "Generic<double> Implementation")]
public void VectorAddition_ShouldAddComponents(bool useGeneric)
{
    // Arrange
    if (!useGeneric)
    {
        var v1 = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);
        var v2 = LinFloat64Vector3D.Create(4.0, 5.0, 6.0);

        // Act
        var result = v1 + v2;

        // Assert
        Assert.That(result.X.ScalarValue, Is.EqualTo(5.0).Within(Tolerance));
    }
    else
    {
        var processor = ScalarProcessorOfFloat64.Instance;
        var v1 = LinVector3D<double>.Create(
            processor.ScalarFromNumber(1.0),
            processor.ScalarFromNumber(2.0),
            processor.ScalarFromNumber(3.0)
        );
        var v2 = LinVector3D<double>.Create(
            processor.ScalarFromNumber(4.0),
            processor.ScalarFromNumber(5.0),
            processor.ScalarFromNumber(6.0)
        );

        // Act
        var result = v1 + v2;

        // Assert
        Assert.That(result.X.ScalarValue, Is.EqualTo(5.0).Within(Tolerance));
    }
}
```

**Alternative (Cleaner mit Hilfsmethoden):**
```csharp
[TestCase(false, Description = "Float64 Implementation")]
[TestCase(true, Description = "Generic<double> Implementation")]
public void VectorAddition_ShouldAddComponents(bool useGeneric)
{
    // Arrange
    var v1 = CreateVector(1.0, 2.0, 3.0, useGeneric);
    var v2 = CreateVector(4.0, 5.0, 6.0, useGeneric);

    // Act
    var result = AddVectors(v1, v2);

    // Assert
    Assert.That(GetX(result), Is.EqualTo(5.0).Within(Tolerance));
    Assert.That(GetY(result), Is.EqualTo(7.0).Within(Tolerance));
    Assert.That(GetZ(result), Is.EqualTo(9.0).Within(Tolerance));
}

private object CreateVector(double x, double y, double z, bool useGeneric)
{
    if (!useGeneric)
        return LinFloat64Vector3D.Create(x, y, z);

    var processor = ScalarProcessorOfFloat64.Instance;
    return LinVector3D<double>.Create(
        processor.ScalarFromNumber(x),
        processor.ScalarFromNumber(y),
        processor.ScalarFromNumber(z)
    );
}

private object AddVectors(object v1, object v2)
{
    if (v1 is LinFloat64Vector3D f64V1 && v2 is LinFloat64Vector3D f64V2)
        return f64V1 + f64V2;

    var gV1 = (LinVector3D<double>)v1;
    var gV2 = (LinVector3D<double>)v2;
    return gV1 + gV2;
}

private double GetX(object vector)
{
    return vector is LinFloat64Vector3D f64V
        ? f64V.X.ScalarValue
        : ((LinVector3D<double>)vector).X.ScalarValue;
}
```

**Impact:**
- ✅ **100 Tests → 200 Tests** (doppelte Coverage)
- ✅ Generic<double> wird validiert
- ✅ Nach Phase 2: Float64 Wrapper wird getestet
- ⚠️ Aufwand: ~4-6 Stunden für alle 10 Dateien

---

### Strategie 2: Equivalence Tests umstellen (Empfohlen: Mittel)

**Ziel:** Verschiedene Code-Pfade derselben Implementation testen

**Problem:** Nach Phase 2 testet `Float64 vs Generic<double>` dasselbe!

**Lösung:** Teste `Generic<double>` via **verschiedene Methoden**

**Vorher (nutzlos nach Phase 2):**
```csharp
[Test]
public void CreateVector_ShouldHaveIdenticalComponents()
{
    // Arrange
    double x = 2.0, y = 3.0, z = 4.0;

    // Act
    var float64Vector = LinFloat64Vector3D.Create(x, y, z);
    var genericVector = LinVector3D<double>.Create(
        _scalarProcessor.ScalarFromNumber(x),
        _scalarProcessor.ScalarFromNumber(y),
        _scalarProcessor.ScalarFromNumber(z)
    );

    // Assert - BEIDE verwenden dieselbe Implementation!
    Assert.That(genericVector.X.ScalarValue, Is.EqualTo(float64Vector.X.ScalarValue));
}
```

**Nachher (testet verschiedene Code-Pfade):**
```csharp
[Test]
public void CreateVector_DirectVsComposition_ShouldProduceSameResult()
{
    // Arrange
    double x = 2.0, y = 3.0, z = 4.0;

    // Act - Methode A: Direkte Konstruktion
    var directVector = LinVector3D<double>.Create(
        _scalarProcessor.ScalarFromNumber(x),
        _scalarProcessor.ScalarFromNumber(y),
        _scalarProcessor.ScalarFromNumber(z)
    );

    // Act - Methode B: Via Basis-Vektoren (anderer Code-Pfad!)
    var composedVector =
        LinVector3D<double>.E1(_scalarProcessor) * x +
        LinVector3D<double>.E2(_scalarProcessor) * y +
        LinVector3D<double>.E3(_scalarProcessor) * z;

    // Assert - VERSCHIEDENE Code-Pfade, gleiches Ergebnis!
    Assert.That(composedVector.X.ScalarValue, Is.EqualTo(directVector.X.ScalarValue).Within(Tolerance));
    Assert.That(composedVector.Y.ScalarValue, Is.EqualTo(directVector.Y.ScalarValue).Within(Tolerance));
    Assert.That(composedVector.Z.ScalarValue, Is.EqualTo(directVector.Z.ScalarValue).Within(Tolerance));
}
```

**Weitere Beispiele für verschiedene Code-Pfade:**
```csharp
// Test 1: Create() vs FromArray()
var v1 = LinVector3D<double>.Create(x, y, z);
var v2 = LinVector3D<double>.FromArray(new[] { x, y, z });

// Test 2: Normalisierung via Divide vs ToUnit
var v3 = vector / vector.Norm();
var v4 = vector.ToUnitLinVector3D();

// Test 3: Cross-Product: Manual vs Method
var cross1 = v1.VectorCross(v2);
var cross2 = ManualCrossProduct(v1, v2);

// Test 4: Rotation: Via Quaternion vs Matrix
var rot1 = RotateViaQuaternion(vector, angle, axis);
var rot2 = RotateViaMatrix(vector, angle, axis);
```

**Impact:**
- ✅ Tests bleiben **nach Phase 2 nützlich**
- ✅ Testen verschiedene Code-Pfade → bessere Coverage
- ✅ Finden Inkonsistenzen zwischen Methoden
- ⚠️ Aufwand: ~6-8 Stunden für alle 15 Dateien
- ⚠️ Kreativität nötig: Verschiedene Methoden finden

---

## 📋 Prioritäten & Empfehlungen

### Phase 2A - Kritische Tests erweitern (Empfohlen: JA)

**Priorität: P0 - Vor Phase 2 Migration**

Erweitere folgende Float64-Tests auf Float64 + Generic<double>:

1. ✅ `LinVector3DTests` (10 Tests) - Geometrie-Kern
2. ✅ `LinVector2DTests` (8 Tests) - Geometrie-Kern
3. ✅ `LinQuaternionTests` (12 Tests) - Rotations-Kern
4. ✅ `LinBivectorTests` (8 Tests) - GA-Kern

**Geschätzter Aufwand:** ~3-4 Stunden
**Nutzen:** Kritische APIs doppelt getestet

### Phase 2B - Equivalence Tests umstellen (Empfohlen: TEILWEISE)

**Priorität: P1 - Für langfristige Wartbarkeit**

Stelle folgende Equivalence Tests um:

1. ✅ `LinVector3DEquivalenceTests` → Via verschiedene Methoden
2. ✅ `LinQuaternionEquivalenceTests` → Via verschiedene Methoden
3. ⏭️ Rest: Behalten als Regression-Tests (werden nach Phase 2 weniger wertvoll)

**Geschätzter Aufwand:** ~2-3 Stunden
**Nutzen:** Tests bleiben nach Phase 2 nützlich

### Phase 2C - Erweiterte Tests (Optional)

**Priorität: P2 - Nice-to-have**

- Signal Interpolation Tests erweitern
- Modeling-spezifische Tests erweitern

---

## 🎯 Empfohlener Aktionsplan

### Option A: Minimal (Empfohlen für schnelle Migration)

**Aufwand:** ~3 Stunden
**Coverage:** Kritische APIs

```
1. Erweitere LinVector3DTests auf Float64 + Generic<double>
2. Erweitere LinQuaternionTests auf Float64 + Generic<double>
3. Behalte Equivalence Tests as-is (Regression)
```

**Ergebnis nach Phase 2:**
- ✅ Kritische Geometrie-APIs: 2x getestet
- ⚠️ Equivalence Tests: Weniger wertvoll, aber als Regression OK
- ✅ Gesamte Test-Suite: Weiterhin ~1460+ Tests passing

### Option B: Umfassend (Empfohlen für maximale Qualität)

**Aufwand:** ~8-10 Stunden
**Coverage:** Vollständige Absicherung

```
1. Erweitere alle LinVector/LinQuaternion/LinBivector Tests (4 Dateien)
2. Stelle 2-3 Equivalence Tests um (verschiedene Code-Pfade)
3. Dokumentiere Test-Strategie für zukünftige Module
```

**Ergebnis nach Phase 2:**
- ✅ Alle kritischen APIs: 2x getestet (Float64 Wrapper + Generic<double>)
- ✅ Equivalence Tests: Testen verschiedene Code-Pfade
- ✅ Template für zukünftige Test-Erweiterungen

### Option C: Maximal (Nur bei verfügbarer Zeit)

**Aufwand:** ~15-20 Stunden
**Coverage:** Absolute Perfektion

```
1. Erweitere ALLE 10 Float64-Test-Dateien
2. Stelle ALLE 15 Equivalence Tests um
3. Füge neue Cross-Implementation Tests hinzu
```

---

## 📊 Beispiel-Code Templates

### Template 1: Parametrisierte Float64/Generic Tests

```csharp
[TestFixture]
public class LinVector3DHybridTests
{
    private const double Tolerance = 1e-10;
    private IScalarProcessor<double> _scalarProcessor = null!;

    [SetUp]
    public void Setup()
    {
        _scalarProcessor = ScalarProcessorOfFloat64.Instance;
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void VectorAddition_ShouldAddComponents(bool useGeneric)
    {
        // Arrange
        var v1 = CreateVector(1.0, 2.0, 3.0, useGeneric);
        var v2 = CreateVector(4.0, 5.0, 6.0, useGeneric);

        // Act
        var result = useGeneric
            ? ((LinVector3D<double>)v1) + ((LinVector3D<double>)v2)
            : ((LinFloat64Vector3D)v1) + ((LinFloat64Vector3D)v2);

        // Assert
        var x = useGeneric
            ? ((LinVector3D<double>)result).X.ScalarValue
            : ((LinFloat64Vector3D)result).X.ScalarValue;

        Assert.That(x, Is.EqualTo(5.0).Within(Tolerance));
    }

    private object CreateVector(double x, double y, double z, bool useGeneric)
    {
        return useGeneric
            ? LinVector3D<double>.Create(
                _scalarProcessor.ScalarFromNumber(x),
                _scalarProcessor.ScalarFromNumber(y),
                _scalarProcessor.ScalarFromNumber(z))
            : LinFloat64Vector3D.Create(x, y, z);
    }
}
```

### Template 2: Same-Literal Equivalence Tests

```csharp
[TestFixture]
public class LinVector3DMethodEquivalenceTests
{
    private const double Tolerance = 1e-14;
    private IScalarProcessor<double> _scalarProcessor = null!;

    [SetUp]
    public void Setup()
    {
        _scalarProcessor = ScalarProcessorOfFloat64.Instance;
    }

    [Test]
    public void CreateVector_DirectVsComposition_ShouldMatch()
    {
        // Arrange
        double x = 2.0, y = 3.0, z = 4.0;

        // Act - Method A: Direct construction
        var directVector = LinVector3D<double>.Create(
            _scalarProcessor.ScalarFromNumber(x),
            _scalarProcessor.ScalarFromNumber(y),
            _scalarProcessor.ScalarFromNumber(z)
        );

        // Act - Method B: Via basis vectors (different code path!)
        var e1 = LinVector3D<double>.E1(_scalarProcessor);
        var e2 = LinVector3D<double>.E2(_scalarProcessor);
        var e3 = LinVector3D<double>.E3(_scalarProcessor);
        var composedVector = e1 * x + e2 * y + e3 * z;

        // Assert
        Assert.That(composedVector.X.ScalarValue, Is.EqualTo(directVector.X.ScalarValue).Within(Tolerance));
        Assert.That(composedVector.Y.ScalarValue, Is.EqualTo(directVector.Y.ScalarValue).Within(Tolerance));
        Assert.That(composedVector.Z.ScalarValue, Is.EqualTo(directVector.Z.ScalarValue).Within(Tolerance));
    }

    [Test]
    public void Normalization_DivideVsToUnit_ShouldMatch()
    {
        // Arrange
        var vector = LinVector3D<double>.Create(
            _scalarProcessor.ScalarFromNumber(3.0),
            _scalarProcessor.ScalarFromNumber(4.0),
            _scalarProcessor.ScalarFromNumber(0.0)
        );

        // Act - Method A: Manual division
        var norm = vector.VectorENorm();
        var normalized1 = vector / norm.ScalarValue;

        // Act - Method B: ToUnit method (different code path!)
        var normalized2 = vector.ToUnitLinVector3D();

        // Assert
        Assert.That(normalized2.X.ScalarValue, Is.EqualTo(normalized1.X.ScalarValue).Within(Tolerance));
        Assert.That(normalized2.Y.ScalarValue, Is.EqualTo(normalized1.Y.ScalarValue).Within(Tolerance));
        Assert.That(normalized2.Z.ScalarValue, Is.EqualTo(normalized1.Z.ScalarValue).Within(Tolerance));
    }
}
```

---

## 🎯 Fazit & Nächste Schritte

**Empfehlung: Option B (Umfassend)**

1. ✅ **Jetzt:** Erweitere 4 kritische Test-Dateien (LinVector2D/3D, LinQuaternion, LinBivector)
2. ✅ **Jetzt:** Stelle 2 Equivalence Tests um als Template
3. ✅ **Später:** Bei Bedarf weitere Tests erweitern

**Geschätzter Gesamt-Aufwand:** 8-10 Stunden
**Return on Investment:** Sehr hoch - Absicherung der gesamten Generic-First Strategy

**Nächster Schritt:** Beginne mit `LinVector3DTests` als Pilot-Implementation

---

🤖 Generated with [Claude Code](https://claude.com/claude-code)
