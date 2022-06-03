using System.Runtime.CompilerServices;

namespace NoiseWorldGen.OpenGL.Items;

public class CoalOre : Item
{
    [ModuleInitializer]
    internal static void Init()
        => Game.OnLoadContent += _ =>
            Item.Add(new CoalOre());
    private CoalOre()
        : base("Coal Ore")
    { }
}