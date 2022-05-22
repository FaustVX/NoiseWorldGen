using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace NoiseWorldGen.OpenGL.Tiles;

public sealed class FrozenWater : SoilTile, ISingletonTile<FrozenWater>, Tile.IsWalkable
{
    [ModuleInitializer]
    internal static void Init()
        => World.OnWorldCreated += _ =>
        {
            var texture = new Microsoft.Xna.Framework.Graphics.Texture2D(SpriteBatches.Pixel.GraphicsDevice, 1, 1);
            texture.SetData(new Color[] { Color.CornflowerBlue });
            TileTemplates._tiles.Add(new TileTemplate.Static(() => Value, texture, "Frozen Water"));
        };

    public static FrozenWater Value { get; } = new();
    public override string Name => "Frozen Water";

    private FrozenWater()
        : base(Color.CornflowerBlue, default!)
    { }
}
