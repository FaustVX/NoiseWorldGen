namespace NoiseWorldGen.OpenGL;

public sealed class Stone : Tile, ISingletonTile<Stone>, Tile.IsWalkable, Tile.IsOrePlacable
{
    public static Stone Value { get; } = new();

    private Stone()
    { }
}

public sealed class Mountain : Tile, ISingletonTile<Mountain>, Tile.IsWalkable, Tile.IsOrePlacable
{
    public static Mountain Value { get; } = new();

    private Mountain()
    { }
}
