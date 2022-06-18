using System.ComponentModel;
using System.Runtime.CompilerServices;
using DotnetNoise;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NoiseWorldGen.Wpf.Tiles;

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
    public interface INetwork
    {
        public Networks.Network Network { get; set; }
        public Point Pos { get; }
    }
    public interface INetworkSupplier : INetwork
    {
        public bool CanSupply(Items.Item item);
        public int TrySupply(Items.Item item, int maxQuantity);
    }
    public interface INetworkReceiver : INetwork { }

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

public abstract class TickedFeatureTile : FeatureTile, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        => PropertyChanged?.Invoke(this, new(propertyName));

    protected TickedFeatureTile(World world, Point pos, Color color, Texture2D? texture, Rectangle? textureRect = null)
            : base(color, texture, textureRect)
    {
        World = world;
        Pos = pos;
    }

    public World World { get; }
    public Point Pos { get; }

    private bool _isPaused;
    public bool IsPaused
    {
        get => _isPaused;
        set
        {
            if (value == _isPaused)
                return;
            if (value)
                TickCount = 0;
            _isPaused = value;
            OnPropertyChanged();
        }
    }

    protected int _tickCount;
    public int TickCount
    {
        get => _tickCount;
        set
        {
            if (value == _tickCount || IsPaused)
                return;
            _tickCount = value;
            if (TickCount < 0)
                OnTick();
            OnPropertyChanged();
            OnPropertyChanged(nameof(Color));
        }
    }
    public override Color Color => Color.Lerp(BaseColor * .5f, BaseColor, TickCount / 10f);
    public Color BaseColor => base.Color;

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
