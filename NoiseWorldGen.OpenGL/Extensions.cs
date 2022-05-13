namespace NoiseWorldGen.OpenGL;

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
}
