using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace NoiseWorldGen.OpenGL.Tiles;

public sealed class CoalMiner : TickedFeatureTile
{

    [ModuleInitializer]
    internal static void Init()
            => World.OnWorldCreated += _ =>
            {
                var texture = new Microsoft.Xna.Framework.Graphics.Texture2D(SpriteBatches.Pixel.GraphicsDevice, 1, 1);
                texture.SetData(new Color[] { Color.DimGray });
                TileTemplates._tiles.Add(new TileTemplate.Dynamic((static (w, p) => new CoalMiner(w, p)), texture));
            };
    public override string Name => $"Coal Miner ({TickCount} ticks, {CoalStored} Coals)";
    public int CoalStored { get; set; }

    protected override void OnTick()
    {
        TickCount = 9;
        var rng = new Random();
        var pos = Extensions.GetRandomPointinCircle(5) + Pos;
        if (World.GetFeatureTileAt(pos.X, pos.Y) is CoalOre ore)
        {
            if (ore.Quantity > 1)
                ore.Quantity--;
            else
                World.SetFeatureTileAt(pos.X, pos.Y, null);
            CoalStored++;
        }
    }

    private CoalMiner(World world, Point pos)
        : base(world, pos, Color.DimGray, default!)
    { }
}
