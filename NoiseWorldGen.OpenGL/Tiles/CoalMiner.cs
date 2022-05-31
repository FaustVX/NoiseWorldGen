using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace NoiseWorldGen.OpenGL.Tiles;

public sealed class CoalMiner : OreMiner<CoalOre>
{

    [ModuleInitializer]
    internal static void Init()
        => Game.OnLoadContent += _ =>
        {
            var texture = new Microsoft.Xna.Framework.Graphics.Texture2D(SpriteBatches.Pixel.GraphicsDevice, 1, 1);
            texture.SetData(new Color[] { Color.DimGray });
            TileTemplates.Add<CoalMiner>(new TileTemplate.Dynamic((static (w, p) => new CoalMiner(w, p)), texture, "Coal Miner"));
        };

    private CoalMiner(World world, Point pos)
        : base(world, pos, "Coal", Color.DimGray, default!)
    { }
}
