using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Utilities.Structures;
using GeometricAlgebraFulcrumLib.Utilities.Structures.Tuples;

namespace GeometricAlgebraFulcrumLib.Modeling.Signals;

public abstract class ScalarSignalSpectrum<T, TScalar, TSamplingSpecs> :
    IReadOnlyList<ScalarSignalSpectrum<T, TScalar, TSamplingSpecs>.SignalSpectrumSample>
    where TSamplingSpecs : ISamplingSpecs<TScalar>
{
    public sealed record SignalSpectrumSample(int Index, T Value);


    protected readonly Dictionary<int, SignalSpectrumSample> IndexSampleDictionary;


    public int Count
        => SamplingSpecs.SampleCount;

    public TSamplingSpecs SamplingSpecs { get; }

    public int SampleCount
        => SamplingSpecs.SampleCount;

    public TScalar SamplingRate
        => SamplingSpecs.SamplingRate;

    public TScalar FrequencyResolution
        => SamplingSpecs.FrequencyResolution;

    public double FrequencyResolutionHz
        => SamplingSpecs.FrequencyResolutionHz;

    public T ValueSum
        => IndexSampleDictionary
            .Values
            .Select(v => v.Value)
            .Aggregate(ZeroValue, Add);

    public IEnumerable<SignalSpectrumSample> Samples
        => IndexSampleDictionary.Values;

    public IEnumerable<SignalSpectrumSample> SamplesDc
        => IndexSampleDictionary
            .Values
            .Where(spectrumSample => IsSampleIndexDc(spectrumSample.Index));

    public IEnumerable<SignalSpectrumSample> SamplesAc
        => IndexSampleDictionary
            .Values
            .Where(spectrumSample => IsSampleIndexAc(spectrumSample.Index));

    public IEnumerable<Pair<SignalSpectrumSample>> SamplePairsAc
    {
        get
        {
            for (var i1 = 1; i1 < SampleCount; i1++)
            {
                var i2 = SampleCount - i1;

                if (i2 < i1)
                    break;

                yield return new Pair<SignalSpectrumSample>(this[i1], this[i2]);
            }
        }
    }

    public IEnumerable<int> FrequencyIndices
        => Samples.Select(r => r.Index);

    public IEnumerable<TScalar> Frequencies
        => Samples.Select(r => GetFrequency(r.Index));

    public IEnumerable<TScalar> FrequenciesHz
        => Samples.Select(r => GetFrequencyHz(r.Index));

    public TScalar FrequencyMin
        => Frequencies.Min();

    public TScalar FrequencyMinHz
        => FrequenciesHz.Min();

    public TScalar FrequencyMax
        => Frequencies.Max();

    public TScalar FrequencyMaxHz
        => FrequenciesHz.Max();

    public Pair<TScalar> FrequencyRange
        => Frequencies.GetRange();

    public Pair<TScalar> FrequencyRangeHz
        => FrequenciesHz.GetRange();

    public SignalSpectrumSample this[int index]
    {
        get => GetSample(index);
        set => SetSample(index, value);
    }


    protected abstract T ZeroValue { get; }

    protected abstract bool IsZeroValue(T value);

    protected abstract T Negative(T value);

    protected abstract T Add(T value1, T value2);

    protected abstract T Subtract(T value1, T value2);

    protected abstract T Times(T value1, T value2);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected ScalarSignalSpectrum(TSamplingSpecs samplingSpecs)
    {
        SamplingSpecs = samplingSpecs;
        IndexSampleDictionary = new Dictionary<int, SignalSpectrumSample>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected ScalarSignalSpectrum(TSamplingSpecs samplingSpecs, Dictionary<int, SignalSpectrumSample> indexSampleDictionary)
    {
        Debug.Assert(
            indexSampleDictionary.Keys.All(
                index => index >= 0 && index < samplingSpecs.SampleCount
            )
        );

        SamplingSpecs = samplingSpecs;
        IndexSampleDictionary = indexSampleDictionary;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsSampleIndexDc(int index)
    {
        return SamplingSpecs.IsSampleIndexDc(index);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsSampleIndexAc(int index)
    {
        return SamplingSpecs.IsSampleIndexAc(index);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TScalar GetFrequency(int index)
    {
        return SamplingSpecs.GetFrequency(index);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TScalar GetFrequencyHz(int index)
    {
        return SamplingSpecs.GetFrequencyHz(index);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ScalarSignalSpectrum<T, TScalar, TSamplingSpecs> Clear()
    {
        IndexSampleDictionary.Clear();

        return this;
    }

    public ScalarSignalSpectrum<T, TScalar, TSamplingSpecs> RemoveZeroValueSamples()
    {
        var indexArray =
            IndexSampleDictionary
                .Where(p => IsZeroValue(p.Value.Value))
                .Select(p => p.Key)
                .ToArray();

        foreach (var index in indexArray)
            IndexSampleDictionary.Remove(index);

        return this;
    }

    public ScalarSignalSpectrum<T, TScalar, TSamplingSpecs> RemoveHighFrequencySamples(TScalar cutoffFrequency)
    {
        var indexSet = new HashSet<int>();

        foreach (var (sample1, sample2) in SamplePairsAc)
        {
            var freq1 = GetFrequency(sample1.Index).Abs();
            var freq2 = GetFrequency(sample2.Index).Abs();

            if (freq1 > cutoffFrequency || freq2 > cutoffFrequency)
            {
                indexSet.Add(sample1.Index);
                indexSet.Add(sample2.Index);
            }
        }

        foreach (var index in indexSet)
            IndexSampleDictionary.Remove(index);

        return this;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SignalSpectrumSample GetSample(int index)
    {
        if (index < 0 || index >= SampleCount)
            index = index.Mod(SampleCount);

        return IndexSampleDictionary.TryGetValue(index, out var record)
            ? record
            : new SignalSpectrumSample(index, ZeroValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetSample(int index, SignalSpectrumSample sample)
    {
        if (index < 0 || index >= SampleCount)
            index = index.Mod(SampleCount);

        var record = sample ?? new SignalSpectrumSample(index, ZeroValue);

        if (IndexSampleDictionary.ContainsKey(index))
            IndexSampleDictionary[index] = record;
        else
            IndexSampleDictionary.Add(index, record);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ScalarSignalSpectrum<T, TScalar, TSamplingSpecs> Set(int index, T value)
    {
        if (index < 0 || index >= SampleCount)
            index = index.Mod(SampleCount);

        if (IndexSampleDictionary.ContainsKey(index))
            IndexSampleDictionary[index] = new SignalSpectrumSample(index, value);
        else
            IndexSampleDictionary.Add(index, new SignalSpectrumSample(index, value));

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ScalarSignalSpectrum<T, TScalar, TSamplingSpecs> Set(SignalSpectrumSample spectrumSample)
    {
        return Set(spectrumSample.Index, spectrumSample.Value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ScalarSignalSpectrum<T, TScalar, TSamplingSpecs> Set(IEnumerable<SignalSpectrumSample> spectrumSamples)
    {
        foreach (var (index, value) in spectrumSamples)
            Set(index, value);

        return this;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ScalarSignalSpectrum<T, TScalar, TSamplingSpecs> Add(int index, T value)
    {
        if (index < 0 || index >= SampleCount)
            index = index.Mod(SampleCount);

        if (IndexSampleDictionary.TryGetValue(index, out var record))
            IndexSampleDictionary[index] = new SignalSpectrumSample(index, Add(record.Value, value));
        else
            IndexSampleDictionary.Add(index, new SignalSpectrumSample(index, value));

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ScalarSignalSpectrum<T, TScalar, TSamplingSpecs> Add(SignalSpectrumSample spectrumSample)
    {
        return Add(spectrumSample.Index, spectrumSample.Value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ScalarSignalSpectrum<T, TScalar, TSamplingSpecs> Add(IEnumerable<SignalSpectrumSample> spectrumSamples)
    {
        foreach (var (index, value) in spectrumSamples)
            Add(index, value);

        return this;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ScalarSignalSpectrum<T, TScalar, TSamplingSpecs> Subtract(int index, T value)
    {
        if (index < 0 || index >= SampleCount)
            index = index.Mod(SampleCount);

        if (IndexSampleDictionary.TryGetValue(index, out var record))
            IndexSampleDictionary[index] = new SignalSpectrumSample(index, Subtract(record.Value, value));
        else
            IndexSampleDictionary.Add(index, new SignalSpectrumSample(index, Negative(value)));

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ScalarSignalSpectrum<T, TScalar, TSamplingSpecs> Subtract(SignalSpectrumSample spectrumSample)
    {
        return Subtract(spectrumSample.Index, spectrumSample.Value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ScalarSignalSpectrum<T, TScalar, TSamplingSpecs> Subtract(IEnumerable<SignalSpectrumSample> spectrumSamples)
    {
        foreach (var (index, value) in spectrumSamples)
            Subtract(index, value);

        return this;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ScalarSignalSpectrum<T, TScalar, TSamplingSpecs> MapValues(Func<T, T> valueMapping)
    {
        var indexSampleDictionary = IndexSampleDictionary.ToDictionary(
            p => p.Key,
            p => new SignalSpectrumSample(
                p.Value.Index,
                valueMapping(p.Value.Value)
            )
        );

        return CreateSignalSpectrum(indexSampleDictionary).RemoveZeroValueSamples();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ScalarSignalSpectrum<T, TScalar, TSamplingSpecs> MapValuesByIndexValue(Func<int, T, T> indexValueMapping)
    {
        var indexSampleDictionary = IndexSampleDictionary.ToDictionary(
            p => p.Key,
            p => new SignalSpectrumSample(
                p.Value.Index,
                indexValueMapping(p.Value.Index, p.Value.Value)
            )
        );

        return CreateSignalSpectrum(indexSampleDictionary).RemoveZeroValueSamples();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ScalarSignalSpectrum<T, TScalar, TSamplingSpecs> MapValuesByFrequencyValue(Func<double, T, T> frequencyValueMapping)
    {
        var indexSampleDictionary = IndexSampleDictionary.ToDictionary(
            p => p.Key,
            p => new SignalSpectrumSample(
                p.Value.Index,
                frequencyValueMapping(p.Value.Index * FrequencyResolution, p.Value.Value)
            )
        );

        return CreateSignalSpectrum(indexSampleDictionary).RemoveZeroValueSamples();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ScalarSignalSpectrum<T, TScalar, TSamplingSpecs> GetCopy()
    {
        var indexSampleDictionary = IndexSampleDictionary.ToDictionary(
            p => p.Key,
            p => p.Value
        );

        return CreateSignalSpectrum(indexSampleDictionary).RemoveZeroValueSamples();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ScalarSignalSpectrum<T, TScalar, TSamplingSpecs> ScaleBy(T scalingFactor)
    {
        return MapValues(value => Times(value, scalingFactor));
    }

    protected ScalarSignalSpectrum<T, TScalar, TSamplingSpecs> CreateSignalSpectrum(Dictionary<int, SignalSpectrumSample> indexSampleDictionary)
    {
        return CreateSignalSpectrum(SamplingSpecs, indexSampleDictionary);
    }

    protected abstract ScalarSignalSpectrum<T, TScalar, TSamplingSpecs> CreateSignalSpectrum(TSamplingSpecs samplingSpecs, Dictionary<int, SignalSpectrumSample> indexSampleDictionary);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerator<SignalSpectrumSample> GetEnumerator()
    {
        return Enumerable.Range(0, Count).Select(i => this[i]).GetEnumerator();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString()
    {
        return IndexSampleDictionary
            .Values
            .OrderBy(spectrumSample =>
                GetFrequencyHz(spectrumSample.Index).Abs()
            )
            .ThenBy(spectrumSample =>
                GetFrequencyHz(spectrumSample.Index).Sign()
            )
            .Select(spectrumSample =>
                $"({spectrumSample.Value}) Exp[2π({GetFrequencyHz(spectrumSample.Index):G})i t]"
            )
            .ConcatenateText($" + {Environment.NewLine}");
    }
}