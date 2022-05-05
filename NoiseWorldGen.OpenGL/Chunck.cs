using NoiseWorldGen.OpenGL.Biomes;
using NoiseWorldGen.OpenGL.Tiles;

namespace NoiseWorldGen.OpenGL;

public class Chunck
{
    public static readonly byte Size = 8;

    public Tile[,] Tiles { get; }
    public Biome[,] Biomes { get; }

    public Chunck(World world, int chunckX, int chunckY)
    {
        World = world;
        ChunckX = chunckX;
        ChunckY = chunckY;
        Biomes = PopulateArray(Size, Size, (x, y) => GenerateBiome(x, y, world));
        Tiles = PopulateArray(Size, Size, GenerateTile);
    }

    private Biome GenerateBiome(int x, int y, World world)
    {
        int tileX = ChunckX * Size + x;
        int tileY = ChunckY * Size + y;
        var continentalness = World.Continentalness.GetNoise(tileX, tileY);
        var temperature = World.Temperature.GetNoise(tileX, tileY);
        return Biome.GetBiome(continentalness, temperature, world);
    }

    private Tile GenerateTile(int x, int y)
    {
        int tileX = ChunckX * Size + x;
        int tileY = ChunckY * Size + y;
        var biome = Biomes[x, y];
        return biome.GenerateTile(tileX, tileY);
    }

    public World World { get; }
    public int ChunckX { get; }
    public int ChunckY { get; }
}
