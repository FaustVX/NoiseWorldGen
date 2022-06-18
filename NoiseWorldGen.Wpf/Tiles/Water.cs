using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace NoiseWorldGen.Wpf.Tiles;

public sealed class Water : SoilTile, ISingletonTile<Water>
{
    [ModuleInitializer]
    internal static void Init()
        => MainWindowViewModel.OnCreateGraphicDevice += gd =>
        {
            var texture = new Microsoft.Xna.Framework.Graphics.Texture2D(gd, 1, 1);
            texture.SetData(new Color[] { Color.DarkBlue });
            TileTemplates.Add<Water>(new TileTemplate.Static(() => Value, texture, "Water"));
        };

    public static Water Value { get; } = new();
    public override string Name => "Water";

    private Water()
        : base(Color.DarkBlue, default!)
    { }
}
