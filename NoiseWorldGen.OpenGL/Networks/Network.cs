using System.Linq;
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
            _positions.Add(position);
        else
            foreach (var pos in _positions)
                if ((pos - position).ToVector2().LengthSquared() <= rangeSquared)
                {
                    _positions.Add(position);
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

    public Tiles.Tile.INetworkSupplier? Request(TileTemplate tile)
        => _positions.Select(World.GetFeatureTileAt)
            .OfType<Tiles.Tile.INetworkSupplier>()
            .FirstOrDefault(t => t.CanSupply(tile));

    public static void Remove(Network network)
    {
        Networks.Remove(network);
        if (network.Name is not null)
            NamedNetworks.Remove(network.Name);
    }
}