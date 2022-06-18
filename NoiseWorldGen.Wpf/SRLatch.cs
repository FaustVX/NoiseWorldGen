namespace NoiseWorldGen.Wpf;

public class SRLatch
{
    private readonly Func<int> _quantity;
    public int Min { get; }
    public int Max { get; }
    private bool _isValid;
    public bool IsValid
    {
        get
        {
            if (_quantity() < Min)
                return _isValid = true;
            if (_quantity() > Max)
                return _isValid = false;
            return _isValid;
        }
    }

    public SRLatch(Func<int> quantity, int min, int max)
    {
        _quantity = quantity;
        Min = min;
        Max = max;
    }

    public static implicit operator bool(SRLatch latch)
        => latch.IsValid;
}
