using System.Linq;
using NoiseWorldGen.OpenGL.Tiles;

namespace NoiseWorldGen.OpenGL.Biomes;

public abstract class Biome
{
    public interface IBiome<T>
        where T : IBiome<T>
    {
        public static abstract T Create(World world);
        public static abstract (float min, float max)? Continentalness { get; }
        public static abstract (float min, float max)? Temperature { get; }
    }
    private static readonly List<Type> _biomes = new();
    public static IReadOnlyList<Type> Biomes => _biomes;
    public World World { get; }
    public abstract Tile BaseTile { get; }

    public abstract Tile GenerateTile(int x, int y);

    protected Biome(World world)
    {
        World = world;
    }

    protected static void AddBiome<T>()
        where T : Biome, IBiome<T>
        => _biomes.Add(typeof(T));

    public static Biome GetBiome(float continentalness, float temperature, World world)
    {
        var possibleBiomes = new List<(Type type, float continentalness, float temperature)>(Biomes.Count);
        foreach (var type in Biomes)
        {
            if (IsPropertyValid(type, "Continentalness", continentalness, out var cont) && IsPropertyValid(type, "Temperature", temperature, out var temp))
                possibleBiomes.Add((type, cont, temp));
        }

        return Create(possibleBiomes
            .OrderByDescending(static t => t.continentalness * t.temperature)
            .First().type, world);

        static bool IsPropertyValid(Type type, string property, float value, out float percent)
            => type.GetProperty(property)!.GetValue(null) switch
            {
                null => ReturnOut(true, 1f, out percent),
                (float min, float max) when min <= value && value < max => ReturnOut(true, MathF.Abs((value - min) / (max - min) - .5f), out percent),
                _ => ReturnOut(false, 0f, out percent),
            };

        static TReturn ReturnOut<TReturn, TOut>(TReturn @return, in TOut @in, out TOut @out)
        {
            @out = @in;
            return @return;
        }

        static Biome Create(Type type, World world)
            => type.GetMethod("Create")!.CreateDelegate<Func<World, Biome>>()(world);
    }
}
