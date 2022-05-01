namespace NoiseWorldGen.OpenGL;

public class Chunck
{
    public static readonly byte Size = 8;

    private readonly Tile[,] Tiles = GenerateArray(Size, Size, Stone.Value as Tile);

    public Tile this[int x, int y]
    {
        get => Tiles[x, y];
        set => Tiles[x, y] = value ?? Stone.Value;
    }
}
