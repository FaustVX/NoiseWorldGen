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

    public override SoilTile BaseSoil => Mountain.Value;

    public static Mountains Create(World world)
        => new(world);

    public override FeatureTile? GenerateFeatureTile(int x, int y, float localContinentalness, float localTemperature)
    {
        var tile = World.GetSoilTileAt(x, y);
        if (tile is Tile.IsOrePlacable)
        {
            IOre? ore = null;
            GenerateOre(x, y, ref ore, qty => new IronOre(qty));
            GenerateOre(x, y, ref ore, qty => new CoalOre(qty));
            return ore as FeatureTile;
        }
        return null;
    }

    public static (float min, float max)? Continentalness => (.5f, 1f);

    public static (float min, float max)? Temperature => (0, 1f);
}
