using System.Runtime.CompilerServices;

namespace NoiseWorldGen.Wpf.Items;

public class Sapling : Item
{
    [ModuleInitializer]
    internal static void Init()
        => MainWindowViewModel.OnLoadContent += _ =>
            Item.Add(new Sapling());
    private Sapling()
        : base("Sapling")
    { }
}