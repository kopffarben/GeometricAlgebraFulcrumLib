# Test Insights and Best Practices

**Last Updated**: 2025-10-17
**Source**: Analysis of 1153 tests and 10+ critical bug fixes

## Critical Learnings from 1000+ Tests

### 1. Floating-Point Arithmetic Issues

**NEVER use exact zero comparisons** for floating-point results. Different storage implementations compute operations in different orders, leading to different rounding errors.

```csharp
// ❌ WRONG - Will fail sporadically
Assert.That(difference.IsZero);

// ✅ CORRECT - Use tolerance
const double tolerance = 1e-12;
Assert.That(difference.IsNearZero(tolerance));
```

**Typical error magnitude**: 1e-13 to 1e-15 when comparing Uniform, Graded, and Dense storage types.

**Where to apply**:
- All MultivectorStoragesTests
- Product operations (Gp, Cp, Acp)
- Self-operations like Gp(Reverse())
- Any test comparing numerical results

### 2. Test Isolation - Random Number Generators

**Test-order dependencies** are a major source of flaky tests. Always create fresh `Random` instances with explicit seeds.

```csharp
// ❌ WRONG - Shared state
public class MyTests
{
    private static Random _random = new Random(42);
    // State persists across tests!
}

// ✅ CORRECT - Per-test isolation
[Test]
public void TestSomething()
{
    var random = new Random(42);  // Fresh instance
    // Test is now deterministic and independent
}
```

**Bug found**: `BasisBladeTests.TestOddGradeInvolution` failed when run after certain other tests due to shared random state.

### 3. Critical API Bugs Fixed

#### Bug #1: GetBivector (CRITICAL - 13 tests blocked)
```csharp
// ❌ WRONG - Called wrong API
index.BasisVectorIndexToId()    // Creates single index

// ✅ CORRECT
index.BasisBivectorIndexToId()  // Creates pair of indices
```
**Impact**: Bivectors require TWO basis vectors (e.g., e₁∧e₂), not one!

#### Bug #2: Commutator/Anti-Commutator Products (4 tests)
```csharp
// ✅ CORRECT implementations
// Cp:  [A,B] = (AB - BA) / 2
// Acp: {A,B} = (AB + BA) / 2
```
These are NOT inner/outer product variations - they're simple algebraic formulas.

#### Bug #3: Grade Involution (reversed logic)
```csharp
// ❌ WRONG
grade % 2 == 0 ? scalar : -scalar

// ✅ CORRECT - Odd grades negated
grade % 2 == 0 ? scalar : scalar.Negative()
```

#### Bug #4: BitManipulation - GetNthSetBitPosition
Returned relative position instead of absolute position.

#### Bug #5: Combination.Choose(n, 0)
Returned n+1 instead of 1 (missing k=0 edge case check).

### 4. Known Library Limitations

#### CreatePureRotor with Antiparallel Vectors
**Fails with DebugAssertException** when vectors have angle ≈ 180°.

```csharp
// ⚠️ PROBLEMATIC - No angle check
var rotor = u1.CreatePureRotor(u2);

// ✅ SAFE - Check for antiparallel case
var cosAngle = u1.ESp(u2);
if (Math.Abs(cosAngle + 1.0) < 1e-10)
{
    // Skip or handle specially
    return;
}
var rotor = u1.CreatePureRotor(u2);
```

**Root cause**: `GetNormalVector()` creates circular dependency when finding perpendicular to antiparallel vectors.

**Impact**: Rotation tests become flaky with random vectors.

### 5. Testing Strategy by Priority

**Priority Levels**:
1. **P0 (Critical)**: Core algebra operations - 100% must pass
2. **P1 (High)**: LinearMaps, Storage consistency
3. **P2 (Medium)**: Edge cases, boundary conditions
4. **P3 (Low)**: Domain-specific (CGa), acceptable limitations
5. **P4 (Info)**: Known library bugs/edge cases

**Test Coverage Achieved**:
- Algebra: 133 tests (100% ✅)
- LinearMaps: 121 tests (100% ✅)
- AutoDiff: 69 tests (100% ✅)
- Utilities: 295 tests (99.7%)
- Modeling: 507 tests (91%)

### 6. Best Testing Patterns

#### Dual Assertions
```csharp
Debug.Assert(condition);    // Catches in development
Assert.That(condition);     // Catches in CI/CD
```
Both together provide best coverage.

#### Descriptive Failures
```csharp
Assert.That(result.IsNearZero(tolerance),
    $"Expected near-zero, got {result.Norm().ScalarValue}");
```
Always include actual values in failure messages.

#### Storage Consistency Testing
Test the same operation across all storage types:
- XGaUniformMultivector<T> (dictionary-based)
- XGaGradedMultivector<T> (grade-organized)
- RGaFloat64Multivector (dense arrays)

#### Edge Case Coverage
Always test:
- Dimension = 0
- Grade = 0
- Empty multivectors
- Antiparallel vectors (with proper guards)
- Sparse bit patterns (position ≠ index)

### 7. Test Documentation

**Primary References**:
- `ISSUES_TO_FIX.md` - All bugs with priority levels (0 failing!)
- `TODO_TEST_COVERAGE.md` - Coverage plan (2000+ lines)
- `DOCUMENTATION_INDEX.md` - Doc registry
- `UnitTests/KNOWN_ISSUES.md` - Library limitations

### 8. Common Test Anti-Patterns

❌ **Don't**:
- Use exact `IsZero` for floating-point
- Share random generators across tests
- Test implementation details instead of behavior
- Ignore edge cases (antiparallel vectors, etc.)
- Use hardcoded values without documenting why

✅ **Do**:
- Use tolerance-based comparisons
- Isolate test state
- Test mathematical properties
- Check preconditions for known limitations
- Document test purpose and expected values

### 9. Performance Testing Guidelines

- Always benchmark in **Release** mode
- Use **XGaFloat64Processor** (faster than generic XGaProcessor<double>)
- Reuse processors (they cache constants)
- Test with realistic sparse data
- Measure memory allocation patterns

### 10. Continuous Improvement

**When adding new tests**:
1. Check ISSUES_TO_FIX.md for known problems
2. Review TODO_TEST_COVERAGE.md for priority areas
3. Follow existing test patterns
4. Add descriptive comments
5. Update documentation when finding bugs

**When fixing bugs**:
1. Add regression test first
2. Document the bug in ISSUES_TO_FIX.md
3. Update CLAUDE.md if it's a common pitfall
4. Update serena memories with learnings
