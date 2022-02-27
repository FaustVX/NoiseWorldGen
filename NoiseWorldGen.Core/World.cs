namespace NoiseWorldGen.Core;

public class World
{
    public int Seed { get; }
    public int Height { get; }
    public int WaterLevel { get; }
    private readonly Dictionary<int, Column> _columns;
    public NoiseSetting Continentalness { get; }
    public NoiseSetting NoodleCave { get; }
    public NoiseSetting Cave { get; }
    public NoiseSetting Dirt { get; }

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

        Continentalness = NoiseSetting.Create(Seed, 100, GetHeightProportion(.1), GetHeightProportion(.9), (.3f, GetHeightProportion(.35)), (.5f, GetHeightProportion(.45)));
        var noodleCaveOffset = .075f;
        NoodleCave = NoiseSetting.Create(Seed, 100, 0, 0, (-noodleCaveOffset, 0), (0, 1), (noodleCaveOffset, 0));
        var caveOffset = .75f;
        Cave = NoiseSetting.Create(Seed, 100, 1, 1, (-caveOffset - float.Epsilon, 1), (-caveOffset, 0), (caveOffset, 0), (caveOffset + float.Epsilon, 1));
        Dirt = NoiseSetting.Create(Seed, 1, 1, 3);

        int GetHeightProportion(double proportion)
            => (int)(Height * proportion);
    }
}
