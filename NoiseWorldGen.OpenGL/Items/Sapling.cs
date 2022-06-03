using System.Runtime.CompilerServices;

namespace NoiseWorldGen.OpenGL.Items;

public class Sapling : Item
{
    [ModuleInitializer]
    internal static void Init()
        => Game.OnLoadContent += _ =>
            Item.Add(new Sapling());
    private Sapling()
        : base("Sapling")
    { }
}