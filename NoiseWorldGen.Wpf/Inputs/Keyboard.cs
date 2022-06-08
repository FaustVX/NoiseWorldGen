using System.Windows.Input;
using KB = System.Windows.Input.Keyboard;

namespace NoiseWorldGen.Wpf.Inputs;

public static class Keyboard
{
    private static readonly Dictionary<Key, (bool previous, bool current)> _registeredKeys = new();

    public static void RegisterKey(Key key)
        => _registeredKeys[key] = new();

    public static void Update()
        => _registeredKeys.Update(KB.IsKeyDown);

    public static bool IsPressed(this Key key)
        => _registeredKeys[key] is (false, true);

    public static bool IsPressed(this (Key, Key) keys)
        => IsPressed(keys.Item1) || IsPressed(keys.Item2);

    public static bool IsDown(this Key key)
        => _registeredKeys[key].current;

    public static bool IsDown(this (Key, Key) keys)
        => IsDown(keys.Item1) || IsDown(keys.Item2);

    public static int IsXor(this (Key, Key) keys, Func<Key, bool> func)
        => (func(keys.Item1), func(keys.Item2)) switch
        {
            (true, false) => -1,
            (false, true) => +1,
            _ => 0,
        };
}