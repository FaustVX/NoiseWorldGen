using Microsoft.Xna.Framework;

namespace NoiseWorldGen.OpenGL.Tiles;

public sealed class FrozenMountain : SoilTile, ISingletonTile<FrozenMountain>, Tile.IsWalkable, Tile.IsOrePlacable
{
    public static FrozenMountain Value { get; } = new();

    private FrozenMountain()
        : base(Color.WhiteSmoke, default!)
    { }
}
