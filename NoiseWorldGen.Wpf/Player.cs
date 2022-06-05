using Microsoft.Xna.Framework;
using Key = System.Windows.Input.Key;
using NoiseWorldGen.Wpf.Tiles;

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
    { }

    void IUpdateable.Update(GameTime gameTime)
    {
        var speed = Speed;
        if (Key.LeftShift.IsKeyDown() || Key.RightShift.IsKeyDown())
            speed *= SpeedMultipler;
        if (IsFlying)
            speed *= FlySpeedMultiplier;
        var pos = Position + speed * new Vector2()
        {
            X = Key.Q.IsKeyDown() || Key.Left.IsKeyDown() ? -1 : Key.D.IsKeyDown() || Key.Right.IsKeyDown() ? 1 : 0,
            Y = Key.Z.IsKeyDown() || Key.Up.IsKeyDown() ? -1 : Key.S.IsKeyDown() || Key.Down.IsKeyDown() ? 1 : 0,
        };
        if (IsFlying || World.GetSoilTileAt(pos.ToPoint()) is Tile.IsWalkable)
            Position = pos;
        IsFlying ^= Key.Space.IsKeyClicked();
    }
}