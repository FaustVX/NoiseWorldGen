using System.Runtime.CompilerServices;

namespace NoiseWorldGen.Wpf.Items;

public class CoalOre : Item
{
    [ModuleInitializer]
    internal static void Init()
        => MainWindowViewModel.OnLoadContent += _ =>
            Item.Add(new CoalOre());
    private CoalOre()
        : base("Coal Ore")
    { }
}