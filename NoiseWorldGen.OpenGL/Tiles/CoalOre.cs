using System.Runtime.CompilerServices;
using DotnetNoise;
using static DotnetNoise.FastNoise;

namespace NoiseWorldGen.OpenGL.Tiles;

public sealed class CoalOre : Tile, IOre, IInterpolation<CoalOre>, Tile.IsWalkable
{
    [ModuleInitializer]
    internal static void Init()
    {
        World.OnWorldCreated += w => Noise = new(w.Seed ^ typeof(CoalOre).GetHashCode())
        {
            Frequency = .02f,
            UsedNoiseType = NoiseType.CubicFractal,
        };
    }

    public static uint MaxQuantity { get; } = 1000;
    public static FastNoise Noise { get; private set; } = default!;
    public static Interpolation Interpolation { get; } = new(0, (int)MaxQuantity)
    {
        { .3f, 0 },
    };
    public uint Quantity { get; set; }


    public CoalOre(uint quantity)
    {
        Quantity = quantity;
    }
}