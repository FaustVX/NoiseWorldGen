using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace NoiseWorldGen.OpenGL.Tiles;

public sealed class Water : SoilTile, ISingletonTile<Water>
{
    [ModuleInitializer]
    internal static void Init()
        => Game.OnLoadContent += _ =>
        {
            var texture = new Microsoft.Xna.Framework.Graphics.Texture2D(SpriteBatches.Pixel.GraphicsDevice, 1, 1);
            texture.SetData(new Color[] { Color.DarkBlue });
            TileTemplates._tiles.Add(new TileTemplate.Static(() => Value, texture, "Water"));
        };

    public static Water Value { get; } = new();
    public override string Name => "Water";

    private Water()
        : base(Color.DarkBlue, default!)
    { }
}
