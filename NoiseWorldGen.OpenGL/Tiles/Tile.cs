using DotnetNoise;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NoiseWorldGen.OpenGL.Tiles;

public interface ISingletonTile<T>
    where T : Tile, ISingletonTile<T>
{
    public static abstract T Value { get; }
}

public abstract class Tile
{
    protected Tile(Color color, Texture2D? texture, Rectangle? textureRect = default)
    {
        Color = color;
        Texture = texture!;
        TextureRect = textureRect ?? texture?.Bounds ?? default;
    }

    public interface IsWalkable { }
    public interface IsOrePlacable { }

    public virtual Color Color { get; }
    public virtual Texture2D Texture { get; }
    public virtual Rectangle TextureRect { get; }

    public static float GetNoise<T>(float x, float y)
        where T : INoise<T>
        => T.Noise.GetNoise(x, y);

    public static int GetInterpolatedNoise<T>(float x, float y)
        where T : IInterpolation<T>
        => IInterpolation<T>.GetValue(x, y);
}

public abstract class SoilTile : Tile
{
    protected SoilTile(Color color, Texture2D? texture, Rectangle? textureRect = null)
        : base(color, texture, textureRect)
    { }
}

public abstract class FeatureTile : Tile
{
    protected FeatureTile(Color color, Texture2D? texture, Rectangle? textureRect = null)
        : base(color, texture, textureRect)
    { }
}

public interface INoise<T>
{
    public static abstract FastNoise Noise { get; }
}

public interface IInterpolation<T> : INoise<T>
    where T : IInterpolation<T>
{
    public static abstract Interpolation Interpolation { get; }

    public static int GetValue(float x, float y)
        => T.Interpolation.Lerp(Tile.GetNoise<T>(x, y));
}
