using System.Runtime.CompilerServices;

namespace NoiseWorldGen.OpenGL.Items;

public class Wood : Item
{
    [ModuleInitializer]
    internal static void Init()
        => Game.OnLoadContent += _ =>
            Item.Add(new Wood());
    private Wood()
        : base("Wood")
    { }
}