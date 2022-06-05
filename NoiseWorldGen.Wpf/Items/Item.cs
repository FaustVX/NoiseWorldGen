namespace NoiseWorldGen.Wpf.Items;

public abstract class Item
{
    private static readonly Dictionary<Type, Item> _items = new();
    protected Item(string name)
        => Name = name;

    public string Name { get; }

    protected static void Add<T>(T item)
        where T : Item
        => _items[typeof(T)] = item;

    public static T Get<T>()
        where T : Item
        => (T)_items[typeof(T)];
}