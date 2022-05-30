using System.Runtime.CompilerServices;
using DotnetNoise;
using Microsoft.Xna.Framework;
using NoiseWorldGen.OpenGL.Tiles;
using static DotnetNoise.FastNoise;

namespace NoiseWorldGen.OpenGL.Biomes;


public sealed class Forest : Biome, Biome.IBiome<Forest>
{
    [ModuleInitializer]
    internal static void Init()
    {
        AddBiome<Forest>();
        World.OnWorldCreated += w =>
            Noise = new(w.Seed ^ typeof(RiverWater).GetHashCode())
            {
                UsedNoiseType = NoiseType.WhiteNoise,
            };
    }

    public Forest(World world)
        : base(world)
    { }

    public override SoilTile BaseSoil => Stone.Value;
    public static FastNoise Noise { get; private set; } = default!;

    public static Forest Create(World world)
        => new(world);

    public override FeatureTile? GenerateFeatureTile(Point pos, float localContinentalness, float localTemperature)
    {
        if (IsTreePos(pos, localTemperature))
            return Tree.Value;

        IOre? ore = null;
        GenerateOre(pos, ref ore, qty => new IronOre(qty));
        GenerateOre(pos, ref ore, qty => new CoalOre(qty));
        return ore as FeatureTile;
    }

    private static bool IsTreePos(Point pos, float temperature)
    {
        var threshold = temperature switch
        {
            >= 0/3f and < 1/3f => -.5f,
            >= 1/3f and < 2/3f => 0f,
            >= 2/3f => .5f,
            _ => 1f,
        };
        return Noise.GetNoise(pos.X, pos.Y) > threshold;
    }

    public override string Name => "Forest";

    public static (float min, float max)? Continentalness => (.15f, 1f);

    public static (float min, float max)? Temperature => (0f, .5f);
}
