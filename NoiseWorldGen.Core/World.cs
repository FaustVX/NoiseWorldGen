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
        MaxHeight = (int)(Height * .75);
        WaterLevel = (int)(Height * .5);
        MinHeight = (int)(Height * .1);
        Noise = new(Seed);
        HeightLerp = new(MinHeight, MaxHeight);
        _columns = new();
    }
}
