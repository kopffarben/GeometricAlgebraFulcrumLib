# Task Completion Checklist

When a task is completed, follow this checklist to ensure code quality and consistency:

## 1. Build the Code
Always build the project to ensure there are no compilation errors:
```powershell
dotnet build GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.sln
```

## 2. Run Tests
If tests are affected by your changes, run the test suite:
```powershell
dotnet test GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.sln
```

For specific tests:
```powershell
dotnet test GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.UnitTests/GeometricAlgebraFulcrumLib.UnitTests.csproj --filter "FullyQualifiedName~<TestName>"
```

## 3. Code Style Verification
Ensure code follows project conventions:
- PascalCase for classes, methods, properties
- camelCase for parameters and local variables
- File-scoped namespaces
- Nullable reference types handled correctly
- Expression-bodied members for simple properties

## 4. Documentation
- Add XML documentation comments for public APIs
- Update inline comments for complex algorithms
- Ensure naming is self-documenting

## 5. Git Operations
### Check what changed
```powershell
git status
git diff
```

### Stage and commit changes
```powershell
git add <files>
git commit -m "Descriptive commit message"
```

## 6. Specific Checks by Task Type

### For New Features
- [ ] Unit tests written and passing
- [ ] Public APIs documented
- [ ] Integration with existing code verified
- [ ] Examples or samples provided if applicable

### For Bug Fixes
- [ ] Root cause identified
- [ ] Test case added to prevent regression
- [ ] Fix verified with existing tests
- [ ] Related code reviewed for similar issues

### For Refactoring
- [ ] All tests still pass
- [ ] No breaking changes to public APIs
- [ ] Performance not degraded
- [ ] Code readability improved

### For Performance Improvements
- [ ] Benchmarks run before and after
- [ ] Results documented
- [ ] No breaking changes
- [ ] Tests still pass

## 7. Optional: Run Benchmarks
If performance-critical code was changed:
```powershell
dotnet run --project GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.Benchmarks/GeometricAlgebraFulcrumLib.Benchmarks.csproj --configuration Release
```

## 8. Review Before Committing
- [ ] No debug code left in
- [ ] No commented-out code (unless with explanation)
- [ ] No TODO comments without issue tracking
- [ ] Imports are organized
- [ ] No warnings in build output

## Notes
- The project does not have explicit linting/formatting tools configured
- Use IDE formatting features (Visual Studio, Rider, or VS Code with C# extension)
- Focus on consistency with existing code style
- When in doubt, follow the patterns in similar existing code
