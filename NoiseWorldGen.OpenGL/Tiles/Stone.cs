namespace NoiseWorldGen.OpenGL.Tiles;

public sealed class Stone : Tile, ISingletonTile<Stone>, Tile.IsWalkable, Tile.IsOrePlacable
{
    public static Stone Value { get; } = new();

    private Stone()
    { }
}
