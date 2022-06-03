using System.Runtime.CompilerServices;

namespace NoiseWorldGen.OpenGL.Items;

public class IronOre : Item
{
    [ModuleInitializer]
    internal static void Init()
        => Game.OnLoadContent += _ =>
            Item.Add(new IronOre());
    private IronOre()
        : base("Iron Ore")
    { }
}