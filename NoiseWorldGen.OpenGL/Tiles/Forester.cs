using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace NoiseWorldGen.OpenGL.Tiles;

public sealed class Forester : TickedFeatureTile, Tile.INetworkReceiver
{

    [ModuleInitializer]
    internal static void Init()
        => Game.OnLoadContent += _ =>
        {
            var texture = new Microsoft.Xna.Framework.Graphics.Texture2D(SpriteBatches.Pixel.GraphicsDevice, 1, 1);
            texture.SetData(new Color[] { Color.LawnGreen });
            TileTemplates.Add<Forester>(new TileTemplate.Dynamic((static (w, p) => new Forester(w, p)), texture, "Forester"));
        };
    public override string Name => $"Forester";
    public int Distance { get; } = 5;
    public Networks.Network Network { get; set; } = default!;

    private Point? _lastOrePos;
    protected override void OnTick()
    {
        TickCount = 9;
        if (!Network.ContainsFeature<TreeCutter>(static tc =>
        {
            if(tc.TreeStored > 0)
            {
                tc.TreeStored--;
                return true;
            }
            return false;
        }))
            return;
        var rng = new Random();
        for (var i = 1; i <= Distance; i++)
        {
            var pos = Extensions.GetRandomPointinCircle(i, isfixedDistance: true) + Pos;
            if (World.GetFeatureTileAt(pos) is null && World.GetSoilTileAt(pos) is IsFeaturePlacable)
            {
                _lastOrePos = pos;
                World.SetFeatureTileAt(pos, Tree.Value);
                break;
            }
            else
                _lastOrePos = null;
        }
    }

    public override void Draw(Rectangle tileRect, World world, Point pos)
    {
        base.Draw(tileRect, world, pos);
        if (_lastOrePos is {} lastOre && SpriteBatches.UI is {} sb)
        {
            var destRectangle = tileRect.DrawAtWorldPos(pos, lastOre);
            sb.DrawLine(tileRect.Center.ToVector2(), destRectangle.Center.ToVector2(), Lerp(Color.Red, TickCount));
            sb.Draw(SpriteBatches.Pixel, destRectangle, Lerp(Color.Black, TickCount));
            static Color Lerp(Color color, int tickCount)
                => Color.Lerp(color * 0, color * 75f, tickCount / 10f);
        }
    }

    private Forester(World world, Point pos)
        : base(world, pos, Color.LawnGreen, default!)
    { }
}
