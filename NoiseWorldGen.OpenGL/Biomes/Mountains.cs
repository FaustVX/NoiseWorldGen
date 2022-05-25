using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
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

    public override FeatureTile? GenerateFeatureTile(Point pos, float localContinentalness, float localTemperature)
    {
        var tile = World.GetSoilTileAt(pos);
        if (tile is Tile.IsOrePlacable)
        {
            IOre? ore = null;
            GenerateOre(pos, ref ore, qty => new IronOre(qty));
            GenerateOre(pos, ref ore, qty => new CoalOre(qty));
            return ore as FeatureTile;
        }
        return null;
    }

    public override string Name => "Mountains";

    public static (float min, float max)? Continentalness => (.5f, 1f);

    public static (float min, float max)? Temperature => (0, 1f);
}
