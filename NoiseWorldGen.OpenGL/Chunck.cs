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
        (float continentalness, float temperature)[,] localNoise = new (float, float)[Size, Size];
        Biomes = PopulateArray(Size, Size, (x, y) =>
        {
            var biome = GenerateBiome(x, y, world, out var cont, out var temp);
            localNoise[x, y] = (cont, temp);
            return biome;
        });
        Tiles = PopulateArray(Size, Size, (x, y) => GenerateTile(x, y, localNoise[x, y]));
    }

    private Biome GenerateBiome(int x, int y, World world, out float localContinentalness, out float localTemperature)
    {
        int tileX = ChunckX * Size + x;
        int tileY = ChunckY * Size + y;
        var continentalness = World.Continentalness.GetNoise(tileX, tileY);
        var temperature = World.Temperature.GetNoise(tileX, tileY);
        return Biome.GetBiome(continentalness, temperature, world, out localContinentalness, out localTemperature);
    }

    private Tile GenerateTile(int x, int y, (float continentalness, float temperature) localNoise)
    {
        int tileX = ChunckX * Size + x;
        int tileY = ChunckY * Size + y;
        var biome = Biomes[x, y];
        return biome.GenerateTile(tileX, tileY, localNoise.continentalness, localNoise.temperature);
    }

    public World World { get; }
    public int ChunckX { get; }
    public int ChunckY { get; }
}
