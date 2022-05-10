using Microsoft.Xna.Framework;

namespace NoiseWorldGen.OpenGL.Tiles;

public sealed class FrozenWater : Tile, ISingletonTile<FrozenWater>, Tile.IsWalkable
{
    public static FrozenWater Value { get; } = new();

    private FrozenWater()
        : base(Color.CornflowerBlue, default!)
    { }
}
