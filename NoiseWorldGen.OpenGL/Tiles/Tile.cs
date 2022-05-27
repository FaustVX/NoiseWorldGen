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
    public interface IsFeaturePlacable { }
    public interface IsOrePlacable : IsFeaturePlacable { }

    public virtual Color Color { get; }
    public virtual Texture2D Texture { get; }
    public virtual Rectangle TextureRect { get; }
    public abstract string Name { get; }

    public virtual void Mine(World world, Point pos, Tile tile)
    { }
    public virtual void Mine(World world, Player player)
    { }

    public static float GetNoise<T>(float x, float y)
        where T : INoise<T>
        => T.Noise.GetNoise(x, y);

    public static int GetInterpolatedNoise<T>(float x, float y)
        where T : IInterpolation<T>
        => IInterpolation<T>.GetValue(x, y);

    public virtual void Draw(Rectangle tileRect, World world, Point pos)
    {
        if (Texture is {} text)
            SpriteBatches.Game.Draw(text, tileRect, TextureRect, Color.White);
        else
            SpriteBatches.Game.Draw(SpriteBatches.Pixel, tileRect, Color);
    }
    public virtual void Update(World world, Point pos)
    { }
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
    public override void Mine(World world, Point pos, Tile tile)
        => world.SetFeatureTileAt(pos, null);
    public override void Mine(World world, Player player)
        => world.SetFeatureTileAt(player.Position.ToPoint(), null);
}

public abstract class TickedFeatureTile : FeatureTile
{

    protected TickedFeatureTile(World world, Point pos, Color color, Texture2D? texture, Rectangle? textureRect = null)
            : base(color, texture, textureRect)
    {
        World = world;
        Pos = pos;
    }

    public World World { get; }
    public Point Pos { get; }

    private int tickCount;
    public int TickCount
    {
        get => tickCount;
        set
        {
            tickCount = value;
            if (TickCount < 0)
                OnTick();
        }
    }
    public override Color Color => Color.Lerp(base.Color * .5f, base.Color, TickCount / 10f);

    protected abstract void OnTick();

    public override void Update(World world, Point pos)
    {
        TickCount--;
    }
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
