using DotnetNoise;
using static DotnetNoise.FastNoise;

namespace NoiseWorldGen.OpenGL;

public class World
{
    public static event Action<World>? OnWorldCreated;
    public FastNoise Continentalness { get; }
    public FastNoise Temperature { get; }

    public float WaterHeight { get; } = 0f;
    public float MountainHeight { get; } = .5f;
    public float ShallowWaterHeight { get; } = -.2f;
    public float DeepWaterHeight { get; } = -.5f;
    public int Seed { get; }

    private readonly Dictionary<(int x, int y), Chunck> Chunks = new();

    public World(int seed)
    {
        Seed = seed;
        Continentalness = new(Seed ^ 1)
        {
            Frequency = .005f,
            UsedNoiseType = NoiseType.SimplexFractal,
        };
        Temperature = new(Seed ^ 2)
        {
            Frequency = .003f,
            UsedNoiseType = NoiseType.Cubic,
        };

        OnWorldCreated?.Invoke(this);
    }

    public Tile GetTileAt(int x, int y)
        => GetChunkAtPos(x, y, out var posX, out var posY).Tiles[posX, posY];

    public void SetTileAt(int x, int y, Tile tile)
        => GetChunkAtPos(x, y, out var posX, out var posY).Tiles[posX, posY] = tile;

    public Biomes.Biome GetBiomeAt(int x, int y)
        => GetChunkAtPos(x, y, out var posX, out var posY).Biomes[posX, posY];

    public void SetBiomeAt(int x, int y, Biomes.Biome biome)
        => GetChunkAtPos(x, y, out var posX, out var posY).Biomes[posX, posY] = biome;

    public Chunck GetChunkAtPos(int x, int y, out int posX, out int posY)
    {
        (var divX, posX) = ProperRemainder(x, Chunck.Size);
        (var divY, posY) = ProperRemainder(y, Chunck.Size);
        return GetOrCreateValue(Chunks, (divX, divY), pos => new(this, divX, divY), out _);
    }
}
