using System.Linq;
using System.Collections.Concurrent;
using Microsoft.Xna.Framework;

namespace NoiseWorldGen.OpenGL.Networks;

public class Network
{
    public static HashSet<Network> Networks { get; } = new();
    public static Dictionary<string, Network> NamedNetworks { get; } = new();
    public static Network GetOrCreateNetworkAt(World world, Point position)
    {
        foreach (var network in Networks)
        {
            var rangeSquared = network.MaxRange * network.MaxRange;
            foreach (var pos in network._positions)
                if ((pos - position).ToVector2().LengthSquared() <= rangeSquared)
                    return network;
        }
        return new(world, null);
    }
    private readonly HashSet<Point> _positions = new();
    public IReadOnlySet<Point> Positions => _positions;
    public int MaxRange { get; } = 8;
    public World World { get; }
    public string? Name { get; set; }
    private readonly ConcurrentBag<ItemStack> _request = new();

    private readonly Dictionary<Point, List<Point>> _connections = new();
    public IEnumerable<Tiles.Tile.INetwork> GetConnection(Tiles.Tile.INetwork Tile)
        => _connections[Tile.Pos].Select(World.GetFeatureTileAt).OfType<Tiles.Tile.INetwork>();

    public Network(World world, string? name = null)
    {
        if (name is not null)
            NamedNetworks.Add(name, this);
        Networks.Add(this);
        World = world;
        Name = name;
    }

    public void AddTile(Point position)
    {
        if (World.GetFeatureTileAt(position) is not Tiles.Tile.INetwork)
            return;
        var rangeSquared = MaxRange * MaxRange;
        if (_positions.Count == 0)
        {
            _positions.Add(position);
            _connections[position] = new();
        }
        else
            foreach (var pos in _positions)
                if ((pos - position).ToVector2().LengthSquared() <= rangeSquared)
                {
                    _positions.Add(position);
                    var connection = _connections[position] = new();
                    foreach (var pos0 in _positions)
                        if ((pos0 - position).ToVector2().LengthSquared() <= rangeSquared)
                        {
                            connection.Add(pos0);
                            _connections[pos0].Add(position);
                        }
                    break;
                }
    }

    public bool ContainsFeature<T>(Func<T, bool>? predicate = null)
        where T : Tiles.FeatureTile, Tiles.Tile.INetwork
        => _positions.Select(World.GetFeatureTileAt)
            .OfType<T>()
            .Any(predicate ?? (static _ => true));

    public void Merge(Network other)
    {
        if (other.World != World)
            return;
        if ((Name, other.Name) is (null, not null))
            other.Merge(this);
        else
        {
            Remove(other);
            foreach (var pos in other._positions)
            {
                _positions.Add(pos);
                if (World.GetFeatureTileAt(pos) is Tiles.Tile.INetwork tile)
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
        => _positions.Select(World.GetFeatureTileAt)
            .OfType<Tiles.Tile.INetworkSupplier>()
            .FirstOrDefault(t => t.CanSupply(item));

    public void Request(ItemStack stack)
    {
        if (!stack.IsFull)
            _request.Add(stack);
    }

    public static void Remove(Network network)
    {
        Networks.Remove(network);
        if (network.Name is not null)
            NamedNetworks.Remove(network.Name);
    }
}