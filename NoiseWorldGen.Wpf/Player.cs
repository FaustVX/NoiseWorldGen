using Microsoft.Xna.Framework;
using NoiseWorldGen.Wpf.Tiles;
using static NoiseWorldGen.Wpf.Inputs.Keyboard;
using Key = System.Windows.Input.Key;

namespace NoiseWorldGen.Wpf;

public class Player : IGameComponent, IUpdateable
{
    public event EventHandler<EventArgs>? EnabledChanged;

    public event EventHandler<EventArgs>? UpdateOrderChanged;

    public World World { get; }
    public Vector2 Position { get; set; } = new(.5f);
    public float Speed { get; } = .75f;
    public float SpeedMultipler { get; } = 5;
    public float FlySpeedMultiplier { get; } = 3f;
    public bool IsFlying { get; set; } = false;

    bool IUpdateable.Enabled { get; } = true;

    int IUpdateable.UpdateOrder { get; } = 0;

    public Player(World world)
    {
        World = world;
    }

    void IGameComponent.Initialize()
    {
        Inputs.Keyboard.RegisterKey(Key.LeftShift);
        Inputs.Keyboard.RegisterKey(Key.RightShift);
        Inputs.Keyboard.RegisterKey(Key.Z);
        Inputs.Keyboard.RegisterKey(Key.Q);
        Inputs.Keyboard.RegisterKey(Key.S);
        Inputs.Keyboard.RegisterKey(Key.D);
        Inputs.Keyboard.RegisterKey(Key.Up);
        Inputs.Keyboard.RegisterKey(Key.Left);
        Inputs.Keyboard.RegisterKey(Key.Down);
        Inputs.Keyboard.RegisterKey(Key.Right);
        Inputs.Keyboard.RegisterKey(Key.Space);
    }

    void IUpdateable.Update(GameTime gameTime)
    {
        var speed = Speed;
        if (Key.LeftShift.IsDown() || Key.RightShift.IsDown())
            speed *= SpeedMultipler;
        if (IsFlying)
            speed *= FlySpeedMultiplier;
        var pos = Position + speed * new Vector2()
        {
            X = Key.Q.IsDown() || Key.Left.IsDown() ? -1 : Key.D.IsDown() || Key.Right.IsDown() ? 1 : 0,
            Y = Key.Z.IsDown() || Key.Up.IsDown() ? -1 : Key.S.IsDown() || Key.Down.IsDown() ? 1 : 0,
        };
        if (IsFlying || World.GetSoilTileAt(pos.ToPoint()) is Tile.IsWalkable)
            Position = pos;
        IsFlying ^= Key.Space.IsPressed();
    }
}