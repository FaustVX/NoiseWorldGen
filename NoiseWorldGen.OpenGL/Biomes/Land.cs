using System.Runtime.CompilerServices;
using NoiseWorldGen.OpenGL.Tiles;

namespace NoiseWorldGen.OpenGL.Biomes;

public sealed class Land : Biome, Biome.IBiome<Land>
{
    [ModuleInitializer]
    internal static void Init()
    {
        AddBiome<Land>();
    }

    public Land(World world)
        : base(world)
    { }

    public override SoilTile BaseSoil => Stone.Value;

    public static Land Create(World world)
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

    public override string Name => "Land";

    public static (float min, float max)? Continentalness => (0f, 1f);

    public static (float min, float max)? Temperature => null;
}
