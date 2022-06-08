using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using NoiseWorldGen.Wpf.Tiles;

namespace NoiseWorldGen.Wpf.Biomes;

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

    public override SoilTile BaseSoil => FrozenWater.Value;

    public static FrozenOcean Create(World world)
        => new(world);

    public override FeatureTile? GenerateFeatureTile(Point pos, float localContinentalness, float localTemperature)
    {
        return null;
    }

    public override string Name => "Frozen Ocean";

    public static (float min, float max)? Continentalness => Ocean.Continentalness;

    public static (float min, float max)? Temperature => (-1f, 0f);
}