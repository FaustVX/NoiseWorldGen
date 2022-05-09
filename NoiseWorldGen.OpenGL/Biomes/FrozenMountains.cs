using System.Runtime.CompilerServices;
using NoiseWorldGen.OpenGL.Tiles;

namespace NoiseWorldGen.OpenGL.Biomes;

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

    public override Tile BaseTile => FrozenMountain.Value;

    public static FrozenMountains Create(World world)
        => new(world);

    public override Tile GenerateTile(int x, int y, float localContinentalness, float localTemperature)
    {
        return BaseTile;
    }

    public static (float min, float max)? Continentalness => (.5f, 1f);

    public static (float min, float max)? Temperature => (-1f, 0);
}
