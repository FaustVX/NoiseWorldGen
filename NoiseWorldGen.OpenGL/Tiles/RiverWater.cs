using System.Runtime.CompilerServices;
using DotnetNoise;
using static DotnetNoise.FastNoise;
using Microsoft.Xna.Framework;

namespace NoiseWorldGen.OpenGL.Tiles;

public sealed class RiverWater : SoilTile, ISingletonTile<RiverWater>, Tile.IsWalkable, IInterpolation<RiverWater>
{
    [ModuleInitializer]
    internal static void Init()
        => World.OnWorldCreated += w =>
        {
            Noise = new(w.Seed ^ typeof(RiverWater).GetHashCode())
            {
                Frequency = .015f,
                UsedNoiseType = NoiseType.CubicFractal,
            };
            var texture = new Microsoft.Xna.Framework.Graphics.Texture2D(SpriteBatches.Pixel.GraphicsDevice, 1, 1);
            texture.SetData(new Color[] { Color.DarkBlue });
            TileTemplates._tiles.Add(new TileTemplate.Static(() => Value, texture));
        };

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
    public override string Name => "River Water";

    private RiverWater()
        : base(Color.DarkBlue, default!)
    { }
}
