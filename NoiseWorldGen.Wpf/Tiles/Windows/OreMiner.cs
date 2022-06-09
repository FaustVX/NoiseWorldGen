using System.Windows.Controls;
using System.Windows.Data;

namespace NoiseWorldGen.Wpf.Tiles.Windows;

public class OreMiner : Window
{
    private readonly ProgressBar _tick;
    public OreMiner(FeatureTile tile)
        : base(tile)
    {
        _tick = new ProgressBar()
        {
            Maximum = 9,
        };

        _tick.SetBinding(ProgressBar.ValueProperty, new Binding(nameof(TickedFeatureTile.TickCount)));
        _tick.SetBinding(ProgressBar.ToolTipProperty, new Binding(nameof(TickedFeatureTile.TickCount)));
        this.SetBinding(Window.TitleProperty, new Binding(nameof(TickedFeatureTile.Name)));

        Content = _tick;
    }
}