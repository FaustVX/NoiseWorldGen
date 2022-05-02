namespace NoiseWorldGen.OpenGL;

public class Chunck
{
    public static readonly byte Size = 8;

    private readonly Tile[,] Tiles;

    public Chunck(World world, int chunckX, int chunckY)
    {
        World = world;
        ChunckX = chunckX;
        ChunckY = chunckY;
        Tiles = PopulateArray(Size, Size, GenerateTile);
    }

    private Tile GenerateTile(int x, int y)
    {
        int tileX = ChunckX * Size + x;
        int tileY = ChunckY * Size + y;
        var continentalness = World.Continentalness.GetNoise(tileX, tileY);
        if (continentalness >= .15f)
        {
            var heightness = continentalness >= .5f;
            return heightness ? Tile.MountainTile : Tile.StoneTile;
        }
        else
        {
            return continentalness > -.05f ? Tile.ShallowWaterTile : continentalness < -.5f ? Tile.WaterTile : Tile.DeepWaterTile;
        }
    }

    public Tile this[int x, int y]
    {
        get => Tiles[x, y];
        set => Tiles[x, y] = value ?? Stone.Value;
    }

    public World World { get; }
    public int ChunckX { get; }
    public int ChunckY { get; }
}
