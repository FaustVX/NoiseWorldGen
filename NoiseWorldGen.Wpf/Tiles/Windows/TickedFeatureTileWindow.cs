using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace NoiseWorldGen.Wpf.Tiles.Windows;

public class TickedFeatureTileWindow : FeatureTileWindow
{
    protected readonly ProgressBar _tick;
    protected readonly ToggleButton _pause;
    protected readonly Panel _panel;
    public new TickedFeatureTile Tile => (TickedFeatureTile)base.Tile;
    public TickedFeatureTileWindow(TickedFeatureTile tile)
        : base(tile)
    {
        _tick = new()
        {
            Maximum = 9,
        };
        _pause = new()
        {
            Content = "Pause",
        };

        _tick.SetBinding(ProgressBar.ValueProperty, new Binding(nameof(TickedFeatureTile.TickCount)));
        _tick.SetBinding(ProgressBar.ToolTipProperty, new Binding(nameof(TickedFeatureTile.TickCount)));

        _pause.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(TickedFeatureTile.IsPaused)));
        _pause.Checked += (s, e) => Tile.IsPaused = ((ToggleButton)s).IsChecked!.Value;

        this.SetBinding(Window.TitleProperty, new Binding(nameof(TickedFeatureTile.Name)));

        Content = _panel = new StackPanel()
        {
            Orientation = Orientation.Vertical,
            Children =
            {
                _tick,
                _pause,
            },
        };
    }
}