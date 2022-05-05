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

    public static (float min, float max)? Continentalness => (.5f, 1f);

    public static (float min, float max)? Temperature => null;
}
