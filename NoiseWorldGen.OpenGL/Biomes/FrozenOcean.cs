using System.Runtime.CompilerServices;
using NoiseWorldGen.OpenGL.Tiles;

namespace NoiseWorldGen.OpenGL.Biomes;

public sealed class FrozenOcean : Biome, Biome.IBiome<FrozenOcean>
{
    [ModuleInitializer]
    internal static void Init()
    {
        AddBiome<FrozenOcean>();
    }

    public FrozenOcean(World world)
        : base(world)
    { }

    public override Tile BaseTile => FrozenWater.Value;

    public static FrozenOcean Create(World world)
        => new(world);

    public static (float min, float max)? Continentalness => Ocean.Continentalness;

    public static (float min, float max)? Temperature => (-1f, 0f);
}
