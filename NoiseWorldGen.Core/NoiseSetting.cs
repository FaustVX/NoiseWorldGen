using SimplexNoise;

namespace NoiseWorldGen.Core;

public readonly record struct NoiseSetting(Noise Noise, Interpolation Lerp)
{
    public int Generate(int x)
        => Lerp.Lerp(Noise.Generate(x));

    public int Generate(int x, int y)
        => Lerp.Lerp(Noise.Generate(x, y));

    public static NoiseSetting Create(int seed, int scale, int start, int end, params (float noise, int value)[] noiseValues)
    {
        seed ^= _settingId++;
        var noise = new Noise(1f / scale, seed);
        var lerp = new Interpolation(start, end);
        foreach (var (n, v) in noiseValues)
            lerp.Add(n, v);
        return new(noise, lerp);
    }
    private static int _settingId = 1;
}
