# Code Style and Conventions

## Naming Conventions
- **Classes**: PascalCase (e.g., `XGaFloat64Processor`, `BasisBladeTests`)
- **Methods**: PascalCase (e.g., `TestIdGradeIndexConversion`, `ExecutePovRay`)
- **Properties**: PascalCase (e.g., `VSpaceDimensions`, `BasisSet`)
- **Parameters**: camelCase (e.g., `args`, `id`, `grade`)
- **Local Variables**: camelCase (e.g., `kvSpaceDimensions`, `equalFlag`)
- **Constants**: PascalCase or UPPER_CASE depending on context
- **Interfaces**: PascalCase with 'I' prefix (standard C# convention)

## Language Features
- **Nullable Reference Types**: Enabled (`<Nullable>enable</Nullable>`)
- **Implicit Usings**: Enabled (`<ImplicitUsings>enable</ImplicitUsings>`)
- **File-Scoped Namespaces**: Modern C# style
  ```csharp
  namespace GeometricAlgebraFulcrumLib.UnitTests.Processing;
  // No braces around namespace
  ```
- **Latest C# Version**: Uses `<LangVersion>latest</LangVersion>`

## Code Organization
- **One Class Per File**: Generally followed
- **Nested Classes**: Used for internal implementation details
- **Static Classes**: Used for utility methods and extension methods
- **Sealed Classes**: Used for test fixtures and final implementations

## Testing Patterns
- **Test Class Naming**: `<FeatureName>Tests` (e.g., `BasisBladeTests`, `RotorsTests`)
- **Test Method Naming**: Descriptive with `Test` prefix (e.g., `TestIdGradeIndexConversion`)
- **Attributes**: 
  - `[TestFixture]` for test classes
  - `[Test]` for test methods
- **Assertions**: Both `Debug.Assert()` and `Assert.That()` used together for debugging and testing

## Documentation
- XML documentation comments for public APIs (standard C# convention)
- Inline comments for complex algorithms
- Reference to external documentation when appropriate

## Code Structure Patterns
- **Expression-Bodied Members**: Used for simple properties and methods
  ```csharp
  public int VSpaceDimensions 
      => 6;
  ```
- **Property Initializers**: Used for readonly properties
  ```csharp
  public XGaFloat64Processor BasisSet { get; }
      = XGaFloat64Processor.Create(2, 2);
  ```

## Design Principles
- **Data-Oriented Programming (DOP)**: Separation of behavior and data
- **Immutability**: Preferred for data structures
- **Composer Pattern**: Used for object construction
- **Generic Programming**: Heavy use of generics for scalar abstraction
