# XGa Processors API-Unterschiede: Float64 vs Generic

**Datum:** 2025-10-23
**Analysierte Dateien:** Alle Processor-Dateien in Float64/ und Generic/Processors/

## Zusammenfassung

Die **Float64**- und **Generic**-Prozessor-Implementierungen folgen einem konsistenten Architekturmuster, haben jedoch fundamentale Unterschiede in:
1. **Factory-Methoden-Signaturen** (Generic benötigt `IScalarProcessor<T>` Parameter)
2. **Scalar-Parameter-Typen** (`double` vs `T`, `IScalar<T>`, `Scalar<T>`)
3. **Static vs Instance Singletons** (Float64 nutzt static singletons, Generic erzeugt Instanzen)
4. **Scalar-Operationen** (Float64 direkt mit `+`, `*`; Generic via `ScalarProcessor`)

---

## 1. XGaProcessor Factory Methods

### Float64: XGaFloat64Processor
```csharp
// Static singleton properties
public static XGaFloat64EuclideanProcessor Euclidean { get; }
public static XGaFloat64ProjectiveProcessor Projective { get; }
public static XGaFloat64ConformalProcessor Conformal { get; }

// Factory methods (NO scalar processor parameter)
public static XGaFloat64Processor Create(int negativeCount, int zeroCount)
public static XGaFloat64Processor Create(XGaMetric metric)
```

### Generic: XGaProcessor<T>
```csharp
// Static factory methods (REQUIRE scalar processor)
public static XGaEuclideanProcessor<T> CreateEuclidean(IScalarProcessor<T> scalarProcessor)
public static XGaProjectiveProcessor<T> CreateProjective(IScalarProcessor<T> scalarProcessor)
public static XGaConformalProcessor<T> CreateConformal(IScalarProcessor<T> scalarProcessor)
public static XGaProcessor<T> Create(IScalarProcessor<T> scalarProcessor, int negativeCount, int zeroCount)
public static XGaProcessor<T> Create(IScalarProcessor<T> scalarProcessor, XGaMetric metric)
```

### Unterschiede:
1. **Float64 hat static singleton properties**, Generic hat nur factory methods
2. **Generic benötigt IMMER `IScalarProcessor<T>` als ersten Parameter**
3. **Parameter-Reihenfolge:** Generic hat scalarProcessor ZUERST, dann metric/counts

---

## 2. Processor Properties

### Float64: XGaFloat64Processor
```csharp
// NO ScalarProcessor property (uses double directly)
public XGaFloat64Scalar ScalarZero { get; }
public XGaFloat64Scalar ScalarOne { get; }
public XGaFloat64Scalar ScalarMinusOne { get; }
public XGaFloat64Vector VectorZero { get; }
public XGaFloat64Bivector BivectorZero { get; }
public XGaFloat64GradedMultivector GradedMultivectorZero { get; }
public XGaFloat64UniformMultivector UniformMultivectorZero { get; }
```

### Generic: XGaProcessor<T>
```csharp
// Has ScalarProcessor property
public IScalarProcessor<T> ScalarProcessor { get; }
public XGaEuclideanProcessor<T> EuclideanProcessor { get; }  // Extra!
public XGaScalar<T> ScalarZero { get; }
public XGaScalar<T> ScalarOne { get; }
public XGaScalar<T> ScalarMinusOne { get; }
public XGaVector<T> VectorZero { get; }
public XGaBivector<T> BivectorZero { get; }
public XGaGradedMultivector<T> GradedMultivectorZero { get; }
public XGaUniformMultivector<T> UniformMultivectorZero { get; }
```

### Unterschiede:
1. **Generic hat `ScalarProcessor` property** (Float64 nicht)
2. **Generic hat `EuclideanProcessor` property** (für Euclidean-Operationen)
3. **Typen:** `XGaFloat64*` vs `XGa*<T>`

---

## 3. Composer Creation

### Identisch in beiden:
```csharp
// Float64
CreateScalarComposer() → XGaFloat64KVectorComposer
CreateVectorComposer() → XGaFloat64KVectorComposer
CreateBivectorComposer() → XGaFloat64KVectorComposer
CreateTrivectorComposer() → XGaFloat64KVectorComposer
CreateKVectorComposer(grade) → XGaFloat64KVectorComposer
CreateMultivectorComposer() → XGaFloat64GradedMultivectorComposer
CreateUniformComposer() → XGaFloat64UniformMultivectorComposer

// Generic
CreateScalarComposer() → XGaKVectorComposer<T>
CreateVectorComposer() → XGaKVectorComposer<T>
CreateBivectorComposer() → XGaKVectorComposer<T>
CreateTrivectorComposer() → XGaKVectorComposer<T>
CreateKVectorComposer(grade) → XGaKVectorComposer<T>
CreateMultivectorComposer() → XGaGradedMultivectorComposer<T>
CreateUniformComposer() → XGaUniformMultivectorComposer<T>
```

### Unterschiede:
- **Nur Typ-Namen:** `XGaFloat64*Composer` vs `Xga*Composer<T>`

---

## 4. Scalar Factory Methods

### Float64: XGaFloat64Processor
```csharp
// Single overload: double only
XGaFloat64Scalar Scalar(double scalarValue)

// Aggregate operations
XGaFloat64Scalar ScalarFromSum(double scalar1, double scalar2)
XGaFloat64Scalar ScalarFromSum(params double[] scalarValueList)
XGaFloat64Scalar ScalarFromSum(IEnumerable<double> scalarValueList)

XGaFloat64Scalar ScalarFromProduct(double scalar1, double scalar2)
XGaFloat64Scalar ScalarFromProduct(int sign, double scalar1, double scalar2)
XGaFloat64Scalar ScalarFromProduct(params double[] scalarValueList)
XGaFloat64Scalar ScalarFromProduct(IEnumerable<double> scalarValueList)
```

### Generic: XGaProcessor<T>
```csharp
// Multiple overloads for different types
XGaScalar<T> Scalar(int scalarValue)
XGaScalar<T> Scalar(uint scalarValue)
XGaScalar<T> Scalar(long scalarValue)
XGaScalar<T> Scalar(ulong scalarValue)
XGaScalar<T> Scalar(float scalarValue)
XGaScalar<T> Scalar(double scalarValue)
XGaScalar<T> Scalar(string scalarValue)
XGaScalar<T> Scalar(T scalarValue)                   // Generic T
XGaScalar<T> Scalar(Scalar<T> scalar)                // Wrapped scalar
XGaScalar<T> Scalar(IScalar<T> scalar)               // Interface

// Aggregate operations
XGaScalar<T> ScalarFromSum(T scalar1, T scalar2)
XGaScalar<T> ScalarFromSum(params T[] scalarValueList)
XGaScalar<T> ScalarFromSum(IEnumerable<T> scalarValueList)

XGaScalar<T> ScalarFromProduct(T scalar1, int scalar2)    // Extra: int overload
XGaScalar<T> ScalarFromProduct(T scalar1, T scalar2)
XGaScalar<T> ScalarFromProduct(int sign, T scalar1, T scalar2)
XGaScalar<T> ScalarFromProduct(params T[] scalarValueList)
XGaScalar<T> ScalarFromProduct(IEnumerable<T> scalarValueList)
```

### Unterschiede:
1. **Generic hat 10 Scalar()-Überladungen** (Float64 nur 1)
2. **Generic unterstützt:** `int`, `uint`, `long`, `ulong`, `float`, `double`, `string`, `T`, `Scalar<T>`, `IScalar<T>`
3. **Float64 nur:** `double`
4. **Generic hat Extra-Überladung:** `ScalarFromProduct(T, int)`

---

## 5. Vector Factory Methods

### Float64: XGaFloat64Processor
```csharp
// VectorTerm overloads
XGaFloat64Vector VectorTerm(int index)
XGaFloat64Vector VectorTerm(int index, double scalar)
XGaFloat64Vector VectorTerm(KeyValuePair<int, double> indexScalarPair)
XGaFloat64Vector VectorTerm(ulong basisVector)
XGaFloat64Vector VectorTerm(ulong basisVector, double scalar)
XGaFloat64Vector VectorTerm(IndexSet basisVector)
XGaFloat64Vector VectorTerm(IndexSet basisVector, double scalar)
XGaFloat64Vector VectorTerm(KeyValuePair<ulong, double> indexScalarPair)
XGaFloat64Vector VectorTerm(KeyValuePair<IndexSet, double> indexScalarPair)

// Vector construction
XGaFloat64Vector Vector(params double[] scalarArray)
XGaFloat64Vector Vector(IEnumerable<double> scalarList)
XGaFloat64Vector Vector(IReadOnlyDictionary<IndexSet, double> basisScalarDictionary)
XGaFloat64Vector Vector(IReadOnlyDictionary<int, double> basisScalarDictionary)
XGaFloat64Vector Vector(int termsCount, Func<int, double> indexToScalarFunc)
XGaFloat64Vector Vector(LinFloat64Vector vector)
XGaFloat64Vector Vector(ILinFloat64Vector2D vector)
XGaFloat64Vector Vector(ILinFloat64Vector3D vector)
XGaFloat64Vector Vector(ILinFloat64Vector4D vector)

// Symmetric vectors
XGaFloat64Vector VectorSymmetric(int count)
XGaFloat64Vector VectorSymmetric(int count, double scalarValue)
XGaFloat64Vector VectorSymmetricUnit(int count)
```

### Generic: XGaProcessor<T>
```csharp
// VectorTerm overloads (MORE variants)
XGaVector<T> VectorTerm(int index)
XGaVector<T> VectorTerm(int index, T scalar)
XGaVector<T> VectorTerm(int index, Scalar<T> scalar)              // Extra
XGaVector<T> VectorTerm(KeyValuePair<int, T> indexScalarPair)
XGaVector<T> VectorTerm(ulong basisVectorId)
XGaVector<T> VectorTerm(ulong basisVectorId, T scalar)
XGaVector<T> VectorTerm(ulong basisVectorId, Scalar<T> scalar)   // Extra
XGaVector<T> VectorTerm(KeyValuePair<ulong, T> idScalarPair)
XGaVector<T> VectorTerm(IndexSet basisVectorId)
XGaVector<T> VectorTerm(IndexSet basisVectorId, T scalar)
XGaVector<T> VectorTerm(IndexSet basisVectorId, Scalar<T> scalar) // Extra
XGaVector<T> VectorTerm(KeyValuePair<IndexSet, T> idScalarPair)

// Vector construction (MORE type variants)
XGaVector<T> Vector(IReadOnlyDictionary<IndexSet, T> basisScalarDictionary)
XGaVector<T> Vector(IReadOnlyDictionary<int, T> basisScalarDictionary)
XGaVector<T> Vector(params double[] scalarArray)        // Converts to T
XGaVector<T> Vector(params string[] scalarArray)        // Extra: string
XGaVector<T> Vector(params T[] scalarArray)
XGaVector<T> Vector(params Scalar<T>[] scalarArray)    // Extra: Scalar<T>
XGaVector<T> Vector(params IScalar<T>[] scalarArray)   // Extra: IScalar<T>
XGaVector<T> Vector(IEnumerable<T> scalarList)
XGaVector<T> Vector(int termsCount, Func<int, double> indexToScalarFunc)
XGaVector<T> Vector(int termsCount, Func<int, string> indexToScalarFunc)  // Extra
XGaVector<T> Vector(int termsCount, Func<int, T> indexToScalarFunc)       // Extra
XGaVector<T> Vector(LinVector<T> vector)
XGaVector<T> Vector(ILinFloat64Vector2D vector)
XGaVector<T> Vector(ILinFloat64Vector3D vector)
XGaVector<T> Vector(ILinFloat64Vector4D vector)

// Symmetric vectors
XGaVector<T> VectorSymmetric(int count)
XGaVector<T> VectorSymmetric(int count, T scalarValue)
XGaVector<T> VectorSymmetricUnit(int count)

// Additional: Phasor vectors (ONLY in Generic)
XGaVector<T> VectorUnit(LinAngle<T> angle, int index1, int index2)
XGaVector<T> VectorPhasor(T magnitude, LinAngle<T> angle, int index1, int index2)
XGaVector<T> VectorPhasor(IScalar<T> magnitude, LinAngle<T> angle, int index1, int index2)
```

### Unterschiede:
1. **Generic hat `Scalar<T>` Überladungen** für VectorTerm (Float64 nicht)
2. **Generic hat string[], Scalar<T>[], IScalar<T>[] Überladungen** für Vector()
3. **Generic hat 3 Func<> Überladungen** (double, string, T) - Float64 nur double
4. **Generic hat VectorUnit/VectorPhasor** (mit LinAngle<T>) - Float64 hat diese NICHT
5. **Parameter-Name:** Float64 `basisVector`, Generic `basisVectorId`

---

## 6. Bivector Factory Methods

### Float64: XGaFloat64Processor
```csharp
// BivectorTerm overloads
XGaFloat64Bivector BivectorTerm(int index1, int index2)
XGaFloat64Bivector BivectorTerm(int index1, int index2, double scalar)
XGaFloat64Bivector BivectorTerm(IPair<int> indexPair)
XGaFloat64Bivector BivectorTerm(IPair<int> indexPair, double scalar)
XGaFloat64Bivector BivectorTerm(KeyValuePair<Int32Pair, double> indexScalarPair)
XGaFloat64Bivector BivectorTerm(KeyValuePair<IndexSet, double> indexScalarPair)
XGaFloat64Bivector BivectorTerm(IndexSet basisBlade)
XGaFloat64Bivector BivectorTerm(IndexSet basisBlade, double scalar)

// Bivector construction
XGaFloat64Bivector Bivector(params double[] scalarArray)
XGaFloat64Bivector Bivector(IEnumerable<double> scalarList)
XGaFloat64Bivector Bivector(IReadOnlyDictionary<IndexPair, double> basisScalarDictionary)
XGaFloat64Bivector Bivector(IReadOnlyDictionary<Int32Pair, double> basisScalarDictionary)
XGaFloat64Bivector Bivector(IReadOnlyDictionary<IndexSet, double> basisScalarDictionary)

// Specialized constructors
XGaFloat64Bivector Bivector2D(double scalar01)
XGaFloat64Bivector Bivector3D(double scalar01, double scalar02, double scalar12)
XGaFloat64Bivector Bivector3D(LinFloat64Bivector3D bivector)
```

### Generic: XGaProcessor<T>
```csharp
// BivectorTerm overloads (MORE variants with Scalar<T>)
XGaBivector<T> BivectorTerm(IPair<int> indexPair)
XGaBivector<T> BivectorTerm(IPair<int> indexPair, T scalar)
XGaBivector<T> BivectorTerm(int index1, int index2)
XGaBivector<T> BivectorTerm(int index1, int index2, T scalar)
XGaBivector<T> BivectorTerm(int index1, int index2, IScalar<T> scalar)       // Extra
XGaBivector<T> BivectorTerm(KeyValuePair<Int32Pair, T> indexScalarPair)
XGaBivector<T> BivectorTerm(KeyValuePair<IndexSet, T> indexScalarPair)
XGaBivector<T> BivectorTerm(IndexSet basisBlade)
XGaBivector<T> BivectorTerm(IndexSet basisBlade, T scalar)
XGaBivector<T> BivectorTerm(IndexSet basisBlade, Scalar<T> scalar)           // Extra

// Bivector construction (NO array overloads in Generic!)
XGaBivector<T> Bivector(IReadOnlyDictionary<IndexPair, T> basisScalarDictionary)
XGaBivector<T> Bivector(IReadOnlyDictionary<Int32Pair, T> basisScalarDictionary)
XGaBivector<T> Bivector(IReadOnlyDictionary<IndexSet, T> basisScalarDictionary)

// Specialized constructors (MORE type overloads)
XGaBivector<T> Bivector2D(double scalar01)
XGaBivector<T> Bivector2D(string scalar01)                                   // Extra
XGaBivector<T> Bivector2D(T scalar01)                                        // Extra
XGaBivector<T> Bivector3D(double scalar01, double scalar02, double scalar12)
XGaBivector<T> Bivector3D(string scalar01, string scalar02, string scalar12) // Extra
XGaBivector<T> Bivector3D(T scalar01, T scalar02, T scalar12)                // Extra
XGaBivector<T> Bivector3D(LinFloat64Bivector3D bivector)
```

### Unterschiede:
1. **Generic hat `IScalar<T>` und `Scalar<T>` Überladungen** - Float64 nicht
2. **Float64 hat `Bivector(double[])` und `Bivector(IEnumerable<double>)`** - Generic NICHT!
3. **Generic hat string und T Überladungen** für Bivector2D/3D - Float64 nur double
4. **Parameter-Reihenfolge identisch** (index1, index2, scalar)

---

## 7. HigherKVector Factory Methods

### Float64: XGaFloat64Processor
```csharp
XGaFloat64HigherKVector HigherKVectorZero(int grade)
XGaFloat64HigherKVector HigherKVectorTerm(IndexSet id)
XGaFloat64HigherKVector HigherKVectorTerm(IndexSet id, double scalar)
XGaFloat64HigherKVector HigherKVectorTerm(KeyValuePair<IndexSet, double> term)

XGaFloat64HigherKVector HigherKVector(int grade, params double[] scalarArray)
XGaFloat64HigherKVector HigherKVector(int grade, IEnumerable<double> scalarList)
XGaFloat64HigherKVector HigherKVector(int grade, IReadOnlyDictionary<IndexSet, double> basisScalarDictionary)
```

### Generic: XGaProcessor<T>
```csharp
XGaHigherKVector<T> HigherKVectorZero(int grade)
XGaHigherKVector<T> HigherKVectorTerm(IndexSet id)
XGaHigherKVector<T> HigherKVectorTerm(IndexSet id, T scalar)
XGaHigherKVector<T> HigherKVectorTerm(IndexSet id, IScalar<T> scalar)      // Extra
XGaHigherKVector<T> HigherKVectorTerm(KeyValuePair<IndexSet, T> term)

XGaHigherKVector<T> HigherKVector(int grade, IReadOnlyDictionary<IndexSet, T> basisScalarDictionary)
// NO array or IEnumerable overloads!
```

### Unterschiede:
1. **Float64 hat `HigherKVector(grade, double[])` und `HigherKVector(grade, IEnumerable<double>)`** - Generic NICHT
2. **Generic hat `IScalar<T>` Überladung** für HigherKVectorTerm - Float64 nicht

---

## 8. KVector Factory Methods

### Float64: XGaFloat64Processor
```csharp
XGaFloat64KVector KVectorZero(int grade)
XGaFloat64KVector KVectorTerm(KeyValuePair<IndexSet, double> term)
XGaFloat64KVector KVectorTerm(IndexSet basisBlade)
XGaFloat64KVector KVectorTerm(IndexSet basisBlade, double scalar)
XGaFloat64KVector KVectorTerm(ulong basisBlade)
XGaFloat64KVector KVectorTerm(ulong basisBlade, double scalar)

XGaFloat64KVector KVector(int grade, params double[] scalarArray)
XGaFloat64KVector KVector(int grade, IEnumerable<double> scalarList)
XGaFloat64KVector KVector(int grade, IReadOnlyDictionary<IndexSet, double> basisScalarDictionary)
```

### Generic: XGaProcessor<T>
```csharp
XGaKVector<T> KVectorZero(int grade)
XGaKVector<T> KVectorTerm(KeyValuePair<IndexSet, T> term)
XGaKVector<T> KVectorTerm(IndexSet basisBlade)
XGaKVector<T> KVectorTerm(IndexSet basisBlade, T scalar)
XGaKVector<T> KVectorTerm(IndexSet basisBlade, string scalar)               // Extra
XGaKVector<T> KVectorTerm(IndexSet basisBlade, IScalar<T> scalar)           // Extra
XGaKVector<T> KVectorTerm(IReadOnlyList<int> basisVectorIndexList)          // Extra
XGaKVector<T> KVectorTerm(IReadOnlyList<int> basisVectorIndexList, T scalar) // Extra

XGaKVector<T> KVector(int grade, IReadOnlyDictionary<IndexSet, T> basisScalarDictionary)
// NO array or IEnumerable overloads!
```

### Unterschiede:
1. **Float64 hat array/IEnumerable Überladungen** - Generic NICHT
2. **Generic hat string und IScalar<T> Überladungen** - Float64 nicht
3. **Generic hat `KVectorTerm(IReadOnlyList<int>)` Überladungen** (basis vector index lists) - Float64 NICHT

---

## 9. PseudoScalar Methods

### Float64: XGaFloat64Processor
```csharp
XGaFloat64KVector PseudoScalar(int vSpaceDimensions)
XGaFloat64KVector PseudoScalar(int vSpaceDimensions, double scalarValue)
XGaFloat64KVector PseudoScalarReverse(int vSpaceDimensions)
XGaFloat64KVector PseudoScalarConjugate(int vSpaceDimensions)
XGaFloat64KVector PseudoScalarEInverse(int vSpaceDimensions)
XGaFloat64KVector PseudoScalarInverse(int vSpaceDimensions)
```

### Generic: XGaProcessor<T>
```csharp
XGaKVector<T> PseudoScalar(int vSpaceDimensions)
XGaKVector<T> PseudoScalar(int vSpaceDimensions, T scalarValue)
XGaKVector<T> PseudoScalarReverse(int vSpaceDimensions)
XGaKVector<T> PseudoScalarConjugate(int vSpaceDimensions)
XGaKVector<T> PseudoScalarEInverse(int vSpaceDimensions)
XGaKVector<T> PseudoScalarInverse(int vSpaceDimensions)
```

### Unterschiede:
- **Identische Methoden-Signaturen**, nur Typen unterschiedlich (`double` vs `T`)

---

## 10. Multivector Factory Methods

### Float64: XGaFloat64Processor
```csharp
// GradedMultivector
XGaFloat64GradedMultivector GradedMultivector(IndexSet id)
XGaFloat64GradedMultivector GradedMultivector(IndexSet id, double scalar)
XGaFloat64GradedMultivector GradedMultivector(KeyValuePair<IndexSet, double> basisScalarPair)
XGaFloat64GradedMultivector GradedMultivector(params double[] scalarArray)
XGaFloat64GradedMultivector GradedMultivector(IEnumerable<double> scalarList)
XGaFloat64GradedMultivector GradedMultivector(IReadOnlyDictionary<IndexSet, double> termList)
XGaFloat64GradedMultivector GradedMultivector(IReadOnlyDictionary<int, XGaFloat64KVector> gradeKVectorDictionary)
XGaFloat64GradedMultivector GradedMultivector(IEnumerable<KeyValuePair<IndexSet, double>> termList)
XGaFloat64GradedMultivector GradedMultivectorFromSum(IEnumerable<XGaFloat64KVector> kVectorList)

// UniformMultivector
XGaFloat64UniformMultivector UniformMultivector(IndexSet basisBlade)
XGaFloat64UniformMultivector UniformMultivector(IndexSet basisBlade, double scalar)
XGaFloat64UniformMultivector UniformMultivector(KeyValuePair<IndexSet, double> basisScalarPair)
XGaFloat64UniformMultivector UniformMultivector(params double[] scalarList)
XGaFloat64UniformMultivector UniformMultivector(IEnumerable<double> scalarList)
XGaFloat64UniformMultivector UniformMultivector(IReadOnlyDictionary<IndexSet, double> basisScalarDictionary)

// Multivector (base type)
XGaFloat64Multivector Multivector(params double[] scalarArray)
XGaFloat64Multivector Multivector(IEnumerable<double> scalarList)
XGaFloat64Multivector Multivector(IReadOnlyDictionary<IndexSet, double> termList)
XGaFloat64Multivector Multivector(IReadOnlyDictionary<int, XGaFloat64KVector> gradeKVectorDictionary)
XGaFloat64Multivector Multivector(IEnumerable<KeyValuePair<IndexSet, double>> termList)
XGaFloat64Multivector MultivectorFromSum(IEnumerable<XGaFloat64KVector> kVectorList)
XGaFloat64Multivector Multivector2D(double scalar, double vectorScalar0, double vectorScalar1, double bivectorScalar)
```

### Generic: XGaProcessor<T>
```csharp
// GradedMultivector
XGaGradedMultivector<T> GradedMultivector(IReadOnlyDictionary<IndexSet, T> termList)
XGaGradedMultivector<T> GradedMultivector(IReadOnlyDictionary<int, XGaKVector<T>> gradeKVectorDictionary)
XGaGradedMultivector<T> GradedMultivector(IEnumerable<KeyValuePair<IndexSet, T>> termList)
XGaGradedMultivector<T> GradedMultivector(IndexSet id)
XGaGradedMultivector<T> GradedMultivector(IndexSet id, T scalar)
XGaGradedMultivector<T> GradedMultivector(IndexSet id, IScalar<T> scalar)              // Extra
XGaGradedMultivector<T> GradedMultivector(KeyValuePair<IndexSet, T> basisScalarPair)
XGaGradedMultivector<T> GradedMultivectorFromSum(IEnumerable<XGaKVector<T>> kVectorList)
// NO array or IEnumerable<double> overloads!

// UniformMultivector
XGaUniformMultivector<T> UniformMultivector(IReadOnlyDictionary<IndexSet, T> basisScalarDictionary)
XGaUniformMultivector<T> UniformMultivector(IndexSet basisBlade)
XGaUniformMultivector<T> UniformMultivector(IndexSet basisBlade, T scalar)
XGaUniformMultivector<T> UniformMultivector(KeyValuePair<IndexSet, T> basisScalarPair)
XGaUniformMultivector<T> UniformMultivector(IndexSet basisBlade, Scalar<T> scalar)     // Extra
// NO array or IEnumerable<double> overloads!

// Multivector (base type)
XGaMultivector<T> MultivectorFromSum(IEnumerable<XGaKVector<T>> kVectorList)
XGaMultivector<T> Multivector2D(T scalar, T vectorScalar0, T vectorScalar1, T bivectorScalar)
XGaMultivector<T> Multivector2D(IScalar<T> scalar, IScalar<T> vectorScalar0, IScalar<T> vectorScalar1, IScalar<T> bivectorScalar) // Extra
// NO generic Multivector() overloads with array/IEnumerable!
```

### Unterschiede:
1. **Float64 hat `GradedMultivector(double[])` und `GradedMultivector(IEnumerable<double>)`** - Generic NICHT
2. **Float64 hat `UniformMultivector(double[])` und `UniformMultivector(IEnumerable<double>)`** - Generic NICHT
3. **Float64 hat `Multivector(double[])`, `Multivector(IEnumerable<double>)`, `Multivector(Dictionary<>)` Überladungen** - Generic NICHT
4. **Generic hat `IScalar<T>` und `Scalar<T>` Überladungen** - Float64 nicht
5. **Generic Multivector2D hat IScalar<T> Überladung** - Float64 nur double

---

## 11. Outer Product (Op) Operations

### Float64: XGaFloat64Processor
```csharp
XGaFloat64KVector Op(IEnumerable<XGaFloat64Vector> mvList)
XGaFloat64KVector SpanToBlade(IEnumerable<XGaFloat64Vector> mvList)
```

### Generic: XGaProcessor<T>
```csharp
XGaKVector<T> Op(IEnumerable<XGaVector<T>> mvList)
XGaKVector<T> SpanToBlade(IEnumerable<XGaVector<T>> mvList)
```

### Unterschiede:
- **Identische Methoden-Signaturen**, nur Typen unterschiedlich
- **Semantik:** `Op()` gibt ScalarZero zurück bei zero blade, `SpanToBlade()` überspringt near-zero blades

---

## 12. Random Composers

### Float64: XGaFloat64Processor
```csharp
XGaFloat64RandomComposer CreateXGaRandomComposer(int vSpaceDimensions)
XGaFloat64RandomComposer CreateXGaRandomComposer(int vSpaceDimensions, int seed)
XGaFloat64RandomComposer CreateXGaRandomComposer(int vSpaceDimensions, Random randomGenerator)
```

### Generic: XGaProcessor<T>
```csharp
XGaRandomComposer<T> CreateXGaRandomComposer(int vSpaceDimensions)
XGaRandomComposer<T> CreateXGaRandomComposer(int vSpaceDimensions, int seed)
// NO Random overload!
```

### Unterschiede:
- **Float64 hat `Random randomGenerator` Überladung** - Generic NICHT

---

## 13. Frame Operations

### Float64: XGaFloat64Processor
```csharp
XGaFloat64BasisVectorFrame CreateBasisVectorFrame(int vSpaceDimensions)
XGaFloat64VectorFrame CreateFreeFrameOfBasis(int vSpaceDimensions)
XGaFloat64VectorFrame CreateFreeFrameOfScaledBasis(int vSpaceDimensions, double scalingFactor)
XGaFloat64VectorFrame CreateFreeFrameOfSimplex(int vSpaceDimensions, double scalingFactor)
XGaFloat64VectorFrameFixed CreateBasisVectorFrameFixed(int vSpaceDimensions)
XGaFloat64VectorFrameFixed CreateFixedFrameOfScaledBasis(int vSpaceDimensions, double scalingFactor)
XGaFloat64VectorFrameFixed CreateFixedFrameOfSimplex(int vSpaceDimensions, double scalingFactor)
```

### Generic: XGaProcessor<T>
```csharp
XGaBasisVectorFrame<T> CreateBasisVectorFrame(int vSpaceDimensions)
XGaVectorFrame<T> CreateFreeFrameOfBasis(int vSpaceDimensions)
XGaVectorFrame<T> CreateFreeFrameOfScaledBasis(int vSpaceDimensions, T scalingFactor)
XGaVectorFrame<T> CreateFreeFrameOfSimplex(int vSpaceDimensions, T scalingFactor)
XGaVectorFrameFixed<T> CreateBasisVectorFrameFixed(int vSpaceDimensions)
XGaVectorFrameFixed<T> CreateFixedFrameOfScaledBasis(int vSpaceDimensions, T scalingFactor)
XGaVectorFrameFixed<T> CreateFixedFrameOfSimplex(int vSpaceDimensions, T scalingFactor)
```

### Unterschiede:
- **Identische Methoden-Signaturen**, nur Typen unterschiedlich (`double` vs `T`)
- **Float64:** `.CreatePureRotor()`, **Generic:** `.GetEuclideanPureRotorTo()` (unterschiedliche Methoden-Namen!)

---

## 14. Specialized Processors

### 14.1 Euclidean Processors

#### Float64: XGaFloat64EuclideanProcessor
```csharp
// Singleton pattern
public static XGaFloat64EuclideanProcessor Instance { get; }

XGaFloat64EuclideanSpace CreateSpace(int vSpaceDimensions)
```

#### Generic: XGaEuclideanProcessor<T>
```csharp
// NO static singleton (instance-based)
internal XGaEuclideanProcessor(IScalarProcessor<T> scalarProcessor) : base(scalarProcessor, 0, 0)

XGaEuclideanSpace<T> CreateSpace(int vSpaceDimensions)
```

### 14.2 Projective Processors

#### Float64: XGaFloat64ProjectiveProcessor
```csharp
public static XGaFloat64ProjectiveProcessor Instance { get; }

XGaFloat64KVector PGaDual(XGaFloat64KVector kVector, int vSpaceDimensions)
XGaFloat64Multivector PGaDual(XGaFloat64Multivector mv, int vSpaceDimensions)
```

#### Generic: XGaProjectiveProcessor<T>
```csharp
internal XGaProjectiveProcessor(IScalarProcessor<T> scalarProcessor) : base(scalarProcessor, 0, 1)

XGaKVector<T> PGaDual(XGaKVector<T> kVector, int vSpaceDimensions)
XGaMultivector<T> PGaDual(XGaMultivector<T> mv, int vSpaceDimensions)
XGaKVector<T> PGaUnDual(XGaKVector<T> kVector, int vSpaceDimensions)            // Extra
XGaMultivector<T> PGaUnDual(XGaMultivector<T> mv, int vSpaceDimensions)         // Extra
XGaKVector<T> PGaPolarity(XGaKVector<T> kVector, int vSpaceDimensions)          // Extra
XGaMultivector<T> PGaPolarity(XGaMultivector<T> mv, int vSpaceDimensions)       // Extra
XGaKVector<T> InnerProduct(XGaKVector<T> kVector1, XGaKVector<T> kVector2)      // Extra
XGaKVector<T> Meet(XGaKVector<T> kVector1, XGaKVector<T> kVector2)              // Extra
XGaMultivector<T> Meet(XGaMultivector<T> kVector1, XGaMultivector<T> kVector2)  // Extra
XGaKVector<T> Join(XGaKVector<T> kVector1, XGaKVector<T> kVector2, int vSpaceDimensions) // Extra
XGaMultivector<T> Join(XGaMultivector<T> kVector1, XGaMultivector<T> kVector2, int vSpaceDimensions) // Extra

XGaProjectiveSpace<T> CreateSpace(int vSpaceDimensions)
```

#### Unterschiede:
1. **Generic hat 9 EXTRA Methoden:** PGaUnDual, PGaPolarity, InnerProduct, Meet (2x), Join (2x)
2. **Float64 nutzt `.EDual()`, Generic nutzt `.EUnDual()`** (unterschiedliche Basis-Methoden)

### 14.3 Conformal Processors

#### Float64: XGaFloat64ConformalProcessor
```csharp
public static XGaFloat64ConformalProcessor Instance { get; }

// Basis vectors
public XGaFloat64Vector En { get; }
public XGaFloat64Vector Ep { get; }
public XGaFloat64Vector Eo { get; }
public XGaFloat64Vector Ei { get; }
public XGaFloat64MusicalAutomorphism MusicalAutomorphism { get; }

// Validation
bool IsValidHGaPoint(XGaFloat64Vector hgaPoint)
bool IsValidPGaPoint(XGaFloat64KVector pgaPoint, int vSpaceDimensions)
bool IsValidIpnsPoint(XGaFloat64Vector ipnsPoint)

// PGaDual
XGaFloat64KVector PGaDual(XGaFloat64KVector mv, int vSpaceDimensions)
XGaFloat64Multivector PGaDual(XGaFloat64Multivector mv, int vSpaceDimensions)

// Encoding (only double overloads)
XGaFloat64Vector EncodeEGaVector(double x, double y)
XGaFloat64Vector EncodeEGaVector(double x, double y, double z)
XGaFloat64Vector EncodeEGaVector(LinFloat64Vector2D egaVector)
XGaFloat64Vector EncodeEGaVector(LinFloat64Vector3D egaVector)

XGaFloat64Vector EncodeHGaPoint(double x, double y)
XGaFloat64Vector EncodeHGaPoint(double x, double y, double z)
XGaFloat64Vector EncodeHGaPoint(LinFloat64Vector2D egaVector)
XGaFloat64Vector EncodeHGaPoint(LinFloat64Vector3D egaVector)

XGaFloat64KVector EncodePGaPoint(double x, double y)
XGaFloat64KVector EncodePGaPoint(double x, double y, double z)
XGaFloat64KVector EncodePGaPoint(LinFloat64Vector2D egaPoint)
XGaFloat64KVector EncodePGaPoint(LinFloat64Vector3D egaPoint)

XGaFloat64Vector EncodeIpnsPoint(double x, double y)
XGaFloat64Vector EncodeIpnsPoint(double x, double y, double z)
XGaFloat64Vector EncodeIpnsPoint(LinFloat64Vector2D egaPoint)
XGaFloat64Vector EncodeIpnsPoint(LinFloat64Vector3D egaPoint)

// Decoding (returns LinFloat64Vector or XGaFloat64Vector)
LinFloat64Vector2D DecodeEGaVectorAsVector2D(XGaFloat64Vector egaVector)
LinFloat64Vector2D DecodeEGaVectorAsVector2D(XGaFloat64Vector egaVector, double scalingFactor)
LinFloat64Vector3D DecodeEGaVectorAsVector3D(XGaFloat64Vector egaVector)
LinFloat64Vector3D DecodeEGaVectorAsVector3D(XGaFloat64Vector egaVector, double scalingFactor)
LinFloat64Vector DecodeEGaVectorAsVector(XGaFloat64Vector egaVector)
LinFloat64Vector DecodeEGaVectorAsVector(XGaFloat64Vector egaVector, double scalingFactor)
XGaFloat64Vector DecodeEGaVector(XGaFloat64Vector egaVector)
XGaFloat64Vector DecodeEGaVector(XGaFloat64Vector egaVector, double scalingFactor)

LinFloat64Vector2D DecodeHGaPointAsVector2D(XGaFloat64Vector hgaPoint)
LinFloat64Vector3D DecodeHGaPointAsVector3D(XGaFloat64Vector hgaPoint)
XGaFloat64Vector DecodeHGaPoint(XGaFloat64Vector hgaPoint)

LinFloat64Vector2D DecodePGaPointAsVector2D(XGaFloat64KVector pgaPoint)
LinFloat64Vector3D DecodePGaPointAsVector3D(XGaFloat64KVector pgaPoint)
XGaFloat64Vector DecodePGaPoint(XGaFloat64KVector pgaPoint, int vSpaceDimensions)

LinFloat64Vector2D DecodeIpnsPointAsVector2D(XGaFloat64Vector ipnsPoint)
LinFloat64Vector3D DecodeIpnsPointAsVector3D(XGaFloat64Vector ipnsPoint)
XGaFloat64Vector DecodeIpnsPoint(XGaFloat64Vector ipnsPoint)

// PGa Regressive Product
XGaFloat64KVector PGaRp(XGaFloat64KVector mv1, XGaFloat64KVector mv2, int vSpaceDimensions)
XGaFloat64Multivector PGaRp(XGaFloat64Multivector mv1, XGaFloat64Multivector mv2, int vSpaceDimensions)

// Space creation
XGaFloat64ConformalSpace CreateSpace(int vSpaceDimensions)
```

#### Generic: XGaConformalProcessor<T>
```csharp
// NO static singleton (instance-based with IScalarProcessor<T>)
internal XGaConformalProcessor(IScalarProcessor<T> scalarProcessor) : base(scalarProcessor, 1, 0)

// Basis vectors
public XGaVector<T> En { get; }
public XGaVector<T> Ep { get; }
public XGaVector<T> Eo { get; }
public XGaVector<T> Ei { get; }
public XGaMusicalAutomorphism<T> MusicalAutomorphism { get; }

// Validation (SAME signatures)
bool IsValidHGaPoint(XGaVector<T> hgaPoint)
bool IsValidPGaPoint(XGaKVector<T> pgaPoint, int vSpaceDimensions)
bool IsValidIpnsPoint(XGaVector<T> ipnsPoint)

// PGaDual (SAME signatures)
XGaKVector<T> PGaDual(XGaKVector<T> mv, int vSpaceDimensions)
XGaMultivector<T> PGaDual(XGaMultivector<T> mv, int vSpaceDimensions)

// Encoding (double AND T overloads)
XGaVector<T> EncodeEGaVector(double x, double y)
XGaVector<T> EncodeEGaVector(T x, T y)                                   // Extra
XGaVector<T> EncodeEGaVector(double x, double y, double z)
XGaVector<T> EncodeEGaVector(T x, T y, T z)                              // Extra
XGaVector<T> EncodeEGaVector(LinFloat64Vector2D egaVector)
XGaVector<T> EncodeEGaVector(LinFloat64Vector3D egaVector)

XGaVector<T> EncodeHGaPoint(double x, double y)
XGaVector<T> EncodeHGaPoint(T x, T y)                                    // Extra
XGaVector<T> EncodeHGaPoint(Scalar<T> x, Scalar<T> y)                   // Extra
XGaVector<T> EncodeHGaPoint(double x, double y, double z)
XGaVector<T> EncodeHGaPoint(T x, T y, T z)                               // Extra
XGaVector<T> EncodeHGaPoint(LinFloat64Vector2D egaVector)
XGaVector<T> EncodeHGaPoint(LinFloat64Vector3D egaVector)

XGaKVector<T> EncodePGaPoint(double x, double y)
XGaKVector<T> EncodePGaPoint(T x, T y)                                   // Extra
XGaKVector<T> EncodePGaPoint(double x, double y, double z)
XGaKVector<T> EncodePGaPoint(T x, T y, T z)                              // Extra
XGaKVector<T> EncodePGaPoint(LinFloat64Vector2D egaPoint)
XGaKVector<T> EncodePGaPoint(LinFloat64Vector3D egaPoint)

XGaVector<T> EncodeIpnsPoint(double x, double y)
XGaVector<T> EncodeIpnsPoint(T x, T y)                                   // Extra
XGaVector<T> EncodeIpnsPoint(double x, double y, double z)
XGaVector<T> EncodeIpnsPoint(T x, T y, T z)                              // Extra
XGaVector<T> EncodeIpnsPoint(LinFloat64Vector2D egaPoint)
XGaVector<T> EncodeIpnsPoint(LinFloat64Vector3D egaPoint)

// Decoding (ONLY XGaVector<T>, NO LinFloat64Vector)
XGaVector<T> DecodeEGaVector(XGaVector<T> egaVector)
XGaVector<T> DecodeEGaVector(XGaVector<T> egaVector, T scalingFactor)
XGaVector<T> DecodeHGaPoint(XGaVector<T> hgaPoint)
XGaVector<T> DecodePGaPoint(XGaKVector<T> pgaPoint, int vSpaceDimensions)
XGaVector<T> DecodeIpnsPoint(XGaVector<T> ipnsPoint)

// NO DecodeAsVector2D/3D methods in Generic!

// PGa Regressive Product (SAME signatures)
XGaKVector<T> PGaRp(XGaKVector<T> mv1, XGaKVector<T> mv2, int vSpaceDimensions)
XGaMultivector<T> PGaRp(XGaMultivector<T> mv1, XGaMultivector<T> mv2, int vSpaceDimensions)

// Space creation
XGaConformalSpace<T> CreateSpace(int vSpaceDimensions)
```

#### Unterschiede:
1. **Float64 hat static singleton**, Generic instance-based
2. **Generic hat T Überladungen** für alle Encode-Methoden (Float64 nur double)
3. **Generic hat `Scalar<T>` Überladung** für EncodeHGaPoint
4. **Float64 hat `DecodeAsVector2D/3D/AsVector` Methoden** (return LinFloat64Vector) - Generic NICHT
5. **Generic decode methods return nur `XGaVector<T>`**, nicht LinFloat64Vector

---

## 15. Extension Methods (ProcessorComposerUtils)

### Float64: XGaFloat64ProcessorComposerUtils
```csharp
// Extension methods on IXGaFloat64ProcessorContainer
XGaFloat64Processor CreateEuclideanXGaFloat64Processor(this IXGaFloat64ProcessorContainer)
XGaFloat64Processor CreateProjectiveXGaFloat64Processor(this IXGaFloat64ProcessorContainer)
XGaFloat64Processor CreateConformalXGaFloat64Processor(this IXGaFloat64ProcessorContainer)
XGaFloat64Processor CreateXGaFloat64Processor(this IXGaFloat64ProcessorContainer, XGaMetric metric)
XGaFloat64Processor CreateXGaFloat64Processor(this IXGaFloat64ProcessorContainer, int negativeCount, int zeroCount)
```

### Generic: XGaProcessorComposerUtils
```csharp
// Extension methods on IScalarProcessor<T>
XGaEuclideanProcessor<T> CreateEuclideanXGaProcessor<T>(this IScalarProcessor<T>)
XGaProjectiveProcessor<T> CreateProjectiveXGaProcessor<T>(this IScalarProcessor<T>)
XGaConformalProcessor<T> CreateConformalXGaProcessor<T>(this IScalarProcessor<T>)
XGaProcessor<T> CreateXGaProcessor<T>(this IScalarProcessor<T>, XGaMetric metric)
XGaProcessor<T> CreateXGaProcessor<T>(this IScalarProcessor<T>, int negativeCount, int zeroCount)
```

### Unterschiede:
1. **Float64 extends `IXGaFloat64ProcessorContainer`**
2. **Generic extends `IScalarProcessor<T>`**
3. **Generic hat conditional `AttachXGaProcessor()` call** (nur wenn processorContainer ist IXGaProcessorContainer<T>)

---

## 16. Scalar Operations Patterns

### Float64: Direkte double Operationen
```csharp
// Interne Arithmetik verwendet native double operators
var result = scalar1 + scalar2;
var product = scalar1 * scalar2;
var sum = 0d;
sum += scalarValue;

// Validation mit extension methods
if (scalarValue.IsValid()) { }
if (scalarValue.IsZero()) { }
```

### Generic: ScalarProcessor Operations
```csharp
// ALLE Arithmetik über ScalarProcessor
var result = ScalarProcessor.Add(scalar1, scalar2);
var product = ScalarProcessor.Times(scalar1, scalar2);
var sum = ScalarProcessor.ZeroValue;
sum = ScalarProcessor.Add(sum, scalarValue).ScalarValue;

// Validation über ScalarProcessor
if (ScalarProcessor.IsValid(scalarValue)) { }
if (ScalarProcessor.IsZero(scalarValue)) { }

// Konvertierung von primitiven Typen
var fromInt = ScalarProcessor.ValueFromNumber(42);
var fromString = ScalarProcessor.ValueFromText("3.14");
var fromDouble = ScalarProcessor.ScalarFromNumber(2.5);
```

### Pattern-Unterschiede:
1. **Float64 nutzt native C# operators** (`+`, `-`, `*`, `/`)
2. **Generic nutzt IMMER ScalarProcessor methods**
3. **Generic benötigt Typ-Konvertierung** für primitive Typen
4. **Float64 nutzt extension methods** (`.IsZero()`) auf double
5. **Generic nutzt ScalarProcessor methods** (`ScalarProcessor.IsZero(value)`)

---

## 17. Parsing Methods (ONLY in Float64)

### Float64: XGaFloat64Processor
```csharp
XGaFloat64Scalar ParseScalar(string inputText)
XGaFloat64Vector ParseVector(string inputText)
XGaFloat64Bivector ParseBivector(string inputText)
XGaFloat64HigherKVector ParseTrivector(string inputText)
XGaFloat64KVector ParseKVector(string inputText, int grade)
XGaFloat64Multivector Parse(string inputText)
```

### Generic: XGaProcessor<T>
```csharp
// NO Parse methods!
```

### Unterschiede:
- **Float64 hat 6 Parse-Methoden** für string → multivector
- **Generic hat KEINE Parse-Methoden** (muss via ScalarProcessor.ScalarFromText + Composer gemacht werden)

---

## 18. Key API Design Patterns

### 18.1 Static Singletons vs Factory Methods

**Float64:**
```csharp
// Use pre-configured singletons
var processor = XGaFloat64Processor.Euclidean;
var processor = XGaFloat64Processor.Conformal;
```

**Generic:**
```csharp
// Create new instances with scalar processor
var processor = XGaProcessor<T>.CreateEuclidean(scalarProcessor);
var processor = XGaProcessor<T>.CreateConformal(scalarProcessor);
```

### 18.2 Hybrid API (Generic)

Generic-Prozessor unterstützt **drei Parameter-Typen** für maximale Flexibilität:

```csharp
// 1. Primitive types (werden konvertiert)
processor.Vector(1.0, 2.0, 3.0)             // double[]
processor.Vector("1", "2", "3")             // string[]

// 2. Generic type T (direkt)
processor.Vector(t1, t2, t3)                // T[]

// 3. Wrapped scalars
processor.Vector(scalar1, scalar2, scalar3) // Scalar<T>[]
processor.Vector(iscalar1, iscalar2)        // IScalar<T>[]
```

### 18.3 Parameter Naming Conventions

**Float64:**
- `basisVector`, `basisBlade`, `scalar`, `scalarValue`
- Verwendet `double` direkt

**Generic:**
- `basisVectorId`, `basisBlade`, `scalar`, `scalarValue`
- Verwendet `T`, `Scalar<T>`, `IScalar<T>`

**Unterschiede:**
- Generic fügt oft "Id" zu basis blade parameters hinzu
- Generic hat explizite wrapper-Typen

---

## 19. Vollständige API Differenz-Tabelle

| **Feature** | **Float64** | **Generic** | **Unterschiede** |
|-------------|-------------|-------------|------------------|
| **Factory Creation** | Static singletons | Factory methods mit `IScalarProcessor<T>` | Generic benötigt scalar processor |
| **ScalarProcessor Property** | ❌ Nicht vorhanden | ✅ `IScalarProcessor<T>` | Float64 nutzt double direkt |
| **EuclideanProcessor Property** | ❌ Nicht vorhanden | ✅ `XGaEuclideanProcessor<T>` | Generic hat dedicated property |
| **Scalar() Überladungen** | 1 (double) | 10 (int, uint, long, ulong, float, double, string, T, Scalar<T>, IScalar<T>) | Generic hat 10x mehr |
| **VectorTerm() Überladungen** | 9 | 12 | Generic hat +3 Scalar<T> variants |
| **Vector() Überladungen** | 9 | 16 | Generic hat +7 type variants |
| **VectorPhasor Methods** | ❌ Nicht vorhanden | ✅ 2 methods | Nur Generic |
| **BivectorTerm() Überladungen** | 8 | 10 | Generic hat +2 IScalar<T> variants |
| **Bivector2D/3D Überladungen** | 3 (double only) | 9 (double, string, T) | Generic hat +6 type variants |
| **Bivector(array) Konstruktoren** | ✅ 2 methods | ❌ Nicht vorhanden | Float64 exclusive |
| **HigherKVector(array) Konstruktoren** | ✅ 2 methods | ❌ Nicht vorhanden | Float64 exclusive |
| **KVector(array) Konstruktoren** | ✅ 2 methods | ❌ Nicht vorhanden | Float64 exclusive |
| **KVectorTerm(string) Überladung** | ❌ | ✅ | Nur Generic |
| **KVectorTerm(IReadOnlyList<int>)** | ❌ | ✅ 2 methods | Nur Generic |
| **GradedMultivector(array)** | ✅ 2 methods | ❌ Nicht vorhanden | Float64 exclusive |
| **UniformMultivector(array)** | ✅ 2 methods | ❌ Nicht vorhanden | Float64 exclusive |
| **Multivector(array)** | ✅ 2 methods | ❌ Nicht vorhanden | Float64 exclusive |
| **Multivector2D IScalar<T> Überladung** | ❌ | ✅ | Nur Generic |
| **CreateXGaRandomComposer(Random)** | ✅ | ❌ | Float64 exclusive |
| **Parse Methods** | ✅ 6 methods | ❌ Nicht vorhanden | Float64 exclusive |
| **Projective: PGaUnDual** | ❌ | ✅ 2 methods | Nur Generic |
| **Projective: PGaPolarity** | ❌ | ✅ 2 methods | Nur Generic |
| **Projective: InnerProduct** | ❌ | ✅ 1 method | Nur Generic |
| **Projective: Meet** | ❌ | ✅ 2 methods | Nur Generic |
| **Projective: Join** | ❌ | ✅ 2 methods | Nur Generic |
| **Conformal: Encode T Überladungen** | ❌ | ✅ ~12 methods | Nur Generic |
| **Conformal: Encode Scalar<T>** | ❌ | ✅ 1 method | Nur Generic |
| **Conformal: DecodeAsVector2D/3D** | ✅ 9 methods | ❌ Nicht vorhanden | Float64 exclusive |
| **Conformal: DecodeAsVector** | ✅ 2 methods | ❌ Nicht vorhanden | Float64 exclusive |
| **Scalar Arithmetic** | Native `+`, `-`, `*`, `/` | `ScalarProcessor.Add()`, `.Times()` etc. | Fundamentaler Unterschied |
| **Validation** | Extension methods `.IsZero()` | `ScalarProcessor.IsZero(value)` | Unterschiedliche Pattern |

---

## 20. Wichtige Erkenntnisse

### 20.1 Architektur-Konsistenz
- **Float64 und Generic folgen identischer Architektur** (partial classes, method names)
- **Unterschiede sind primär type-system driven**, nicht design-driven

### 20.2 Float64 = Performance-optimiert
- Direkte double operations
- Array-Konstruktoren für compactness
- Parse-Methoden für convenience
- Static singletons für zero-overhead

### 20.3 Generic = Flexibilität-optimiert
- Maximale Type-Generizität (10 Scalar() Überladungen!)
- Explizite ScalarProcessor dependency injection
- Hybrid API (double, string, T, Scalar<T>, IScalar<T>)
- Zusätzliche PGa operations (Meet, Join, Polarity)

### 20.4 Migration Float64 → Generic
**Benötigt:**
1. Add `IScalarProcessor<T>` parameter
2. Replace `.Euclidean` → `.CreateEuclidean(scalarProcessor)`
3. Replace `double` → `T` (oder `Scalar<T>`, `IScalar<T>`)
4. Replace native operators → ScalarProcessor methods
5. Replace array constructors → composer pattern
6. Replace parse methods → custom parsing via ScalarProcessor

### 20.5 Hybrid API Usage (Generic)
**Best Practice:**
```csharp
// Nutze double für convenience
processor.Vector(1.0, 2.0, 3.0)

// Nutze T für type-safety
processor.Vector(t1, t2, t3)

// Nutze Scalar<T> für explizite scalar domain
processor.Vector(scalar1, scalar2, scalar3)

// Nutze IScalar<T> für interface abstraction
processor.Vector(iscalar1, iscalar2)
```

---

## Zusammenfassung

Die **Float64**- und **Generic**-Prozessor-APIs sind **strukturell identisch**, aber unterscheiden sich fundamental in:

1. **Type System:** `double` vs `T` mit wrapper-Typen
2. **Factory Pattern:** Static singletons vs factory methods mit dependency injection
3. **Scalar Operations:** Native operators vs ScalarProcessor methods
4. **API Surface:** Float64 hat convenience methods (Parse, array constructors), Generic hat type flexibility (10 overloads!)
5. **Specialized Processors:** Generic hat MEHR PGa operations, Float64 hat MEHR decode variants

**Empfehlung:**
- **Float64 für Performance-kritische, double-basierte Berechnungen**
- **Generic für Flexibilität, custom scalar types (rational, symbolic, etc.)**
- **Hybrid API (Generic)** erlaubt smooth transition zwischen convenience (double) und type-safety (T)
