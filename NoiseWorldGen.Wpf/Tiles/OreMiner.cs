using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NoiseWorldGen.Wpf.Tiles;

public abstract class OreMiner<TOre> : TickedFeatureTile, Tile.INetworkSupplier, Tile.INetworkReceiver
    where TOre : IOre
{
    public string OreName { get; }
    public override string Name => $"{OreName} Miner ({OreStored}, {WoodStored}, {Energy})";
    public ItemStack OreStored { get; }
    public ItemStack WoodStored { get; }
    public int Energy { get; set; }
    public virtual int Distance { get; } = 5;
    public Networks.Network Network { get; set; } = default!;
    private Point? _lastOrePos;
    private readonly SRLatch _requestWood, _requestEnergy;

    public override void Update(World world, Point pos)
    {
        if (_requestWood)
            this.Request(WoodStored);
        base.Update(world, pos);
    }

    protected override void OnTick()
    {
        TickCount = 9;
        _lastOrePos = null;
        if (OreStored.IsFull)
            return;
        if (_requestEnergy && WoodStored is { IsEmpty: false, item: Items.ICombustable c })
        {
            WoodStored.Quantity--;
            Energy += c.EnergyQuantity;
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

    bool INetworkSupplier.CanSupply(Items.Item item)
        => OreStored.item == item && !OreStored.IsEmpty;

    int INetworkSupplier.TrySupply(Items.Item item, int maxQuantity)
        => ((INetworkSupplier)this).CanSupply(item) ? OreStored.Request(maxQuantity) : 0;

    protected OreMiner(World world, Point pos, Items.Item oreItem, Color color, Texture2D? texture, Rectangle? textureRect = null)
        : base(world, pos, color, texture, textureRect)
    {
        OreName = oreItem.Name;
        OreStored = new(oreItem);
        WoodStored = new(Items.Item.Get<Items.Wood>());
        _requestEnergy = new(() => Energy, 75, 100);
        _requestWood = new(() => WoodStored.Quantity, 5, 10);
    }
}
