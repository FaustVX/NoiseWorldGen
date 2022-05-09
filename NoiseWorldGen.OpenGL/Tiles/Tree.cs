namespace NoiseWorldGen.OpenGL.Tiles;

public sealed class Tree : Tile, ISingletonTile<Tree>
{
    public static Tree Value { get; } = new();

    private Tree()
    { }
}
