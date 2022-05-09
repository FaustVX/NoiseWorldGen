using Microsoft.Xna.Framework;
using NoiseWorldGen.OpenGL.Inputs;
using NoiseWorldGen.OpenGL.Tiles;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace NoiseWorldGen.OpenGL;

public class Player : IGameComponent, IUpdateable
{
    public event EventHandler<EventArgs>? EnabledChanged;

    public event EventHandler<EventArgs>? UpdateOrderChanged;

    public World World { get; }
    public Point<float> Position { get; set; } = new Point<float>(.5f);
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
        if (Keyboard.Instance.IsDown(Keys.RightShift) || Keyboard.Instance.IsDown(Keys.LeftShift))
            speed *= SpeedMultipler;
        if (IsFlying)
            speed *= FlySpeedMultiplier;
        var pos = Position with
        {
            X = Position.X + speed * Keyboard.XorFunc(Keyboard.Instance.IsDown, Keys.Q, Keys.Left, Keys.D, Keys.Right),
            Y = Position.Y + speed * Keyboard.XorFunc(Keyboard.Instance.IsDown, Keys.Z, Keys.Up, Keys.S, Keys.Down),
        };
        if (IsFlying || World.GetTileAt((int)pos.X, (int)pos.Y) is Tile.IsWalkable)
            Position = pos;
        IsFlying ^= Keyboard.Instance.IsClicked(Keys.Space);
    }
}