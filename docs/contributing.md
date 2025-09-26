# Contributing to GA-FuL

Thank you for your interest in contributing to the Geometric Algebra Fulcrum Library! This document provides guidelines for contributing to the project.

## Getting Started

### Prerequisites
- .NET 7.0 or later
- Visual Studio 2022 or JetBrains Rider (recommended)
- Git for version control
- Basic understanding of Geometric Algebra concepts

### Development Setup
1. **Clone the repository**:
   ```bash
   git clone https://github.com/kopffarben/GeometricAlgebraFulcrumLib.git
   cd GeometricAlgebraFulcrumLib
   ```

2. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

3. **Build the solution**:
   ```bash
   dotnet build
   ```

4. **Run tests**:
   ```bash
   dotnet test
   ```

## Code Style and Standards

### C# Coding Conventions
- Follow Microsoft's C# coding conventions
- Use PascalCase for public members
- Use camelCase for private fields with underscore prefix (`_fieldName`)
- Use meaningful variable and method names
- Include XML documentation for all public APIs

### Example:
```csharp
/// <summary>
/// Computes the geometric product of two multivectors
/// </summary>
/// <param name="mv1">First multivector</param>
/// <param name="mv2">Second multivector</param>
/// <returns>The geometric product result</returns>
public XGaMultivector<T> GeometricProduct(XGaMultivector<T> mv1, XGaMultivector<T> mv2)
{
    if (mv1 == null) throw new ArgumentNullException(nameof(mv1));
    if (mv2 == null) throw new ArgumentNullException(nameof(mv2));
    
    return ComputeGeometricProduct(mv1, mv2);
}
```

### Performance Guidelines
- Use appropriate data structures for different scenarios
- Prefer immutable operations over mutable state
- Use `Span<T>` and `ReadOnlySpan<T>` for performance-critical operations
- Consider memory allocation patterns in hot paths

## Architecture Guidelines

### Layer Responsibilities
1. **Utilities Layer**: Foundation data structures, no dependencies on higher layers
2. **Algebra Layer**: Mathematical operations, depends only on Utilities
3. **Modeling Layer**: High-level abstractions, depends on Algebra and Utilities  
4. **MetaProgramming Layer**: Code generation, depends on all lower layers

### Adding New Features

#### New Scalar Types
To add support for a new scalar type:

1. **Implement `IScalarProcessor<T>`**:
   ```csharp
   public class ScalarProcessorOfMyType : IScalarProcessor<MyType>
   {
       public static ScalarProcessorOfMyType Instance { get; } = new();
       
       public MyType ZeroValue => MyType.Zero;
       public MyType OneValue => MyType.One;
       
       // Implement all interface methods
   }
   ```

2. **Add unit tests**:
   ```csharp
   [TestFixture]
   public class ScalarProcessorOfMyTypeTests
   {
       private ScalarProcessorOfMyType _processor;
       
       [SetUp]
       public void Setup()
       {
           _processor = ScalarProcessorOfMyType.Instance;
       }
       
       [Test]
       public void Add_TwoValues_ReturnsSum()
       {
           var result = _processor.Add(MyType.One, MyType.One);
           Assert.AreEqual(new MyType(2), result);
       }
   }
   ```

#### New Visualization Backends
To add a new visualization backend:

1. **Create backend-specific classes** in the appropriate namespace
2. **Implement core interfaces** for rendering and scene management
3. **Add integration examples** showing usage patterns
4. **Include performance benchmarks** if relevant

### Testing Requirements

#### Unit Tests
- All public methods must have unit tests
- Test edge cases and error conditions
- Use meaningful test names: `Method_Scenario_ExpectedBehavior`
- Include performance tests for critical operations

#### Integration Tests
- Test cross-layer interactions
- Verify code generation produces correct results
- Test platform integrations where applicable

#### Example Tests
```csharp
[Test]
public void CreateVector_ValidCoordinates_ReturnsCorrectVector()
{
    // Arrange
    var processor = XGaProcessor<double>.CreateEuclidean(ScalarProcessorOfFloat64.Instance);
    
    // Act
    var vector = processor.CreateVector(1, 2, 3);
    
    // Assert
    Assert.AreEqual(1.0, vector[0]);
    Assert.AreEqual(2.0, vector[1]);
    Assert.AreEqual(3.0, vector[2]);
}

[Test]
public void GeometricProduct_OrthogonalVectors_ReturnsExpectedBivector()
{
    var processor = XGaProcessor<double>.CreateEuclidean(ScalarProcessorOfFloat64.Instance);
    var e1 = processor.CreateVector(1, 0, 0);
    var e2 = processor.CreateVector(0, 1, 0);
    
    var result = e1.Gp(e2);
    
    Assert.True(result.GetBivectorPart().GetScalar(0, 1).IsNearEqual(1.0));
}
```

## Documentation Standards

### Code Documentation
- All public APIs must have XML documentation
- Include usage examples for complex operations
- Document performance characteristics where relevant
- Explain mathematical concepts when necessary

### README Updates
- Update README.md if adding major features
- Include installation and usage examples
- Keep the feature list current

### Example Documentation
```csharp
/// <summary>
/// Computes the outer product (wedge product) of two vectors, resulting in a bivector.
/// The outer product represents the oriented area spanned by the two vectors.
/// </summary>
/// <param name="vector2">The second vector in the outer product</param>
/// <returns>A bivector representing the oriented area</returns>
/// <example>
/// <code>
/// var e1 = processor.CreateVector(1, 0, 0);
/// var e2 = processor.CreateVector(0, 1, 0);
/// var bivector = e1.Op(e2); // Results in e12 basis bivector
/// </code>
/// </example>
/// <remarks>
/// The outer product is anticommutative: a ∧ b = -(b ∧ a)
/// Time complexity: O(n²) where n is the vector space dimension
/// </remarks>
public XGaBivector<T> Op(XGaVector<T> vector2)
```

## Pull Request Process

### Before Submitting
1. **Run all tests**: Ensure `dotnet test` passes completely
2. **Check code style**: Use built-in formatting tools
3. **Update documentation**: Include relevant documentation updates
4. **Add/update tests**: Ensure new code is properly tested

### Pull Request Template
```markdown
## Description
Brief description of changes made.

## Type of Change
- [ ] Bug fix (non-breaking change that fixes an issue)
- [ ] New feature (non-breaking change that adds functionality)  
- [ ] Breaking change (fix or feature that breaks existing functionality)
- [ ] Documentation update

## Testing
- [ ] Unit tests pass
- [ ] Integration tests pass
- [ ] Manual testing completed
- [ ] Performance impact evaluated

## Checklist
- [ ] Code follows project style guidelines
- [ ] Self-review completed
- [ ] Documentation updated
- [ ] Tests added/updated as needed
```

### Review Process
1. **Automated checks**: CI/CD pipeline runs automatically
2. **Code review**: Maintainers review for quality and design
3. **Testing verification**: All tests must pass
4. **Documentation review**: Ensure documentation is accurate and complete

## Issue Reporting

### Bug Reports
Use the following template for bug reports:

```markdown
## Bug Description
Clear and concise description of the bug.

## Steps to Reproduce
1. Go to '...'
2. Click on '...'
3. Scroll down to '...'
4. See error

## Expected Behavior
What you expected to happen.

## Actual Behavior
What actually happened.

## Environment
- OS: [e.g., Windows 11, macOS 12.0, Ubuntu 20.04]
- .NET Version: [e.g., .NET 7.0]
- GA-FuL Version: [e.g., 1.0.0]

## Additional Context
Any other context about the problem here.
```

### Feature Requests
```markdown
## Feature Description
Clear and concise description of the desired feature.

## Use Case
Describe the problem this feature would solve.

## Proposed Solution
Describe your preferred solution.

## Alternatives Considered
Other approaches you've considered.

## Additional Context
Any other context or screenshots about the feature request.
```

## Community Guidelines

### Code of Conduct
- Be respectful and inclusive
- Focus on constructive feedback
- Help newcomers learn the codebase
- Keep discussions focused on technical merit

### Communication Channels
- **Issues**: GitHub Issues for bugs and feature requests
- **Discussions**: GitHub Discussions for questions and ideas
- **Pull Requests**: For code contributions and reviews

### Recognition
Contributors are recognized in:
- Release notes for significant contributions
- Contributors section in README.md
- Git commit history and pull request records

## Development Workflow

### Branch Strategy
- `main`: Stable release branch
- `develop`: Integration branch for new features  
- `feature/*`: Individual feature branches
- `hotfix/*`: Critical bug fixes

### Commit Message Guidelines
```
type(scope): brief description

Longer explanation if needed.

- List changes
- Reference issues: Fixes #123
```

Types: `feat`, `fix`, `docs`, `style`, `refactor`, `test`, `chore`

### Release Process
1. **Feature freeze**: Stop adding new features
2. **Testing phase**: Comprehensive testing of all components
3. **Documentation update**: Ensure all docs are current
4. **Version tagging**: Create release tag with semantic versioning
5. **Package publishing**: Publish NuGet packages
6. **Release notes**: Document changes and breaking changes

Thank you for contributing to GA-FuL! Your contributions help make geometric algebra more accessible to developers and researchers worldwide.

---

**[← Previous: Integration](integration.md) | [Next: Home →](README.md)**