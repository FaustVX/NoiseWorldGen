namespace NoiseWorldGen.OpenGL;

public class ItemStack
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
    public Items.Item item { get; }

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

    public ItemStack(Items.Item item)
    {
        this.item = item;
    }

    public override string ToString()
        => $"{Quantity} {item.Name}";
}