namespace NoiseWorldGen.Core;

public class Column
{
    public World World { get; }
    public Block[] Blocks { get; }

    public Column(World world, int x)
    {
        World = world;
        Blocks = new Block[World.Height];
        var baseHeight = World.GetBaseHeight(x);
        Blocks[0] = Block.Bedrock;
        for (int y = 1; y < World.Height; y++)
        {
            var isCave = World.IsNoodleCave(x, y) || World.IsCave(x, y);
            Blocks[y] = y switch
            {
                _ when y < baseHeight => isCave ? Block.Air : Block.Stone,
                _ when y < World.WaterLevel => Block.Water,
                _ => Block.Air,
            };
        }
    }
}
