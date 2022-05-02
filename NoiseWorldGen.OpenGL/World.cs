using DotnetNoise;
using static DotnetNoise.FastNoise;

namespace NoiseWorldGen.OpenGL;

public class World
{
    public FastNoise Continentalness { get; }
    private readonly Dictionary<(int x, int y), Chunck> Chunks = new();

    public World(int seed)
    {
        Continentalness = new(seed ^ 1)
        {
            Frequency = .005f,
            // Octaves = 0,
            // Lacunarity = 2,
            UsedNoiseType = NoiseType.SimplexFractal,
        };
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
