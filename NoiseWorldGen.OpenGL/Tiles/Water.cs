using Microsoft.Xna.Framework;

namespace NoiseWorldGen.OpenGL.Tiles;

public sealed class Water : SoilTile, ISingletonTile<Water>
{
    public static Water Value { get; } = new();
    public override string Name => "Water";

    private Water()
        : base(Color.DarkBlue, default!)
    { }
}
