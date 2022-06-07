using Microsoft.Xna.Framework;
using M = System.Windows.Input.Mouse;
using MBS = System.Windows.Input.MouseButtonState;

namespace NoiseWorldGen.Wpf.Inputs;

public static class Mouse
{
    public enum Button
    {
        LeftClick,
        MiddleClick,
        RightClick,
        X1,
        X2,
    }

    static Mouse()
    {
        M.AddMouseWheelHandler(App.Current.MainWindow, (s, e) => ScroolWheelDirection = -Math.Sign(e.Delta));
    }

    private static readonly Dictionary<Button, (MBS previous, MBS current)> _states = new()
    {
        [Button.LeftClick] = default,
        [Button.MiddleClick] = default,
        [Button.RightClick] = default,
        [Button.X1] = default,
        [Button.X2] = default,
    };

    public static Point MousePosition { get; private set; }
    public static int ScroolWheelDirection { get; private set; }

    public static void Update()
    {
        MousePosition = M.GetPosition(App.Current.MainWindow) is { X: var x, Y : var y }
            ? new((int)x, (int)y)
            : default;

        ScroolWheelDirection = 0;

        foreach (var button in _states.Keys)
            _states[button] = (_states[button].current, GetCurrentStates(button));
    }

    public static bool IsPressed(this Button button)
        => _states[button] is (MBS.Released, MBS.Pressed);

    public static bool IsDown(this Button button)
        => _states[button].current is MBS.Pressed;

    private static MBS GetCurrentStates(Button button)
        => button switch
        {
            Button.LeftClick => M.LeftButton,
            Button.MiddleClick => M.MiddleButton,
            Button.RightClick => M.RightButton,
            Button.X1 => M.XButton1,
            Button.X2 => M.XButton2,
        };
}