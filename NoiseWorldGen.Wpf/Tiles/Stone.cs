using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace NoiseWorldGen.Wpf.Tiles;

public sealed class Stone : SoilTile, ISingletonTile<Stone>, Tile.IsWalkable, Tile.IsOrePlacable
{
    [ModuleInitializer]
    internal static void Init()
        => MainWindowViewModel.OnLoadContent += _ =>
        {
            var texture = new Microsoft.Xna.Framework.Graphics.Texture2D(SpriteBatches.Pixel.GraphicsDevice, 1, 1);
            texture.SetData(new Color[] { Color.DarkGray });
            TileTemplates.Add<Stone>(new TileTemplate.Static(() => Value, texture, "Stone"));
        };

    public static Stone Value { get; } = new();
    public override string Name => "Stone";

    private Stone()
        : base(Color.DarkGray, default!)
    { }
}
