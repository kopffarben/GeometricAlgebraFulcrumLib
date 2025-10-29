using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Normalized;

/// <summary>
/// A normalized time signal has time in the range [-1, 1] and values in the range [-1, 1]
/// </summary>
public abstract class ScalarNormalizedSignal<T> :
    ScalarSignal<T>
{
    /// <summary>
    /// A normalized time signal has time in the range [-1, 1] and values in the range [-1, 1]
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected ScalarNormalizedSignal(IScalarProcessor<T> scalarProcessor, bool isPeriodic)
        : base(ScalarRange<T>.SymmetricOne(scalarProcessor), isPeriodic)
    {
    }
}
