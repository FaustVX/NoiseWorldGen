using System.Runtime.CompilerServices;
using DotnetNoise;
using static DotnetNoise.FastNoise;

namespace NoiseWorldGen.OpenGL.Tiles;

public sealed class RiverWater : Tile, ISingletonTile<RiverWater>, Tile.IsWalkable, IInterpolation<RiverWater>
{
    [ModuleInitializer]
    internal static void Init()
    {
        World.OnWorldCreated += w => Noise = new(w.Seed ^ typeof(RiverWater).GetHashCode())
        {
            Frequency = .015f,
            UsedNoiseType = NoiseType.CubicFractal,
        };
    }

    public static float RiverClose { get; } = .01f;
    public static FastNoise Noise { get; private set; } = default!;
    public static Interpolation Interpolation { get; } = new(0, 0)
    {
        { -RiverClose, 0 },
        { MathF.BitIncrement(-RiverClose), 1 },
        { MathF.BitDecrement(RiverClose), 1 },
        { RiverClose, 0 },
    };
    public static RiverWater Value { get; } = new();

    private RiverWater()
    { }
}
