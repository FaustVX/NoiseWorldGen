using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using KB = Microsoft.Xna.Framework.Input.Keyboard;

namespace NoiseWorldGen.OpenGL.Inputs;

public sealed class Keyboard : IGameComponent, IUpdateable
{
    private KeyboardState _lastKeyboard = default!, _currentKeyboard = default!;
    public static Keyboard Instance { get; } = new();

    private Keyboard()
    { }
    bool IUpdateable.Enabled { get; } = true;

    int IUpdateable.UpdateOrder { get; } = 0;

    public event EventHandler<EventArgs>? EnabledChanged;

    public event EventHandler<EventArgs>? UpdateOrderChanged;

    void IGameComponent.Initialize()
    { }

    void IUpdateable.Update(GameTime gameTime)
    {
        _lastKeyboard = _currentKeyboard;
        _currentKeyboard = KB.GetState();
    }

    public bool IsClicked(Keys key)
        => _currentKeyboard.IsKeyDown(key) && _lastKeyboard.IsKeyUp(key);

    public bool IsClicked(Keys key, bool isExclusive)
        => _currentKeyboard.IsKeyDown(key) && _lastKeyboard.IsKeyUp(key) && (!isExclusive || _currentKeyboard.GetPressedKeyCount() == 1);

    public bool IsDown(Keys key)
        => _currentKeyboard.IsKeyDown(key);

    public static bool OrFunc(Func<Keys, bool> func, Keys key1, Keys key2)
        => func(key1) || func(key2);

    public static int XorFunc(Func<Keys, bool> keyFunc, Keys key1, Keys key2)
        => (keyFunc(key1), keyFunc(key2)) switch
        {
            (true, false) => -1,
            (false, true) => +1,
            _ => 0,
        };

    public static int XorFunc(Func<Keys, bool> keyFunc, Keys key1, Keys key1Alt, Keys key2, Keys key2Alt)
        => (keyFunc(key1) || keyFunc(key1Alt), keyFunc(key2) || keyFunc(key2Alt)) switch
        {
            (true, false) => -1,
            (false, true) => +1,
            _ => 0,
        };
}