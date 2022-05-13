using System.Runtime.CompilerServices;
using NoiseWorldGen.OpenGL.Tiles;

namespace NoiseWorldGen.OpenGL.Biomes;

public sealed class Ocean : Biome, Biome.IBiome<Ocean>
{
    [ModuleInitializer]
    internal static void Init()
    {
        AddBiome<Ocean>();
    }

    public Ocean(World world)
        : base(world)
    { }

    public override SoilTile BaseSoil => Water.Value;

    public static Ocean Create(World world)
        => new(world);

    public override FeatureTile? GenerateFeatureTile(int x, int y, float localContinentalness, float localTemperature)
    {
        return null;
    }

    public static (float min, float max)? Continentalness => (-1f, 0f);

    public static (float min, float max)? Temperature => (0f, 1f);
}
