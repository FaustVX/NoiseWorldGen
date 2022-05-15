using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace NoiseWorldGen.OpenGL.Tiles;

public sealed class Tree : FeatureTile, ISingletonTile<Tree>, Tile.IsWalkable
{
    [ModuleInitializer]
    internal static void Init()
        => World.OnWorldCreated += _ =>
        {
            var texture = new Microsoft.Xna.Framework.Graphics.Texture2D(SpriteBatches.Pixel.GraphicsDevice, 1, 1);
            texture.SetData(new Color[] { Color.Green });
            TileTemplates._tiles.Add(new(() => Value, texture));
        };

    public static Tree Value { get; } = new();
    public override string Name => "Tree";

    private Tree()
        : base(Color.Green, default!)
    { }
}
