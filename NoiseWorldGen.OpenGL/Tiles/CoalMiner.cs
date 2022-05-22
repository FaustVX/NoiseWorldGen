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
    public override string Name => $"Coal Miner ({TickCount} ticks, {CoalStored} coals)";
    public int CoalStored { get; set; }
    public int Distance { get; } = 5;

    protected override void OnTick()
    {
        TickCount = 9;
        var rng = new Random();
        for (var i = 1; i <= Distance; i++)
        {
            var pos = Extensions.GetRandomPointinCircle(i, isfixedDistance: true) + Pos;
            if (World.GetFeatureTileAt(pos.X, pos.Y) is CoalOre ore)
            {
                if (ore.Quantity > 1)
                    ore.Quantity--;
                else
                    World.SetFeatureTileAt(pos.X, pos.Y, null);
                CoalStored++;
                break;
            }
        }
    }

    private CoalMiner(World world, Point pos)
        : base(world, pos, Color.DimGray, default!)
    { }
}
