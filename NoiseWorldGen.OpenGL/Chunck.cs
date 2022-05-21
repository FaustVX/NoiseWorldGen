using Microsoft.Xna.Framework;
using NoiseWorldGen.OpenGL.Biomes;
using NoiseWorldGen.OpenGL.Tiles;

namespace NoiseWorldGen.OpenGL;

public class Chunck
{
    public static readonly byte Size = 8;

    public FeatureTile?[,] FeatureTiles { get; }
    public SoilTile[,] SoilTiles { get; }
    public Biome[,] Biomes { get; }
    public bool IsInitialized { get; private set; }

    private bool _isActive;
    public bool IsActive
    {
        get => _isActive;
        set
        {
            if (value == _isActive)
                return;
            _isActive = value;
            if (IsActive)
                World.ActiveChunks.Add(this);
            else
                World.ActiveChunks.Remove(this);
        }
    }

    public Chunck(World world, int chunckX, int chunckY)
    {
        World = world;
        ChunckX = chunckX;
        ChunckY = chunckY;
        Biomes = new Biome[Size, Size];
        SoilTiles = new SoilTile[Size, Size];
        FeatureTiles = new FeatureTile?[Size, Size];
    }

    public void Initialize()
    {
        if (IsInitialized)
            return;
        (float continentalness, float temperature)[,] localNoise = new (float, float)[Size, Size];
        PopulateArray(Biomes, (x, y) =>
        {
            var biome = GenerateBiome(x, y, World, out var cont, out var temp);
            localNoise[x, y] = (cont, temp);
            return biome;
        });
        PopulateArray(SoilTiles, (x, y) => GenerateSoilTile(x, y, localNoise[x, y]));
        PopulateArray(FeatureTiles, (x, y) => GenerateFeatureTile(x, y, localNoise[x, y]));
        IsInitialized = true;
    }

    public void Update()
    {
        foreach (var tile in FeatureTiles)
        {
            if (tile is TickedFeatureTile t and IUpdateable u)
                u.Update(default!);
        }
    }

    private Biome GenerateBiome(int x, int y, World world, out float localContinentalness, out float localTemperature)
    {
        int tileX = ChunckX * Size + x;
        int tileY = ChunckY * Size + y;
        var continentalness = World.Continentalness.GetNoise(tileX, tileY);
        var temperature = World.Temperature.GetNoise(tileX, tileY);
        return Biome.GetBiome(continentalness, temperature, world, out localContinentalness, out localTemperature);
    }

    private SoilTile GenerateSoilTile(int x, int y, (float continentalness, float temperature) localNoise)
    {
        int tileX = ChunckX * Size + x;
        int tileY = ChunckY * Size + y;
        var biome = Biomes[x, y];
        return biome.GenerateSoilTile(tileX, tileY, localNoise.continentalness, localNoise.temperature);
    }

    private FeatureTile? GenerateFeatureTile(int x, int y, (float continentalness, float temperature) localNoise)
    {
        int tileX = ChunckX * Size + x;
        int tileY = ChunckY * Size + y;
        var biome = Biomes[x, y];
        return biome.GenerateFeatureTile(tileX, tileY, localNoise.continentalness, localNoise.temperature);
    }

    public World World { get; }
    public int ChunckX { get; }
    public int ChunckY { get; }
}
