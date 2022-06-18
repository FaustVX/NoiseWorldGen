using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace NoiseWorldGen.Wpf.Tiles;

public sealed class Mountain : SoilTile, ISingletonTile<Mountain>, Tile.IsWalkable, Tile.IsOrePlacable
{
    [ModuleInitializer]
    internal static void Init()
        => MainWindowViewModel.OnCreateGraphicDevice += gd =>
        {
            var texture = new Microsoft.Xna.Framework.Graphics.Texture2D(gd, 1, 1);
            texture.SetData(new Color[] { Color.DimGray });
            TileTemplates.Add<Mountain>(new TileTemplate.Static(() => Value, texture, "Mountain"));
        };

    public static Mountain Value { get; } = new();
    public override string Name => "Mountain";

    private Mountain()
        : base(Color.DimGray, default!)
    { }
}
