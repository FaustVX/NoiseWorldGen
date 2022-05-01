namespace NoiseWorldGen.OpenGL;

public interface ITile<T>
    where T : Tile, ITile<T>
{
    public static abstract T Value { get; }
}

public abstract class Tile
{
    public static Tile StoneTile => Stone.Value;
}

public sealed class Stone : Tile, ITile<Stone>
{
    public static Stone Value { get; } = new();

    private Stone()
    { }
}