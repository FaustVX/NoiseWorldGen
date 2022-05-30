using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace NoiseWorldGen.OpenGL.Tiles;

public sealed class FrozenMountain : SoilTile, ISingletonTile<FrozenMountain>, Tile.IsWalkable, Tile.IsOrePlacable
{
    [ModuleInitializer]
    internal static void Init()
        => Game1.OnLoadContent += _ =>
        {
            var texture = new Microsoft.Xna.Framework.Graphics.Texture2D(SpriteBatches.Pixel.GraphicsDevice, 1, 1);
            texture.SetData(new Color[] { Color.WhiteSmoke });
            TileTemplates._tiles.Add(new TileTemplate.Static(() => Value, texture, "Frozen Mountain"));
        };

    public static FrozenMountain Value { get; } = new();
    public override string Name => "Frozen Mountain";

    private FrozenMountain()
        : base(Color.WhiteSmoke, default!)
    { }
}
