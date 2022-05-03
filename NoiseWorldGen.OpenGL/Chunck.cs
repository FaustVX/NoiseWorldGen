namespace NoiseWorldGen.OpenGL;

public class Chunck
{
    public static readonly byte Size = 8;

    private readonly Tile[,] Tiles;

    public Chunck(World world, int chunckX, int chunckY)
    {
        World = world;
        ChunckX = chunckX;
        ChunckY = chunckY;
        Tiles = PopulateArray(Size, Size, GenerateTile);
    }

    private Tile GenerateTile(int x, int y)
    {
        int tileX = ChunckX * Size + x;
        int tileY = ChunckY * Size + y;
        var continentalness = World.Continentalness.GetNoise(tileX, tileY);
        if (continentalness >= World.WaterHeight)
        {
            var heightness = continentalness >= World.MountainHeight;
            Tile tile = Tile.GetInterpolatedNoise<RiverWater>(tileX, tileY) is 1 ? RiverWater.Value
                : heightness ? Mountain.Value : Stone.Value;
            if (tile is Tile.IsOrePlacable)
            {
                IOre? ore = default!;
                GenerateOre(tileX, tileY, ref ore, qty => new IronOre(qty));
                GenerateOre(tileX, tileY, ref ore, qty => new CoalOre(qty));
                tile = ore as Tile ?? tile;

                static void GenerateOre<T>(float tileX, float tileY, ref IOre? ore, Func<uint, T> ctor)
                    where T : IInterpolation<T>, IOre
                    => ore = Tile.GetInterpolatedNoise<T>(tileX, tileY) is > 0 and var qty && (ore?.Quantity ?? 0) < qty
                        ? ctor((uint)qty)
                        : ore;
            }

            return tile;
        }
        else
        {
            return continentalness > World.ShallowWaterHeight ? ShallowWater.Value :
                continentalness < World.DeepWaterHeight ? Water.Value :
                DeepWater.Value;
        }
    }

    public Tile this[int x, int y]
    {
        get => Tiles[x, y];
        set => Tiles[x, y] = value ?? Stone.Value;
    }

    public World World { get; }
    public int ChunckX { get; }
    public int ChunckY { get; }
}
