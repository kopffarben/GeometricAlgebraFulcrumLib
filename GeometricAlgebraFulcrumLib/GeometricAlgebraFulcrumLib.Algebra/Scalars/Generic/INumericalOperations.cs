using System;

namespace GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

/// <summary>
/// Provides numerical operations for scalar types.
/// Different implementations exist for numeric types (double/float using Math.NET)
/// versus symbolic types (Entity using AngouriMath).
/// </summary>
/// <typeparam name="T">The scalar type</typeparam>
public interface INumericalOperations<T>
{
    /// <summary>
    /// Reference to the scalar processor that provides this numerical operations instance.
    /// </summary>
    IScalarProcessor<T> ScalarProcessor { get; }

    /// <summary>
    /// Compute the first derivative of a function at a given point.
    ///
    /// Implementation varies by scalar type:
    /// - For double/float: Uses Math.NET central finite differences (numerical approximation)
    ///   * Accuracy: ~1e-8 (double), ~1e-4 (float)
    ///   * Performance: Fast (~100ns per call)
    ///
    /// - For symbolic (Entity): Uses AngouriMath symbolic differentiation (exact)
    ///   * Accuracy: Exact (no approximation error)
    ///   * Performance: Slower (~1ms per call, but exact)
    /// </summary>
    /// <param name="function">The function to differentiate. Takes Scalar&lt;T&gt; input and returns Scalar&lt;T&gt; output.</param>
    /// <param name="point">The point at which to evaluate the derivative.</param>
    /// <returns>The derivative value at the specified point.</returns>
    /// <exception cref="ArgumentNullException">Thrown if function or point is null.</exception>
    Scalar<T> Differentiate(
        Func<Scalar<T>, Scalar<T>> function,
        Scalar<T> point
    );

    /// <summary>
    /// Compute the second derivative of a function at a given point.
    ///
    /// Uses the same backend as Differentiate() (Math.NET or AngouriMath depending on type).
    /// </summary>
    /// <param name="function">The function to differentiate twice.</param>
    /// <param name="point">The point at which to evaluate the second derivative.</param>
    /// <returns>The second derivative value at the specified point.</returns>
    /// <exception cref="ArgumentNullException">Thrown if function or point is null.</exception>
    Scalar<T> Differentiate2(
        Func<Scalar<T>, Scalar<T>> function,
        Scalar<T> point
    );

    /// <summary>
    /// Compute the definite integral of a function over the interval [a, b].
    ///
    /// Optional method - returns null if not implemented for this scalar type.
    ///
    /// For double/float: Would use Math.NET adaptive quadrature (not yet implemented).
    /// For symbolic (Entity): Uses AngouriMath symbolic integration (implemented).
    /// </summary>
    /// <param name="function">The function to integrate.</param>
    /// <param name="a">Lower bound of integration.</param>
    /// <param name="b">Upper bound of integration.</param>
    /// <returns>The definite integral value, or null if not implemented.</returns>
    Scalar<T>? Integrate(
        Func<Scalar<T>, Scalar<T>> function,
        Scalar<T> a,
        Scalar<T> b
    );

    /// <summary>
    /// Find a root of the equation function(x) = 0 near the initial guess.
    ///
    /// Optional method - returns null if not implemented for this scalar type.
    ///
    /// For double/float: Would use Math.NET root finding algorithms (not yet implemented).
    /// For symbolic (Entity): Would use AngouriMath equation solving (not yet implemented).
    /// </summary>
    /// <param name="function">The function whose root to find.</param>
    /// <param name="initialGuess">Initial guess for the root location.</param>
    /// <param name="tolerance">Optional tolerance for convergence. If null, uses default tolerance for the type.</param>
    /// <returns>The root value, or null if not implemented or not found.</returns>
    Scalar<T>? FindRoot(
        Func<Scalar<T>, Scalar<T>> function,
        Scalar<T> initialGuess,
        Scalar<T>? tolerance = null
    );
}
