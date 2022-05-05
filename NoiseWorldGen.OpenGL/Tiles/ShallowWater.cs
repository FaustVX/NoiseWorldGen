namespace NoiseWorldGen.OpenGL.Tiles;

public sealed class ShallowWater : Tile, ISingletonTile<ShallowWater>, Tile.IsWalkable
{
    public static ShallowWater Value { get; } = new();

    private ShallowWater()
    { }
}
