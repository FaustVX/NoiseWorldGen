using DotnetNoise;
using static DotnetNoise.FastNoise;
using NoiseWorldGen.OpenGL.Tiles;
using Microsoft.Xna.Framework;
using System.Linq;

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
    public ulong GameTime { get; private set; }

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

    public FeatureTile? GetFeatureTileAt(Point pos)
        => GetChunkAtPos(pos, out var posX, out var posY).FeatureTiles[posX, posY];

    public FeatureTile? GetFeatureTileAtPlayer()
        => GetFeatureTileAt(Player.Position.ToPoint());

    public void SetFeatureTileAtPlayer(FeatureTile? tile)
        => SetFeatureTileAt(Player.Position.ToPoint(), tile);

    public void SetFeatureTileAt(Point pos, FeatureTile? tile)
        => GetChunkAtPos(pos, out var posX, out var posY).FeatureTiles[posX, posY] = tile;

    public SoilTile GetSoilTileAt(Point pos)
        => GetChunkAtPos(pos, out var posX, out var posY).SoilTiles[posX, posY];

    public SoilTile GetSoilTileAtPlayer()
        => GetSoilTileAt(Player.Position.ToPoint());

    public void SetSoilTileAtPlayer(SoilTile tile)
        => SetSoilTileAt(Player.Position.ToPoint(), tile);

    public void SetSoilTileAt(Point pos, SoilTile tile)
        => GetChunkAtPos(pos, out var posX, out var posY).SoilTiles[posX, posY] = tile;

    public Biomes.Biome GetBiomeAt(Point pos)
        => GetChunkAtPos(pos, out var posX, out var posY).Biomes[posX, posY];

    public Biomes.Biome GetBiomeAtPlayer()
        => GetBiomeAt(Player.Position.ToPoint());

    public void SetBiomeAt(Point pos, Biomes.Biome biome)
        => GetChunkAtPos(pos, out var posX, out var posY).Biomes[posX, posY] = biome;

    public Networks.Network? GetNetworkAt(Point pos)
        => Networks.Network.Networks.FirstOrDefault(n => n.World == this && n.Positions.Contains(pos));

    public void PlaceTile(Point pos, Tile tile)
    {
        switch (tile)
        {
            case Tiles.SoilTile st when st is Tiles.Tile.IsFeaturePlacable || GetFeatureTileAt(pos) is null:
                SetSoilTileAt(pos, st);
                break;
            case Tiles.FeatureTile ft when GetSoilTileAt(pos) is Tiles.Tile.IsFeaturePlacable && GetFeatureTileAt(pos) is null:
                SetFeatureTileAt(pos, ft);
                if (ft is Tile.INetwork t)
                {
                    var network = Networks.Network.GetOrCreateNetworkAt(this, pos);
                    network.AddTile(pos);
                    t.Network = network;
                }
                break;
        }
    }

    public Chunck GetChunkAtPosAtPlayer(out int posX, out int posY)
        => GetChunkAtPos(Player.Position.ToPoint(), out posX, out posY);

    public Chunck GetChunkAtPos(Point pos, out int posX, out int posY)
    {
        (var divX, posX) = ProperRemainder(pos.X, Chunck.Size);
        (var divY, posY) = ProperRemainder(pos.Y, Chunck.Size);
        return GetOrCreateValue(Chunks, (divX, divY), pos => new(this, divX, divY), out _);
    }

    void IUpdateable.Update(GameTime gameTime)
    {
        if (gameTime.TotalGameTime - _lastUpdate < TimeSpan.FromSeconds(1 / 20d))
            return;

        foreach (var network in Networks.Network.Networks)
        {
            network.Update();
        }

        foreach (var chunk in ActiveChunks)
        {
            chunk.Update();
        }
        _lastUpdate = gameTime.TotalGameTime;
        GameTime++;
    }

    void IGameComponent.Initialize()
    { }
}
