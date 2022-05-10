using Microsoft.Xna.Framework;

namespace NoiseWorldGen.OpenGL.Tiles;

public sealed class Water : Tile, ISingletonTile<Water>
{
    public static Water Value { get; } = new();

    private Water()
        : base(Color.DarkBlue, default!)
    { }
}
