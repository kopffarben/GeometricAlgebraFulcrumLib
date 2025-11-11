using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Geometry.AffineMaps.Space1D;

/// <summary>
/// Represents a 1D affine transformation: f(x) = scaling * x + offset
/// </summary>
/// <typeparam name="T">The scalar type</typeparam>
public sealed class AffineMap1D<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AffineMap1D<T> Identity(IScalarProcessor<T> scalarProcessor)
    {
        return new AffineMap1D<T>(
            scalarProcessor,
            scalarProcessor.One.ScalarValue,
            scalarProcessor.Zero.ScalarValue
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AffineMap1D<T> Reflection(IScalarProcessor<T> scalarProcessor)
    {
        return new AffineMap1D<T>(
            scalarProcessor,
            scalarProcessor.MinusOne.ScalarValue,
            scalarProcessor.Zero.ScalarValue
        );
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AffineMap1D<T> CreateScale(IScalarProcessor<T> scalarProcessor, Scalar<T> scaling)
    {
        if (scaling.IsOne())
            return Identity(scalarProcessor);

        if (scaling.IsMinusOne())
            return Reflection(scalarProcessor);

        return new AffineMap1D<T>(scalarProcessor, scaling.ScalarValue, scalarProcessor.Zero.ScalarValue);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AffineMap1D<T> CreateTranslate(IScalarProcessor<T> scalarProcessor, Scalar<T> offset)
    {
        return offset.IsZero()
            ? Identity(scalarProcessor)
            : new AffineMap1D<T>(scalarProcessor, scalarProcessor.One.ScalarValue, offset.ScalarValue);
    }


    public static AffineMap1D<T> CreateFromRanges(IScalarProcessor<T> scalarProcessor, Scalar<T> inputValue1, Scalar<T> inputValue2, Scalar<T> outputValue1, Scalar<T> outputValue2)
    {
        var dtInv = scalarProcessor.Divide(
            scalarProcessor.One.ScalarValue,
            scalarProcessor.Subtract(inputValue2.ScalarValue, inputValue1.ScalarValue).ScalarValue
        );

        var scaling = scalarProcessor.Times(
            scalarProcessor.Subtract(outputValue2.ScalarValue, outputValue1.ScalarValue).ScalarValue,
            dtInv.ScalarValue
        );

        var offset = scalarProcessor.Times(
            scalarProcessor.Subtract(
                scalarProcessor.Times(inputValue2.ScalarValue, outputValue1.ScalarValue).ScalarValue,
                scalarProcessor.Times(inputValue1.ScalarValue, outputValue2.ScalarValue).ScalarValue
            ).ScalarValue,
            dtInv.ScalarValue
        );

        return new AffineMap1D<T>(scalarProcessor, scaling.ScalarValue, offset.ScalarValue);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AffineMap1D<T> Create(IScalarProcessor<T> scalarProcessor, Scalar<T> scaling, Scalar<T> offset)
    {
        if (scaling.IsOne())
            return offset.IsZero()
                ? Identity(scalarProcessor)
                : new AffineMap1D<T>(scalarProcessor, scaling.ScalarValue, offset.ScalarValue);

        if (scaling.IsMinusOne())
            return offset.IsZero()
                ? Reflection(scalarProcessor)
                : new AffineMap1D<T>(scalarProcessor, scaling.ScalarValue, offset.ScalarValue);

        return new AffineMap1D<T>(scalarProcessor, scaling.ScalarValue, offset.ScalarValue);
    }


    public IScalarProcessor<T> ScalarProcessor { get; }

    public Scalar<T> Scaling { get; }

    public Scalar<T> Offset { get; }

    public Scalar<T> this[Scalar<T> t]
        => ScalarProcessor.Add(
            ScalarProcessor.Times(Scaling.ScalarValue, t.ScalarValue).ScalarValue,
            Offset.ScalarValue
        );

    public bool IsReflection
        => Offset.IsZero() && Scaling.IsMinusOne();

    public bool SwapsHandedness
        => Scaling.IsNegative();


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private AffineMap1D(IScalarProcessor<T> scalarProcessor, T scaling, T offset)
    {
        ScalarProcessor = scalarProcessor;

        if (!scalarProcessor.IsValid(offset) || !scalarProcessor.IsNumber(offset))
            throw new ArgumentOutOfRangeException(nameof(offset));

        if (scalarProcessor.IsZero(scaling) || !scalarProcessor.IsValid(scaling) || !scalarProcessor.IsNumber(scaling))
            throw new ArgumentOutOfRangeException(nameof(scaling));

        Offset = scalarProcessor.Scalar(offset);
        Scaling = scalarProcessor.Scalar(scaling);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Deconstruct(out Scalar<T> scaling, out Scalar<T> offset)
    {
        scaling = Scaling;
        offset = Offset;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsValid()
    {
        return Offset.IsValid() &&
               Scaling.IsValid() &&
               !Scaling.IsZero();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsIdentity()
    {
        return Offset.IsZero() && Scaling.IsOne();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsEqual(AffineMap1D<T> p2)
    {
        return ScalarProcessor.Subtract(Scaling.ScalarValue, p2.Scaling.ScalarValue).IsZero() &&
               ScalarProcessor.Subtract(Offset.ScalarValue, p2.Offset.ScalarValue).IsZero();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> MapPoint(Scalar<T> point)
    {
        return ScalarProcessor.Add(
            ScalarProcessor.Times(Scaling.ScalarValue, point.ScalarValue).ScalarValue,
            Offset.ScalarValue
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> MapVector(Scalar<T> vector)
    {
        return ScalarProcessor.Times(Scaling.ScalarValue, vector.ScalarValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public AffineMap1D<T> GetInverseAffineMap()
    {
        // Inverse of t' = a*t + b is: t = (1/a)*t' + (-b/a)
        var invScaling = ScalarProcessor.Divide(
            ScalarProcessor.One.ScalarValue,
            Scaling.ScalarValue
        );
        var invOffset = ScalarProcessor.Divide(
            ScalarProcessor.Negative(Offset.ScalarValue).ScalarValue,
            Scaling.ScalarValue
        );

        return new AffineMap1D<T>(ScalarProcessor, invScaling.ScalarValue, invOffset.ScalarValue);
    }


    public override string ToString()
    {
        var scalingText =
            Scaling.IsOne()
                ? "x"
                : Scaling.IsMinusOne() ? "-x" : $"{Scaling} x";

        if (Offset.IsZero())
            return scalingText;

        return Offset.IsPositive()
            ? $"{scalingText} + {Offset}"
            : $"{scalingText} - {Offset.Negative()}";
    }
}
