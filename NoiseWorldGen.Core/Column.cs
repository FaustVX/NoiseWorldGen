namespace NoiseWorldGen.Core;

public class Column
{
    public World World { get; }
    public Block[] Blocks { get; }

    public Column(World world, int x)
    {
        World = world;
        Blocks = new Block[World.Height];
        var baseHeight = World.Continentalness.Generate(x);
        Blocks[0] = Block.Bedrock;
        for (int y = 1; y < World.Height; y++)
        {
            Blocks[y] = y switch
            {
                _ when y <= baseHeight && !(World.NoodleCave.Generate(x, y) || World.Cave.Generate(x, y)) => Block.Stone,
                _ => Block.Air,
            };
        }

        var dirtHeight = World.Dirt.Generate(x);
        for (int y = 0; y < World.Height; y++)
            Blocks[y] = (Blocks[y], y) switch
            {
                (Block.Stone, _) when y > baseHeight - dirtHeight && y <= World.WaterLevel => Block.Sand,
                (Block.Stone, _) when y > baseHeight - dirtHeight => Block.Dirt,
                (Block.Air, _) when y <= World.WaterLevel => Block.Water,
                (var block, _) => block
            };
    }
}
