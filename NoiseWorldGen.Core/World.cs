using SimplexNoise;

namespace NoiseWorldGen.Core;

public class World
{
    public int Seed { get; }
    public int Height { get; }
    public int MaxHeight { get; }
    public int WaterLevel { get; }
    public int MinHeight { get; }
    private readonly List<Column> _columnsPositive = new(), _columnsNegative = new();
    public Noise Noise { get; }

    public Column this[int x]
    {
        get
        {
            (var columns, var newX) = x < 0 ? (_columnsNegative, -x - 1) : (_columnsPositive, x);
            for (var i = columns.Count; i <= newX; i++)
                GenerateColumn(x);
            return columns[newX];
        }
    }

    public Block this[int x, int y]
        => this[x].Blocks[y];

    public World(int seed, int maxHeight)
    {
        Seed = seed;
        Height = maxHeight;
        MaxHeight = (int)(Height * .75);
        WaterLevel = (int)(Height * .5);
        MinHeight = (int)(Height * .1);
        Noise = new(Seed);
        _columnsPositive = new();
    }

    private void GenerateColumn(int x)
    {
        var columns = x < 0 ? _columnsNegative : _columnsPositive;
        columns.Add(new(this, x));
    }
}
