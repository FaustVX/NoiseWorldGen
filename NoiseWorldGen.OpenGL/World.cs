using DotnetNoise;
using static DotnetNoise.FastNoise;
using NoiseWorldGen.OpenGL.Tiles;
using Microsoft.Xna.Framework;

namespace NoiseWorldGen.OpenGL;

public class World : IGameComponent, IUpdateable
{
    public static event Action<World>? OnWorldCreated;
    public FastNoise Continentalness { get; }
    public FastNoise Temperature { get; }

    public float WaterHeight { get; } = 0f;
    public float MountainHeight { get; } = .5f;
    public float ShallowWaterHeight { get; } = -.2f;
    public float DeepWaterHeight { get; } = -.5f;
    public int Seed { get; }
    public Player Player { get; }

    bool IUpdateable.Enabled => true;

    int IUpdateable.UpdateOrder => 0;

    public List<Chunck> ActiveChunks { get; }

    private readonly Dictionary<(int x, int y), Chunck> Chunks = new();
    private TimeSpan _lastUpdate = new();

    public World(int seed)
    {
        Seed = seed;
        Continentalness = new(Seed ^ 1)
        {
            Frequency = .005f,
            Octaves = 4,
            UsedNoiseType = NoiseType.SimplexFractal,
        };
        Temperature = new(Seed ^ 2)
        {
            Frequency = .003f,
            UsedNoiseType = NoiseType.Cubic,
        };

        Player = new(this);
        ActiveChunks = new();

        OnWorldCreated?.Invoke(this);
    }

    public event EventHandler<EventArgs>? EnabledChanged;

    public event EventHandler<EventArgs>? UpdateOrderChanged;

    public FeatureTile? GetFeatureTileAt(int x, int y)
        => GetChunkAtPos(x, y, out var posX, out var posY).FeatureTiles[posX, posY];

    public FeatureTile? GetFeatureTileAtPlayer()
        => GetFeatureTileAt((int)Player.Position.X, (int)Player.Position.Y);

    public void SetFeatureTileAtPlayer(FeatureTile? tile)
        => SetFeatureTileAt((int)Player.Position.X, (int)Player.Position.Y, tile);

    public void SetFeatureTileAt(int x, int y, FeatureTile? tile)
        => GetChunkAtPos(x, y, out var posX, out var posY).FeatureTiles[posX, posY] = tile;

    public SoilTile GetSoilTileAt(int x, int y)
        => GetChunkAtPos(x, y, out var posX, out var posY).SoilTiles[posX, posY];

    public SoilTile GetSoilTileAtPlayer()
        => GetSoilTileAt((int)Player.Position.X, (int)Player.Position.Y);

    public void SetSoilTileAtPlayer(SoilTile tile)
        => SetSoilTileAt((int)Player.Position.X, (int)Player.Position.Y, tile);

    public void SetSoilTileAt(int x, int y, SoilTile tile)
        => GetChunkAtPos(x, y, out var posX, out var posY).SoilTiles[posX, posY] = tile;

    public Biomes.Biome GetBiomeAt(int x, int y)
        => GetChunkAtPos(x, y, out var posX, out var posY).Biomes[posX, posY];

    public Biomes.Biome GetBiomeAtPlayer()
        => GetBiomeAt((int)Player.Position.X, (int)Player.Position.Y);

    public void SetBiomeAt(int x, int y, Biomes.Biome biome)
        => GetChunkAtPos(x, y, out var posX, out var posY).Biomes[posX, posY] = biome;

    public Chunck GetChunkAtPosAtPlayer(out int posX, out int posY)
        => GetChunkAtPos((int)Player.Position.X, (int)Player.Position.Y, out posX, out posY);

    public Chunck GetChunkAtPos(int x, int y, out int posX, out int posY)
    {
        (var divX, posX) = ProperRemainder(x, Chunck.Size);
        (var divY, posY) = ProperRemainder(y, Chunck.Size);
        return GetOrCreateValue(Chunks, (divX, divY), pos => new(this, divX, divY), out _);
    }

    void IUpdateable.Update(GameTime gameTime)
    {
        if (gameTime.TotalGameTime - _lastUpdate < TimeSpan.FromSeconds(1 / 20d))
            return;

        foreach (var chunk in ActiveChunks)
        {
            chunk.Update();
        }
        _lastUpdate = gameTime.TotalGameTime;
    }

    void IGameComponent.Initialize()
    { }
}
