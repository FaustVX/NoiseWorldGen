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
        _continentalnessNoise = new(1 / 100f, Seed);
        _continentalnessLerp = new(GetHeightProportion(.1), GetHeightProportion(.9))
        {
            { 0.3f, GetHeightProportion(.35) },
            { 0.5f, GetHeightProportion(.45) },
        };
        _noodleCaveNoise = new(1 / 100f, seed);
        var noodleCaveOffset = .05f;
        _noodleCaveLerp = new(0, 0)
        {
            { -noodleCaveOffset, 0 },
            { 0, 1 },
            { noodleCaveOffset, 0 },
        };
        _caveNoise = new(1 / 30f, seed);
        var caveOffset = .5f;
        _caveLerp = new(0, 1)
        {
            { caveOffset, 0 },
            { caveOffset + float.Epsilon, 1 },
        };
        _columns = new();

        int GetHeightProportion(double proportion)
            => (int)(Height * proportion);
    }

    public bool IsNoodleCave(int x, int y)
        => _noodleCaveLerp.Lerp(_noodleCaveNoise.Generate(x, y)) != 0;

    public bool IsCave(int x, int y)
        => _caveLerp.Lerp(_caveNoise.Generate(x, y)) != 0;

    public int GetBaseHeight(int x)
        => _continentalnessLerp.Lerp(_continentalnessNoise.Generate(x));
}
