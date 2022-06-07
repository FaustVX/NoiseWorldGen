using System.Linq;
using System.Collections.Concurrent;
using Microsoft.Xna.Framework;

namespace NoiseWorldGen.Wpf.Networks;

public class Network
{
    public static HashSet<Network> Networks { get; } = new();
    public static Network GetOrCreateNetworkAt(World world, Point position)
    {
        foreach (var network in Networks)
        {
            var rangeSquared = network.MaxRange * network.MaxRange;
            foreach (var pos in network._tiles)
                if ((pos.Pos - position).ToVector2().LengthSquared() <= rangeSquared)
                    return network;
        }
        return new(world, null);
    }
    private readonly HashSet<Tiles.Tile.INetwork> _tiles = new();
    private readonly HashSet<Tiles.Tile.INetworkReceiver> _receivers = new();
    private readonly HashSet<Tiles.Tile.INetworkSupplier> _suppliers = new();
    public IReadOnlySet<Tiles.Tile.INetwork> Tiles => _tiles;
    public int MaxRange { get; } = 8;
    public World World { get; }
    private readonly ConcurrentBag<ItemStack> _request = new();

    private readonly Dictionary<Tiles.Tile.INetwork, List<Tiles.Tile.INetwork>> _connections = new();
    public IEnumerable<Tiles.Tile.INetwork> GetConnection(Tiles.Tile.INetwork Tile)
        => _connections[Tile];

    public Network(World world, string? name = null)
    {
        Networks.Add(this);
        World = world;
    }

    public void AddTile(Tiles.Tile.INetwork tile)
    {
        var rangeSquared = MaxRange * MaxRange;
        if (_tiles.Count == 0)
        {
            AddTileImpl(tile);
        }
        else
            foreach (var tile0 in _tiles)
                if ((tile0.Pos - tile.Pos).ToVector2().LengthSquared() <= rangeSquared)
                {
                    var connection = AddTileImpl(tile);
                    foreach (var tile1 in _tiles)
                        if ((tile1.Pos - tile.Pos).ToVector2().LengthSquared() <= rangeSquared)
                        {
                            connection.Add(tile1);
                            _connections[tile1].Add(tile);
                        }
                    break;
                }
    }

    private IList<Tiles.Tile.INetwork> AddTileImpl(Tiles.Tile.INetwork tile)
    {
        _tiles.Add(tile);
        if (tile is Tiles.Tile.INetworkReceiver r)
            _receivers.Add(r);
        if (tile is Tiles.Tile.INetworkSupplier s)
            _suppliers.Add(s);
        return _connections[tile] = new();
    }

    public void Merge(Network other)
    {
        if (other.World != World)
            return;
        else
        {
            Networks.Remove(other);
            foreach (var tile in other._tiles)
            {
                AddTileImpl(tile);
                tile.Network = this;
            }
        }
    }

    public void Update()
    {
        foreach (var stack in _request)
            if (!stack.IsFull && Request(stack.item)?.TrySupply(stack.item, 1) is int q and > 0)
                stack.Quantity += q;
        _request.Clear();
    }

    private Tiles.Tile.INetworkSupplier? Request(Items.Item item)
        => _suppliers.FirstOrDefault(t => t.CanSupply(item));

    public void Request(ItemStack stack)
    {
        if (!stack.IsFull)
            _request.Add(stack);
    }
}