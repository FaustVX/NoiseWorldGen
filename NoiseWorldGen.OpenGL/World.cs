using DotnetNoise;
using static DotnetNoise.FastNoise;

namespace NoiseWorldGen.OpenGL;

public class World
{
    public static event Action<World>? OnWorldCreated;
    public FastNoise Continentalness { get; }

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

        OnWorldCreated?.Invoke(this);
    }

    public Tile this[int x, int y]
    {
        get => GetChunkAtPos(x, y, out var posX, out var posY)[posX, posY];
        set => GetChunkAtPos(x, y, out var posX, out var posY)[posX, posY] = value;
    }

    public Chunck GetChunkAtPos(int x, int y, out int posX, out int posY)
    {
        (var divX, posX) = ProperRemainder(x, Chunck.Size);
        (var divY, posY) = ProperRemainder(y, Chunck.Size);
        return GetOrCreateValue(Chunks, (divX, divY), pos => new(this, divX, divY), out _);
    }
}
