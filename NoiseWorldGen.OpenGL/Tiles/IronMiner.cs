using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace NoiseWorldGen.OpenGL.Tiles;

public sealed class IronMiner : OreMiner<IronOre>
{

    [ModuleInitializer]
    internal static void Init()
            => World.OnWorldCreated += _ =>
            {
                var texture = new Microsoft.Xna.Framework.Graphics.Texture2D(SpriteBatches.Pixel.GraphicsDevice, 1, 1);
                texture.SetData(new Color[] { Color.SlateGray });
                TileTemplates._tiles.Add(new TileTemplate.Dynamic((static (w, p) => new IronMiner(w, p)), texture, "Iron Miner"));
            };

    private IronMiner(World world, Point pos)
        : base(world, pos, "Iron", Color.SlateGray, default!)
    { }
}
