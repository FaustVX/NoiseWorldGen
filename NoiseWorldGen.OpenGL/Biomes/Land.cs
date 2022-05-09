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
        return BaseTile;
    }

    public static (float min, float max)? Continentalness => (0f, 1f);

    public static (float min, float max)? Temperature => null;
}
