using SimplexNoise;

namespace NoiseWorldGen.Core;

public class World
{
    public int Seed { get; }
    public int Height { get; }
    public int WaterLevel { get; }
    private readonly Dictionary<int, Column> _columns;
    private readonly Noise _continentalnessNoise;
    private readonly Interpolation _continentalnessLerp;
    private readonly Noise _noodleCaveNoise;
    private readonly Interpolation _noodleCaveLerp;
    private readonly Noise _caveNoise;
    private readonly Interpolation _caveLerp;
    private readonly Noise _dirtNoise;
    private readonly Interpolation _dirtLerp;

    public Column this[int x]
        => _columns.TryGetValue(x, out var column)
            ? column
            : (_columns[x] = new(this, x));

    public ref Block this[int x, int y]
        => ref this[x].Blocks[y];

    public World(int seed, int maxHeight)
    {
        Seed = seed;
        Height = maxHeight;
        WaterLevel = GetHeightProportion(.25);
        _columns = new();

        (_continentalnessNoise, _continentalnessLerp) = CreateNoise(Seed ^ 1, 100, GetHeightProportion(.1), GetHeightProportion(.9), (.3f, GetHeightProportion(.35)), (.5f, GetHeightProportion(.45)));
        var noodleCaveOffset = .075f;
        (_noodleCaveNoise, _noodleCaveLerp) = CreateNoise(Seed ^ 2, 100, 0, 0, (-noodleCaveOffset, 0), (0, 1), (noodleCaveOffset, 0));
        var caveOffset = .75f;
        (_caveNoise, _caveLerp) = CreateNoise(Seed ^ 3, 100, 1, 1, (-caveOffset - float.Epsilon, 1), (-caveOffset, 0), (caveOffset, 0), (caveOffset + float.Epsilon, 1));
        (_dirtNoise, _dirtLerp) = CreateNoise(Seed ^ 4, 1, 1, 3);

        int GetHeightProportion(double proportion)
            => (int)(Height * proportion);

        static (Noise noise, Interpolation lerp) CreateNoise(int seed, int scale, int start, int end, params (float noise, int value)[] noiseValues)
        {
            var noise = new Noise(1f / scale, seed);
            var lerp = new Interpolation(start, end);
            foreach (var (n, v) in noiseValues)
                lerp.Add(n, v);
            return (noise, lerp);
        }
    }

    public bool IsNoodleCave(int x, int y)
        => _noodleCaveLerp.Lerp(_noodleCaveNoise.Generate(x, y)) != 0;

    public bool IsCave(int x, int y)
        => _caveLerp.Lerp(_caveNoise.Generate(x, y)) != 0;

    public int GetBaseHeight(int x)
        => _continentalnessLerp.Lerp(_continentalnessNoise.Generate(x));

    public int GetDirtHeight(int x)
        => _dirtLerp.Lerp(_dirtNoise.Generate(x));
}
