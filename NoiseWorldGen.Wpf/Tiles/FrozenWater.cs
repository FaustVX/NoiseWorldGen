using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace NoiseWorldGen.Wpf.Tiles;

public sealed class FrozenWater : SoilTile, ISingletonTile<FrozenWater>, Tile.IsWalkable
{
    [ModuleInitializer]
    internal static void Init()
        => MainWindowViewModel.OnLoadContent += _ =>
        {
            var texture = new Microsoft.Xna.Framework.Graphics.Texture2D(SpriteBatches.Pixel.GraphicsDevice, 1, 1);
            texture.SetData(new Color[] { Color.CornflowerBlue });
            TileTemplates.Add<FrozenWater>(new TileTemplate.Static(() => Value, texture, "Frozen Water"));
        };

    public static FrozenWater Value { get; } = new();
    public override string Name => "Frozen Water";

    private FrozenWater()
        : base(Color.CornflowerBlue, default!)
    { }
}
