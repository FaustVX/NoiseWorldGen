namespace NoiseWorldGen.Wpf.Tiles.Windows;

public interface IWindow
{
    public Type WindowType { get; }
}

public abstract class Window : System.Windows.Window
{
    public FeatureTile Tile { get; }

    protected Window(FeatureTile tile)
    {
        DataContext = Tile = tile;
    }
}