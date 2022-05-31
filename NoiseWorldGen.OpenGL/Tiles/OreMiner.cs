using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NoiseWorldGen.OpenGL.Tiles;

public abstract class OreMiner<TOre> : TickedFeatureTile
    where TOre : IOre
{
    public string OreName { get; }
    public override string Name => $"{OreName} Miner ({OreStored} {OreName})";
    public int OreStored { get; set; }
    public virtual int Distance { get; } = 5;
    private Point? _lastOrePos;

    protected override void OnTick()
    {
        TickCount = 9;
        for (var i = 1; i <= Distance; i++)
        {
            var pos = Extensions.GetRandomPointinCircle(i, isfixedDistance: true) + Pos;
            if (World.GetFeatureTileAt(pos) is {} tile and TOre)
            {
                _lastOrePos = pos;
                tile.Mine(World, pos, this);
                OreStored++;
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

    protected OreMiner(World world, Point pos, string oreName, Color color, Texture2D? texture, Rectangle? textureRect = null)
        : base(world, pos, color, texture, textureRect)
    {
        OreName = oreName;
    }
}
