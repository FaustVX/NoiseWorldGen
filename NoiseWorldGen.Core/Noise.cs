namespace NoiseWorldGen.Core;

public class Noise
{
    private readonly (double offset, double waveLength, double frequency, double amplitude)[] _sineSettings;

    public Noise(Random rng, int octaves, double maxAmplitude, double baseFrequancy)
    {
        _sineSettings = GenerateSetting(rng, octaves, maxAmplitude, baseFrequancy).ToArray();
    }

    private static IEnumerable<(double offset, double waveLength, double frequency, double amplitude)> GenerateSetting(Random rng, int octaves, double maxAmplitude, double baseFrequency)
    {
        var amplitudes = Enumerable.Range(0, octaves)
            .Select(static i => 1 / Math.Pow(2, i))
            .ToArray();
        var baseAmplitude = maxAmplitude / amplitudes.Sum();
        return amplitudes.Select((a, i) => (rng.NextDouble(), baseFrequency / a, baseAmplitude * a))
            .Select(static s => (s.Item1, 1 / s.Item2, s.Item2, s.Item3));
    }

    public double Generate(int x)
    {
        var value = 0d;
        foreach (var (offset, _, frequency, amplitude) in _sineSettings)
            value += Math.Sin(x * frequency + offset) * amplitude;
        return value;
    }

    public double Generate(int x, int y)
        => Generate(x) + Generate(y);

    public double Generate(int x, int y, int z)
        => Generate(x) + Generate(y) + Generate(z);
}