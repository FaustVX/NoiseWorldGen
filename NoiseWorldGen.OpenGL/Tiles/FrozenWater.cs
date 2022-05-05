namespace NoiseWorldGen.OpenGL.Tiles;

public sealed class FrozenWater : Tile, ISingletonTile<FrozenWater>
{
    public static FrozenWater Value { get; } = new();

    private FrozenWater()
    { }
}
