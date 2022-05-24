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
            if (World.GetFeatureTileAt(pos.X, pos.Y) is TOre ore)
            {
                _lastOrePos = pos;
                if (ore.Quantity > 1)
                    ore.Quantity--;
                else
                    World.SetFeatureTileAt(pos.X, pos.Y, null);
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
        if (_lastOrePos is {} lastOre)
            SpriteBatches.UI?.Draw(SpriteBatches.Pixel, tileRect.DrawAtWorldPos(pos, lastOre), Color.Lerp(Color.Black * 0, Color.Black * .75f, TickCount / 10f));
    }

    protected OreMiner(World world, Point pos, string oreName, Color color, Texture2D? texture, Rectangle? textureRect = null)
        : base(world, pos, color, texture, textureRect)
    {
        OreName = oreName;
    }
}
