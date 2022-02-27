using SimplexNoise;

namespace NoiseWorldGen.Core;

public abstract record class NoiseSetting(Noise Noise, Interpolation Lerp)
{
    protected static T Create<T>(int seed, int scale, int start, int end, Func<Noise, Interpolation, T> ctor, params (float noise, int value)[] noiseValues)
        where T : NoiseSetting
    {
        seed ^= _settingId++;
        var noise = new Noise(1f / scale, seed);
        var lerp = new Interpolation(start, end);
        foreach (var (n, v) in noiseValues)
            lerp.Add(n, v);
        return ctor(noise, lerp);
    }
    private static int _settingId = 1;
}

public record class NoiseSetting1D(Noise Noise, Interpolation Lerp)
    : NoiseSetting(Noise, Lerp)
{
    public virtual int Generate(int x)
        => Lerp.Lerp(Noise.Generate(x));

    public static NoiseSetting1D Create(int seed, int scale, int start, int end, params (float noise, int value)[] noiseValues)
        => Create(seed, scale, start, end, (n, l) => new NoiseSetting1D(n, l), noiseValues);
}

public record class NoiseSetting2D(Noise Noise, Interpolation Lerp)
    : NoiseSetting(Noise, Lerp)
{
    public virtual int Generate(int x, int y)
        => Lerp.Lerp(Noise.Generate(x, y));

    public static NoiseSetting2D Create(int seed, int scale, int start, int end, params (float noise, int value)[] noiseValues)
        => Create(seed, scale, start, end, (n, l) => new NoiseSetting2D(n, l), noiseValues);
}

public record class NoiseSettingCave(Noise Noise, Interpolation Lerp)
    : NoiseSetting2D(Noise, Lerp)
{
    public new bool Generate(int x, int y)
        => base.Generate(x, y) != 0;

    public static new NoiseSettingCave Create(int seed, int scale, int start, int end, params (float noise, int value)[] noiseValues)
        => Create(seed, scale, start, end, (n, l) => new NoiseSettingCave(n, l), noiseValues);
}
