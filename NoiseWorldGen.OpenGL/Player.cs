namespace NoiseWorldGen.OpenGL;

public class Player
{
    public World World { get; }
    public Point<float> Position { get; set; } = new Point<float>(.5f);
    public float Speed { get; } = .75f;
    public float SpeedMultipler { get; } = 5;
    public float FlySpeedMultiplier { get; } = 3f;
    public bool IsFlying { get; set; } = false;

    public Player(World world)
    {
        World = world;
    }
}