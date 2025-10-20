using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;

namespace GeometricAlgebraFulcrumLib.Algebra.Scalars.Float32;

public sealed partial class Float32ScalarComposer
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Float32ScalarComposer Create()
    {
        return new Float32ScalarComposer();
    }


    private float _scalarValue;

    public float ScalarValue
    {
        get => _scalarValue;
        set
        {
            if (float.IsNaN(value))
                throw new InvalidOperationException();

            _scalarValue = value;
        }
    }

    public bool IsZero
        => _scalarValue == 0f;


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Float32ScalarComposer()
    {
        _scalarValue = 0f;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsValid()
    {
        return !float.IsNaN(_scalarValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Float32ScalarComposer Clear()
    {
        _scalarValue = 0f;

        return this;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float GetScalarValue()
    {
        return _scalarValue;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Float32ScalarComposer SetScalarValue(float scalarValue)
    {
        if (float.IsNaN(scalarValue))
            throw new InvalidOperationException();

        _scalarValue = scalarValue;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Float32ScalarComposer SetScalarValueNegative(float scalarValue)
    {
        return SetScalarValue(-scalarValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Float32ScalarComposer SetScalarValue(float scalarValue, float scalingFactor)
    {
        return SetScalarValue(scalarValue * scalingFactor);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Float32ScalarComposer AddScalarValue(float scalarValue)
    {
        return SetScalarValue(_scalarValue + scalarValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Float32ScalarComposer AddScalarValues(params float[] scalarList)
    {
        foreach (var scalarValue in scalarList)
            AddScalarValue(scalarValue);

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Float32ScalarComposer AddScalarValues(IEnumerable<float> scalarList)
    {
        foreach (var scalarValue in scalarList)
            AddScalarValue(scalarValue);

        return this;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Float32ScalarComposer SubtractScalarValue(float scalarValue)
    {
        return SetScalarValue(_scalarValue - scalarValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Float32ScalarComposer SubtractScalarValues(params float[] scalarList)
    {
        foreach (var scalarValue in scalarList)
            SubtractScalarValue(scalarValue);

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Float32ScalarComposer SubtractScalarValues(IEnumerable<float> scalarList)
    {
        foreach (var scalarValue in scalarList)
            SubtractScalarValue(scalarValue);

        return this;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Float32ScalarComposer MapScalar(Func<float, float> mappingFunction)
    {
        return SetScalarValue(
            mappingFunction(_scalarValue)
        );
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Float32ScalarComposer Negative()
    {
        return SetScalarValue(-_scalarValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Float32ScalarComposer Times(float scalarFactor)
    {
        return SetScalarValue(_scalarValue * scalarFactor);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Float32ScalarComposer Divide(float scalarFactor)
    {
        return SetScalarValue(_scalarValue / scalarFactor);
    }
}
