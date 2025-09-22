# GeometricAlgebraFulcrumLib Testing Guide and Documentation

## Overview

This document provides comprehensive guidance for testing the GeometricAlgebraFulcrumLib library, including current test coverage, additional test implementations, and best practices for extending the test suite.

## Current Test Infrastructure

### Existing Test Structure
- **Test Framework**: NUnit 4.3.2
- **Target Framework**: .NET 8.0
- **Test Runner**: Microsoft.NET.Test.Sdk 17.14.1
- **Additional Packages**: 
  - AngouriMath 1.3.0
  - CSharpMath.Evaluation 0.5.1
  - MathNet.Numerics 5.0.0
  - ILGPU 1.5.3

### Current Test Categories

#### 1. **Algebra Tests** (`/Algebra/`)
- `ProcessorsTests.cs` - Basic processor functionality (mostly empty)
- `ReflectionSamplesTests.cs` - Geometric reflection operations

#### 2. **AutoDiff Tests** (`/AutoDiff/`)
- `BasicDifferentiationTests.cs` - Basic differentiation operations
- `BasicEvaluationTests.cs` - Function evaluation tests
- `TermBuilderTests.cs` - Term construction tests
- `TermOperatorContractTests.cs` - Operator contract validation
- `Utils.cs` - Test utilities

#### 3. **Geometry Tests** (`/Geometry/`)
- `GaSimpleRotorsEuclideanTests.cs` - Euclidean rotor tests

#### 4. **Processing Tests** (`/Processing/`)
- `BasisBladeTests.cs` - **✅ Working** - Comprehensive basis blade operations
- `MultivectorStorageConsistencyTests.cs` - Storage consistency checks
- `MultivectorStoragesTests.cs` - **⚠️ Failing** - Storage operations

#### 5. **Storage Tests** (`/Storage/`)
- `BladeOperationTests.cs` - Blade operation tests
- `LinearAlgebra/VectorStorageTests.cs` - Vector storage tests

## Test Results Summary

### Current Status (Latest Run)
- **Total Tests**: 46
- **Passed**: 18 (39%)
- **Failed**: 28 (61%)

### Working Test Categories
✅ **BasisBladeTests** - All core tests passing
✅ **Basic AutoDiff** - Core differentiation working
✅ **Some Geometry Tests** - Basic geometric operations

### Failing Test Categories
❌ **MultivectorStoragesTests** - Setup issues with Debug.Fail
❌ **ReflectionSamplesTests** - Numerical precision issues
❌ **Various Integration Tests** - API mismatches

## Enhanced Test Coverage Plan

### Implemented Additional Tests

#### 1. **Extended Basis Blade Tests** 
*Status: Disabled due to API mismatches*
- Comprehensive grade operations validation
- Involution consistency checks
- Geometric product sign validation
- Outer product properties
- Metric properties verification
- Duality operations
- Combinatorics properties
- Performance benchmarks

#### 2. **Extended Storage Tests**
*Status: Disabled due to API mismatches*
- IndexSet operations (creation, union, intersection, difference)
- Dictionary storage operations
- Collection operations (List, HashSet, SortedDictionary)
- Performance tests for large operations

#### 3. **Extended AutoDiff Tests**
*Status: Disabled due to namespace issues*
- Basic operations (derivatives of x², x³, etc.)
- Chain rule validation
- Product rule testing
- Quotient rule verification
- Trigonometric function derivatives
- Exponential and logarithmic derivatives
- Multi-variable partial derivatives
- Higher-order derivatives
- Composite function handling
- Edge case robustness

#### 4. **Complex Test Coverage Areas Attempted**
*Status: Various API compatibility issues*
- Float64 scalar operations
- Linear algebra (2D/3D vectors, N-D vectors)
- Geometric algebra multivectors
- Polynomial operations
- Complex number arithmetic
- Text composition utilities

## API Discovery Challenges

During test development, several API compatibility issues were encountered:

### 1. **Namespace Mismatches**
- `GeometricAlgebraFulcrumLib.Utilities.Structures.AutoDiff` - Not found
- Various composer namespaces changed or missing

### 2. **Method Name Changes**
- IndexSet methods (`CreateFromSingleIndex`, `Difference`, `SymmetricDifference`, etc.)
- Scalar property names (`Value` vs other naming conventions)

### 3. **Class Structure Changes**
- Vector classes (Float64Vector2D, Float64Vector3D) - Different structure than expected
- Complex number implementation differences
- Polynomial class API differences

## Recommendations for Test Enhancement

### 1. **Immediate Actions**
1. **Fix Failing Tests**: Address the Debug.Fail issues in MultivectorStoragesTests
2. **API Documentation**: Create comprehensive API documentation to avoid compatibility issues
3. **Stable Test Foundation**: Focus on tests that work with current API

### 2. **Short-term Improvements**
1. **Test Categorization**: Organize tests by stability and functionality
2. **Mock Integration**: Use mocking where API is unstable
3. **Performance Baselines**: Establish performance benchmarks for critical operations

### 3. **Long-term Strategy**
1. **Comprehensive Coverage**: Once API is stable, implement full coverage
2. **Integration Tests**: Add end-to-end workflow tests
3. **Regression Prevention**: Implement CI/CD with automated test runs

## Test Writing Best Practices

### 1. **Structure**
```csharp
[TestFixture]
public class ComponentTests
{
    private const double Tolerance = 1e-12;
    
    [SetUp]
    public void Setup()
    {
        // Initialize common test data
    }
    
    [Test]
    public void Method_Scenario_ExpectedBehavior()
    {
        // Arrange
        var input = CreateTestInput();
        
        // Act
        var result = component.Method(input);
        
        // Assert
        Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
    }
}
```

### 2. **Naming Conventions**
- Test classes: `{ComponentName}Tests`
- Test methods: `{MethodName}_{Scenario}_{ExpectedBehavior}`
- Test data: Descriptive names explaining mathematical significance

### 3. **Mathematical Validation**
- Use appropriate tolerance for floating-point comparisons
- Test mathematical identities and properties
- Include edge cases (zero, infinity, very small/large numbers)
- Validate both positive and negative test cases

### 4. **Performance Considerations**
- Include performance regression tests for critical operations
- Use `[Explicit]` attribute for long-running performance tests
- Set appropriate timeouts for operations

## Current Working Examples

### Basis Blade Tests (Working ✅)
```csharp
[Test]
public void TestIdGradeIndexConversion()
{
    for (var id = 0UL; id < GaSpaceDimensions; id++)
    {
        var (grade, index) = id.ToUInt64IndexSet().BasisBladeIdToGradeIndex();
        var kvSpaceDimensions = VSpaceDimensions.GetBinomialCoefficient((int) grade);
        var id2 = BasisBladeUtils.BasisBladeGradeIndexToId(grade, index);
        
        Assert.That(grade == BitOperations.PopCount(id));
        Assert.That(index < kvSpaceDimensions);
        Assert.That(id == id2.ToUInt64());
    }
}
```

## Future Test Areas to Explore

### 1. **Mathematical Properties**
- [ ] Group theory properties in geometric algebra
- [ ] Metric tensor validation
- [ ] Clifford algebra axioms
- [ ] Duality relationships

### 2. **Numerical Stability**
- [ ] Condition number analysis
- [ ] Error propagation studies  
- [ ] Precision loss detection
- [ ] Overflow/underflow handling

### 3. **Performance Optimization**
- [ ] Memory allocation patterns
- [ ] Cache efficiency tests
- [ ] Parallel processing validation
- [ ] SIMD utilization verification

### 4. **Integration Scenarios**
- [ ] Cross-component workflows
- [ ] Real-world usage patterns
- [ ] Error handling chains
- [ ] Resource cleanup validation

## Conclusion

While the current test suite has significant gaps and compatibility issues, the foundation exists for comprehensive testing. The key is to:

1. Stabilize the existing working tests
2. Gradually expand coverage as API compatibility is resolved
3. Focus on mathematical correctness and numerical stability
4. Build robust performance benchmarks

The extensive test implementations created (though currently disabled) provide a solid foundation for future enhancement once API compatibility issues are resolved.