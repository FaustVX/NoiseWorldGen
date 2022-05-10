using Microsoft.Xna.Framework;

namespace NoiseWorldGen.OpenGL.Tiles;

public sealed class Tree : Tile, ISingletonTile<Tree>, Tile.IsWalkable
{
    public static Tree Value { get; } = new();

    private Tree()
        : base(Color.Green, default!)
    { }
}
