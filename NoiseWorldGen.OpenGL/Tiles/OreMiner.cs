using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NoiseWorldGen.OpenGL.Tiles;

public abstract class OreMiner<TOre> : TickedFeatureTile, Tile.INetworkSupplier
    where TOre : IOre
{
    public string OreName { get; }
    public override string Name => $"{OreName} Miner ({OreStored}, {TreeStored}, {Energy})";
    public TileStack OreStored { get; }
    public TileStack TreeStored { get; }
    public int Energy { get; set; }
    public virtual int Distance { get; } = 5;
    public Networks.Network Network { get; set; } = default!;
    private Point? _lastOrePos;

    public override void Update(World world, Point pos)
    {
        Network.Request(TreeStored);
        base.Update(world, pos);
    }

    protected override void OnTick()
    {
        TickCount = 9;
        _lastOrePos = null;
        if (OreStored.IsFull)
            return;
        if (Energy < 10 && !TreeStored.IsEmpty)
        {
            TreeStored.Quantity--;
            Energy += 5;
        }
        if (Energy <= 0)
            return;
        for (var i = 1; i <= Distance; i++)
        {
            var pos = Extensions.GetRandomPointinCircle(i, isfixedDistance: true) + Pos;
            if (World.GetFeatureTileAt(pos) is {} tile and TOre)
            {
                _lastOrePos = pos;
                Energy--;
                tile.Mine(World, pos, this);
                OreStored.Quantity++;
                break;
            }
        }
    }

    public override void Draw(Rectangle tileRect, World world, Point pos)
    {
        base.Draw(tileRect, world, pos);
        if (_lastOrePos is {} lastOre && SpriteBatches.UI is {} sb)
        {
            var destRectangle = tileRect.DrawAtWorldPos(pos, lastOre);
            sb.DrawLine(tileRect.Center, destRectangle.Center, Lerp(Color.Red, TickCount));
            sb.Draw(SpriteBatches.Pixel, destRectangle, Lerp(Color.Black, TickCount));
            static Color Lerp(Color color, int tickCount)
                => Color.Lerp(color * 0, color * 75f, tickCount / 10f);
        }
    }

    bool INetworkSupplier.CanSupply(TileTemplate tileTemplate)
        => OreStored.Tile == tileTemplate && !OreStored.IsEmpty;

    int INetworkSupplier.TrySupply(TileTemplate tileTemplate, int maxQuantity)
        => ((INetworkSupplier)this).CanSupply(tileTemplate) ? OreStored.Request(maxQuantity) : 0;

    protected OreMiner(World world, Point pos, TileTemplate tileTemplate, Color color, Texture2D? texture, Rectangle? textureRect = null)
        : base(world, pos, color, texture, textureRect)
    {
        OreName = tileTemplate.Name;
        OreStored = new(tileTemplate);
        TreeStored = new(TileTemplates.Get<Tree>());
    }
}
