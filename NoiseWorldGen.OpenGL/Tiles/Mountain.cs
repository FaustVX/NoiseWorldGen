using Microsoft.Xna.Framework;

namespace NoiseWorldGen.OpenGL.Tiles;

public sealed class Mountain : SoilTile, ISingletonTile<Mountain>, Tile.IsWalkable, Tile.IsOrePlacable
{
    public static Mountain Value { get; } = new();

    private Mountain()
        : base(Color.DimGray, default!)
    { }
}
