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
        if (continentalness >= World.WaterHeight)
        {
            var heightness = continentalness >= World.MountainHeight;
            float river = World.RiverNoise.GetNoise(tileX, tileY);
            if (river <= World.RiverClose && river >= -World.RiverClose)
                return Tile.RiverWaterTile;
            return heightness ? Tile.MountainTile : Tile.StoneTile;
        }
        else
        {
            return continentalness > World.ShallowWaterHeight ? Tile.ShallowWaterTile :
                continentalness < World.DeepWaterHeight ? Tile.WaterTile :
                Tile.DeepWaterTile;
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
