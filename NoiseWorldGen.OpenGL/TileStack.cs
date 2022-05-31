namespace NoiseWorldGen.OpenGL;

public class TileStack
{
    public static readonly int Max = 100;
    private int quantity;
    public int Quantity
    {
        get => quantity;
        set
        {
            if (value <= 0 || value > Max)
                return;
            quantity = value;
        }
    }
    public TileTemplate Tile { get; }
    public bool IsEmpty
        => Quantity <= 0;

    public bool IsFullScreen
        => Quantity >= Max;

    public TileStack(TileTemplate tile)
    {
        Tile = tile;
    }
}