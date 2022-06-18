namespace NoiseWorldGen.Wpf.Tiles.Windows;

public interface IWindow
{
    public Type WindowType { get; }
}

public abstract class FeatureTileWindow : Window
{
    public FeatureTile Tile { get; }

    protected FeatureTileWindow(FeatureTile tile)
    {
        DataContext = Tile = tile;
    }
}