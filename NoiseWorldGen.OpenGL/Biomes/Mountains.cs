using System.Runtime.CompilerServices;
using NoiseWorldGen.OpenGL.Tiles;

namespace NoiseWorldGen.OpenGL.Biomes;

public sealed class Mountains : Biome, Biome.IBiome<Mountains>
{
    [ModuleInitializer]
    internal static void Init()
    {
        AddBiome<Mountains>();
    }

    public Mountains(World world)
        : base(world)
    { }

    public override Tile BaseTile => Mountain.Value;

    public static Mountains Create(World world)
        => new(world);

    public override Tile GenerateTile(int x, int y, float localContinentalness, float localTemperature)
    {
        var tile = BaseTile;
        if (tile is Tile.IsOrePlacable)
        {
            IOre? ore = default!;
            GenerateOre(x, y, ref ore, qty => new IronOre(qty));
            GenerateOre(x, y, ref ore, qty => new CoalOre(qty));
            tile = ore as Tile ?? tile;
        }
        return tile;
    }

    public static (float min, float max)? Continentalness => (.5f, 1f);

    public static (float min, float max)? Temperature => (0, 1f);
}
