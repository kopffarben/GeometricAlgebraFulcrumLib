namespace GeometricAlgebraFulcrumLib.Modeling.Signals;

/// <summary>
/// Interface for sampling specifications used by ScalarSignalSpectrum
/// </summary>
/// <typeparam name="TScalar">The scalar type (double for Float64, float for Float32)</typeparam>
public interface ISamplingSpecs<TScalar>
{
    /// <summary>
    /// The number of samples in the signal
    /// </summary>
    int SampleCount { get; }

    /// <summary>
    /// The sampling rate (samples per unit time)
    /// </summary>
    TScalar SamplingRate { get; }

    /// <summary>
    /// The frequency resolution (rad/sample)
    /// </summary>
    TScalar FrequencyResolution { get; }

    /// <summary>
    /// Get the frequency (in radians) for a given sample index
    /// </summary>
    TScalar GetFrequency(int index);

    /// <summary>
    /// Get the frequency (in Hz) for a given sample index
    /// </summary>
    TScalar GetFrequencyHz(int index);

    /// <summary>
    /// Check if the given sample index corresponds to DC (zero frequency)
    /// </summary>
    bool IsSampleIndexDc(int index);

    /// <summary>
    /// Check if the given sample index corresponds to AC (non-zero frequency)
    /// </summary>
    bool IsSampleIndexAc(int index);
}
