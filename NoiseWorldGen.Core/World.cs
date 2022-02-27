using SimplexNoise;

namespace NoiseWorldGen.Core;

public class World
{
    public int Seed { get; }
    public int Height { get; }
    public int MaxHeight { get; }
    public int WaterLevel { get; }
    public int MinHeight { get; }
    private readonly Dictionary<int, Column> _columns;
    public Noise Noise { get; }
    public Interpolation HeightLerp { get; }

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
        MaxHeight = GetHeightProportion(.75);
        WaterLevel = GetHeightProportion(.35);
        MinHeight = GetHeightProportion(.1);
        Noise = new(Seed);
        HeightLerp = new(MinHeight, MaxHeight)
        {
            { 0.3f, GetHeightProportion(.4) },
            { 0.4f, GetHeightProportion(.65) },
        };
        _columns = new();

        int GetHeightProportion(double proportion)
            => (int)(Height * proportion);
    }
}
