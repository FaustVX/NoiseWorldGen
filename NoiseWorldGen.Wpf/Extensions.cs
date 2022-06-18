using System.Windows.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NoiseWorldGen.Wpf;

public static class Extensions
{
    public static void PopulateArray<T>(T[,] array, Func<int, int, T> generator)
    {
        var width = array.GetLength(0);
        var height = array.GetLength(1);
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                array[x, y] = generator(x, y);
    }

    public static (int quotient, int remainder) ProperRemainder(int a, int b)
        => Math.DivRem(a, b) switch
        {
            (var div, < 0 and var rem) => (div - 1, rem + b),
            (var div, var rem) => (div, rem),
        };

    public static TValue GetOrCreateValue<TKey, TValue>(Dictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> createValue, out bool exists)
        where TKey : notnull
        => System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault(dictionary, key, out exists) ??= createValue(key);

    public static TValue GetOrCreateValue<TKey, TValue>(Dictionary<TKey, TValue> dictionary, TKey key, out bool exists)
        where TKey : notnull
        where TValue : new()
        => System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault(dictionary, key, out exists) ??= new();

    public static TValue GetOrCreateValue<TKey, TValue>(Dictionary<TKey, TValue> dictionary, TKey key, TValue value, out bool exists)
        where TKey : notnull
        => System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault(dictionary, key, out exists) ??= value;

    public static Point GetRandomPointinSquare(int radius)
        => new(Random.Shared.Next(-radius, radius + 1), Random.Shared.Next(-radius, radius + 1));

    public static Point GetRandomPointinCircle(int radius, bool isfixedDistance = false)
    {
        var angle = Random.Shared.NextSingle() * MathF.PI * 2;
        var distance = isfixedDistance ? (radius + 1) : Random.Shared.NextSingle() * (radius + 1);
        return new((int)(distance * MathF.Cos(angle)), (int)(distance * MathF.Sin(angle)));
    }

    public static void DrawCenteredString(this SpriteBatch sb, SpriteFont font, string text, Point pos, Vector2 center, Color color)
        => sb.DrawString(font, text, pos.ToVector2() - font.MeasureString(text) * center, color);

    public static Rectangle DrawAtWorldPos(this in Rectangle rect, Point from, Point to)
        => new((to - from) * rect.Size + rect.Location, rect.Size);

    public static void DrawLine(this SpriteBatch sb, Point start, Point end, Color color)
        => DrawLine(sb, start.ToVector2(), end.ToVector2(), color);

    public static void DrawLine(this SpriteBatch sb, Vector2 start, Vector2 end, Color color)
    {
        var line = end - start;
        var angleRad = MathF.Atan2(line.Y, line.X);
        var length = line.Length();
        sb.Draw(sb.Pixel(), start, null, color, angleRad, Vector2.Zero, new Vector2(length, 1), default, 0);
    }

    public static void Request(this Tiles.Tile.INetworkReceiver from, ItemStack stack)
        => from.Network.Request(stack);

    public static void Update<TKey, TValue>(this IDictionary<TKey, (TValue previous, TValue current)> dictionary, Func<TKey, TValue> updater)
    {
        foreach (var key in dictionary.Keys)
            dictionary[key] = (dictionary[key].current, updater(key));
    }

    private static readonly Dictionary<int, WeakReference<Texture2D>> _pixels = new();
    public static Texture2D Pixel(this GraphicsDevice gd)
    {
        if (_pixels.TryGetValue(gd.GetHashCode(), out var @ref) && @ref.TryGetTarget(out var pixel))
            return pixel;
        _pixels[gd.GetHashCode()] = new(pixel = new(gd, 1, 1));
        pixel.SetData(new Color[] { Color.White });
        return pixel;
    }

    public static Texture2D Pixel(this SpriteBatch sb)
        => sb.GraphicsDevice.Pixel();
}
