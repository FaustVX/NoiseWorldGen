namespace NoiseWorldGen.OpenGL;

public interface ITile<T>
    where T : Tile, ITile<T>
{
    public static abstract T Value { get; }
}

public abstract class Tile
{
    public interface IsWalkable
    { }
    public static Tile StoneTile => Stone.Value;
    public static Tile MountainTile => Mountain.Value;
    public static Tile RiverWaterTile => RiverWater.Value;
    public static Tile ShallowWaterTile => ShallowWater.Value;
    public static Tile WaterTile => Water.Value;
    public static Tile DeepWaterTile => DeepWater.Value;
}

public sealed class Stone : Tile, ITile<Stone>, Tile.IsWalkable
{
    public static Stone Value { get; } = new();

    private Stone()
    { }
}

public sealed class Mountain : Tile, ITile<Mountain>, Tile.IsWalkable
{
    public static Mountain Value { get; } = new();

    private Mountain()
    { }
}

public sealed class RiverWater : Tile, ITile<RiverWater>, Tile.IsWalkable
{
    public static RiverWater Value { get; } = new();

    private RiverWater()
    { }
}

public sealed class ShallowWater : Tile, ITile<ShallowWater>, Tile.IsWalkable
{
    public static ShallowWater Value { get; } = new();

    private ShallowWater()
    { }
}

public sealed class Water : Tile, ITile<Water>
{
    public static Water Value { get; } = new();

    private Water()
    { }
}

public sealed class DeepWater : Tile, ITile<DeepWater>
{
    public static DeepWater Value { get; } = new();

    private DeepWater()
    { }
}