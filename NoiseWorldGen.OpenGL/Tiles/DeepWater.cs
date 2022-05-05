namespace NoiseWorldGen.OpenGL.Tiles;

public sealed class DeepWater : Tile, ISingletonTile<DeepWater>
{
    public static DeepWater Value { get; } = new();

    private DeepWater()
    { }
}
