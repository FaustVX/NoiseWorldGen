using System.Runtime.CompilerServices;
using DotnetNoise;
using static DotnetNoise.FastNoise;
using Microsoft.Xna.Framework;

namespace NoiseWorldGen.OpenGL.Tiles;

public sealed class IronOre : Tile, IOre, IInterpolation<IronOre>, Tile.IsWalkable
{
    [ModuleInitializer]
    internal static void Init()
    {
        World.OnWorldCreated += w => Noise = new(w.Seed ^ typeof(IronOre).GetHashCode())
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


    public IronOre(uint quantity)
        : base(Color.Red, default!)
    {
        Quantity = quantity;
    }
}
