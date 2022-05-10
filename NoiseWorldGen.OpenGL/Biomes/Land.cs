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

    public override Tile BaseTile => Stone.Value;

    public static Land Create(World world)
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

            static void GenerateOre<T>(float tileX, float tileY, ref IOre? ore, Func<uint, T> ctor)
                where T : IInterpolation<T>, IOre
                => ore = Tile.GetInterpolatedNoise<T>(tileX, tileY) is > 0 and var qty && (ore?.Quantity ?? 0) < qty
                    ? ctor((uint)qty)
                    : ore;
        }
        return tile;
    }

    public static (float min, float max)? Continentalness => (0f, 1f);

    public static (float min, float max)? Temperature => null;
}
