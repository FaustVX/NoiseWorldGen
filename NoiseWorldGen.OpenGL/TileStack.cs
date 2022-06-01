namespace NoiseWorldGen.OpenGL;

public class TileStack
{
    public static readonly int Max = 99;
    private int quantity;
    public int Quantity
    {
        get => quantity;
        set
        {
            if (value < 0 || value > Max)
                return;
            quantity = value;
        }
    }
    public TileTemplate Tile { get; }
    public bool IsEmpty
        => Quantity <= 0;

    public bool IsFull
        => Quantity >= Max;

    public int RemainingQuantity
        => Max - Quantity;

    public int Request(int quantity)
    {
        quantity = Math.Min(quantity, Quantity);
        Quantity -= quantity;
        return quantity;
    }

    public TileStack(TileTemplate tile)
    {
        Tile = tile;
    }

    public override string ToString()
        => $"{Quantity} {Tile.Name}";
}