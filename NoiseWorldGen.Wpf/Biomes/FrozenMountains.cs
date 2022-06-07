using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using NoiseWorldGen.Wpf.Tiles;

namespace NoiseWorldGen.Wpf.Biomes;

public sealed class FrozenMountains : Biome, Biome.IBiome<FrozenMountains>
{
    [ModuleInitializer]
    internal static void Init()
    {
        AddBiome<FrozenMountains>();
    }

    public FrozenMountains(World world)
        : base(world)
    { }

    public override SoilTile BaseSoil => FrozenMountain.Value;

    public static FrozenMountains Create(World world)
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

    public override string Name => "Frozen Mountains";

    public static (float min, float max)? Continentalness => (.5f, 1f);

    public static (float min, float max)? Temperature => (-1f, 0);
}
