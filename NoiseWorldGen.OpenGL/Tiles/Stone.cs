using Microsoft.Xna.Framework;

namespace NoiseWorldGen.OpenGL.Tiles;

public sealed class Stone : SoilTile, ISingletonTile<Stone>, Tile.IsWalkable, Tile.IsOrePlacable
{
    public static Stone Value { get; } = new();

    private Stone()
        : base(Color.DarkGray, default!)
    { }
}
