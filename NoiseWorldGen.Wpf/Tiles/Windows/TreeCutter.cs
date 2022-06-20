using System.Windows.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NoiseWorldGen.Wpf.Tiles.Windows;

public class TreeCutter : TickedFeatureTileWindow
{
    private sealed class ItemStackVM : MonoGameControls.MonoGameViewModel
    {
        public ItemStackVM(ItemStack stack)
        {
            Stack = stack;
        }

        public ItemStack Stack { get; }
        private SpriteBatch _sb = default!;

        public override void Initialize()
        {
            base.Initialize();
            _sb = CreateSpriteBatch(nameof(_sb));
            Components.Add(new Components.UpdatePerSec(_sb, Content));
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _sb.DrawLine(new Point(5, 0), new(5, GraphicsDevice.Viewport.Height), 5, Color.Green);
            _sb.DrawLine(new Vector2(5, GraphicsDevice.Viewport.Height), new(5, GraphicsDevice.Viewport.Height - (float)GraphicsDevice.Viewport.Height / ItemStack.Max * Stack.Quantity), Color.Red);
        }
    }
    public new Tiles.TreeCutter Tile => (Tiles.TreeCutter)base.Tile;
    protected readonly MonoGameControls.MonoGameContentControl _sappling, _wood;
    private readonly ItemStackVM _sapplingVM, _woodVM;

    public TreeCutter(Tiles.TreeCutter tile)
        : base(tile)
    {
        _wood = new()
        {
            Window = this,
        };
        _sappling = new()
        {
            Window = this,
        };

        Content = null;
        Content = new Grid()
        {
            RowDefinitions =
            {
                new()
                {
                    Height = System.Windows.GridLength.Auto,
                },
                new()
                {
                    Height = new(1, System.Windows.GridUnitType.Star),
                },
            },
            ColumnDefinitions =
            {
                new()
                {
                    Width = new(1, System.Windows.GridUnitType.Star),
                },
                new()
                {
                    Width = new(1, System.Windows.GridUnitType.Star),
                },
            },
            Children =
            {
                _panel,
                _wood,
                _sappling,
            },
        };

        _panel.SetValue(Grid.ColumnSpanProperty, 2);

        _wood.SetValue(Grid.RowProperty, 1);
        _wood.SetValue(Grid.ColumnProperty, 0);
        _wood.DataContext = _woodVM = new(Tile.WoodStored);

        _sappling.SetValue(Grid.RowProperty, 1);
        _sappling.SetValue(Grid.ColumnProperty, 1);
        _sappling.DataContext = _sapplingVM = new(Tile.SapplingStored);
    }
}