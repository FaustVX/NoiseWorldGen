namespace NoiseWorldGen.Core;

public class Column
{
    public World World { get; }
    public Block[] Blocks { get; }

    public Column(World world, int x)
    {
        World = world;
        Blocks = new Block[World.Height];
        var baseHeight = World.Noise.Generate(x) + World.MinHeight;
        Blocks[0] = Block.Bedrock;
        for (int y = 1; y < World.Height; y++)
        {
            Blocks[y] = y switch
            {
                _ when y < baseHeight => Block.Stone,
                _ when y < World.WaterLevel => Block.Water,
                _ => Block.Air,
            };
        }
    }
}
