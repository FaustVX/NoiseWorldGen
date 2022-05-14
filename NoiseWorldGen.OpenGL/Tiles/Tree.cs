using Microsoft.Xna.Framework;

namespace NoiseWorldGen.OpenGL.Tiles;

public sealed class Tree : FeatureTile, ISingletonTile<Tree>, Tile.IsWalkable
{
    public static Tree Value { get; } = new();
    public override string Name => "Tree";

    private Tree()
        : base(Color.Green, default!)
    { }
}
