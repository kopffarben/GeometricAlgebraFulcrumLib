# Project Overview

## Name
Geometric Algebra Fulcrum Library (GA-FuL)

## Purpose
A unified, generic C# library for geometric algebra computations using any kind of scalars (floating point, rational, symbolic, etc.). The library provides a powerful mathematical framework that unifies many algebraic tools including vectors, complex numbers, quaternions, spinors, and matrices.

## Author
Ahmad H. Eid (ga.computing.eg@gmail.com)

## Key Features
1. **Generic Scalar Abstraction**: Support for floating point, arbitrary precision decimals, rational numbers, and symbolic expressions
2. **Memory-Efficient Sparse Multivectors**: Optimized data structures for high-dimensional geometric algebras (up to 64 dimensions optimized, arbitrary dimensions generic)
3. **Metaprogramming & Code Generation**: Automatic code generation from GA expressions with optimization
4. **Layered Architecture**: Algebra, Modeling, MetaProgramming, and System Utilities layers
5. **Data-Oriented Programming (DOP)**: Separation of behavior and data with immutable structures

## Application Domains
- Computer Graphics and Visualization
- Robotics and Motion Control
- Physics Simulations
- Signal and Image Processing
- Machine Learning and AI
- Computer Vision
- Mathematical Modeling
- High-Performance Computing

## Test Quality & Status (as of 2025-10-17)
- **Total Tests**: 1153
- **Pass Rate**: 97.92% (1129 passing, 0 failing, 24 skipped)
- **Test Coverage**: ~50% (growing rapidly)
- **Critical Bugs Fixed**: 10+ (GetBivector, Cp/Acp products, Grade Involution, etc.)
- **Documentation**: `docs/status/ISSUES_TO_FIX.md`, `docs/status/TODO_TEST_COVERAGE.md`, `DOCUMENTATION_INDEX.md`

**Test Suites:**
- Algebra: 133 tests (100% passing)
- LinearMaps: 121 tests (100% passing)
- AutoDiff: 69 tests (100% passing)
- Utilities: 295 tests (99.7% passing)
- Modeling (CGa): 507 tests (91% passing)

## Documentation
Complete documentation available at: https://kopffarben.github.io/GeometricAlgebraFulcrumLib/
Available in both English and German.

**Key Documentation Files:**
- `docs/guides/DEVELOPMENT_GUIDE.md` - Development guide for agents (includes test learnings)
- `README.md` - Project overview and quick start
- `docs/status/ISSUES_TO_FIX.md` - Known issues and bug tracking
- `docs/status/TODO_TEST_COVERAGE.md` - Test coverage plan and statistics
- `DOCUMENTATION_INDEX.md` - Central documentation registry
