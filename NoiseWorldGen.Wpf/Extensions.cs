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
        sb.Draw(SpriteBatches.Pixel, start, null, color, angleRad, Vector2.Zero, new Vector2(length, 1), default, 0);
    }

    public static void Request(this Tiles.Tile.INetworkReceiver from, ItemStack stack)
        => from.Network.Request(stack);

    public static bool IsKeyClicked(this Key key)
        => Keyboard.IsKeyDown(key) && Keyboard.IsKeyToggled(key);

    public static bool IsKeyClicked(this (Key, Key) keys)
        => IsKeyClicked(keys.Item1) || IsKeyClicked(keys.Item2);

    public static bool IsKeyDown(this Key key)
        => Keyboard.IsKeyDown(key);

    public static bool IsKeyDown(this (Key, Key) keys)
        => IsKeyDown(keys.Item1) || IsKeyDown(keys.Item2);

    public static int IsKeyXor(this (Key, Key) keys, Func<Key, bool> func)
        => (func(keys.Item1), func(keys.Item2)) switch
        {
            (true, false) => -1,
            (false, true) => +1,
            _ => 0,
        };

    public class SRLatch
    {
        private readonly Func<int> _quantity;
        public int Min { get; }
        public int Max { get; }
        private bool _isValid;
        public bool IsValid
        {
            get
            {
                if (_quantity() < Min)
                    return _isValid = true;
                if (_quantity() > Max)
                    return _isValid = false;
                return _isValid;
            }
        }

        public SRLatch(Func<int> quantity, int min, int max)
        {
            _quantity = quantity;
            Min = min;
            Max = max;
        }

        public static implicit operator bool(SRLatch latch)
            => latch.IsValid;
    }
}
