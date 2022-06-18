﻿using System;
using System.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace NoiseWorldGen.Wpf.MonoGameControls
{
    public interface IMonoGameViewModel : IDisposable
    {
        IGraphicsDeviceService GraphicsDeviceService { get; set; }

        void Initialize();
        void LoadContent();
        void UnloadContent();
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime);
        void AfterRender();
        void OnActivated(object sender, EventArgs args);
        void OnDeactivated(object sender, EventArgs args);
        void OnExiting(object sender, EventArgs args);

        void SizeChanged(object sender, SizeChangedEventArgs args);
    }

    public class MonoGameViewModel : ViewModel, IMonoGameViewModel
    {
        public MonoGameViewModel()
        {
        }

        public void Dispose()
        {
            Content?.Dispose();
        }

        public IGraphicsDeviceService GraphicsDeviceService { get; set; } = default!;
        protected GraphicsDevice GraphicsDevice => GraphicsDeviceService?.GraphicsDevice!;
        protected MonoGameServiceProvider Services { get; private set; } = default!;
        protected ContentManager Content { get; set; } = default!;
        protected List<IGameComponent> Components { get; } = new();
        private readonly Dictionary<object, SpriteBatch> _spriteBatches = new();
        protected IEnumerable<SpriteBatch> SpriteBatches
            => _spriteBatches.Values;
        public SpriteBatch this[object id]
            => _spriteBatches[id];

        public virtual void Initialize()
        {
            Services = new MonoGameServiceProvider();
            Services.AddService(GraphicsDeviceService);
            Content = new ContentManager(Services) { RootDirectory = "Content" };
        }

        protected void PostInitialize()
        {
            foreach (var component in Components)
                component.Initialize();
        }

        public virtual void LoadContent() { }
        public virtual void UnloadContent() { }
        public virtual void Update(GameTime gameTime)
        {
            foreach (var component in Components)
                if (component is IUpdateable updateable && updateable.Enabled)
                    updateable.Update(gameTime);
        }


        public virtual bool BeginDraw()
        {
            foreach (var sb in _spriteBatches.Values)
                sb.Begin();
            return true;
        }

        public virtual void EndDraw()
        {
            foreach (var sb in _spriteBatches.Values)
                sb.End();
        }
        public virtual void Draw(GameTime gameTime) { }
        void IMonoGameViewModel.Draw(GameTime gameTime)
        {
            if (BeginDraw())
            {
                foreach (var component in Components)
                    if (component is IDrawable drawable && drawable.Visible)
                        drawable.Draw(gameTime);
                Draw(gameTime);
                EndDraw();
            }
        }
        public virtual void AfterRender() { }
        public virtual void OnActivated(object sender, EventArgs args) { }
        public virtual void OnDeactivated(object sender, EventArgs args) { }
        public virtual void OnExiting(object sender, EventArgs args) { }
        public virtual void SizeChanged(object sender, SizeChangedEventArgs args) { }
        protected SpriteBatch CreateSpriteBatch(object id)
            => _spriteBatches[id] = new(GraphicsDevice);
    }

    public class MonoGameViewModelEmbeded : MonoGameViewModel
    {
        private readonly Window _window;
        private readonly UIElement _embededElement;

        public MonoGameViewModelEmbeded(Window window, UIElement embededElement)
            => (_window, _embededElement) = (window, embededElement);

        public override bool BeginDraw()
        {
            var origin = _embededElement.TranslatePoint(new(0, 0), _window);
            GraphicsDevice.Viewport = new(((int)origin.X), ((int)origin.Y), ((int)_embededElement.RenderSize.Width), ((int)_embededElement.RenderSize.Height));
            return base.BeginDraw();
        }
    }
}
