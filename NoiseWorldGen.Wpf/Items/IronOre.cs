using System.Runtime.CompilerServices;

namespace NoiseWorldGen.Wpf.Items;

public class IronOre : Item
{
    [ModuleInitializer]
    internal static void Init()
        => MainWindowViewModel.OnLoadContent += _ =>
            Item.Add(new IronOre());
    private IronOre()
        : base("Iron Ore")
    { }
}