using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NoiseWorldGen.Wpf;

public static class SpriteBatches
{
    private static SpriteBatch? _ui = default!;

    [ModuleInitializer]
    internal static void Init()
            => Wpf.MainWindowViewModel.OnCreateGraphicDevice += gd =>
            {
                SpriteBatches.Game = new(gd);
                SpriteBatches.UI = new(gd);
                SpriteBatches.Pixel = new Texture2D(gd, 1, 1);
                SpriteBatches.Pixel.SetData(new[] { Color.White });
            };
    public static Texture2D Pixel { get; private set; } = default!;
    public static SpriteBatch Game { get; private set; } = default!;
    public static SpriteBatch? UI
    {
        get => ShowUI ? _ui : null;
        private set => _ui = value;
    }
    public static bool ShowUI { get; set; } = true;
}