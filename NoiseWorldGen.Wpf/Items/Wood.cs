using System.Runtime.CompilerServices;

namespace NoiseWorldGen.Wpf.Items;

public class Wood : Item, ICombustable
{
    [ModuleInitializer]
    internal static void Init()
        => MainWindowViewModel.OnLoadContent += _ =>
            Item.Add(new Wood());
    private Wood()
        : base("Wood")
    { }

    public int EnergyQuantity => 3;
}