using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MyEngine_Core.ECS;

namespace MyEngine_Core.MyScenes
{
    /// <summary>
    /// Base class for all game scenes
    /// </summary>
    public abstract class Scene : IDisposable
    {
        /// <summary>
        /// The ECS world for this scene
        /// </summary>
        protected World World { get; private set; }
        
        /// <summary>
        /// Content manager for this scene
        /// </summary>
        protected ContentManager Content { get; private set; }
        
        /// <summary>
        /// Graphics device reference
        /// </summary>
        protected GraphicsDevice GraphicsDevice { get; private set; }
        
        /// <summary>
        /// SpriteBatch for rendering
        /// </summary>
        protected SpriteBatch SpriteBatch { get; private set; }
        
        /// <summary>
        /// Scene-specific resource manager
        /// </summary>
        protected MyServices.ResourceManager Resources { get; private set; }
        
        /// <summary>
        /// Is this scene currently active
        /// </summary>
        public bool IsActive { get; internal set; }
        
        /// <summary>
        /// Has this scene been initialized
        /// </summary>
        public bool IsInitialized { get; private set; }
        
        /// <summary>
        /// Scene name for identification
        /// </summary>
        public string Name { get; protected set; }
        
        /// <summary>
        /// Background color for this scene
        /// </summary>
        public Color BackgroundColor { get; set; } = Color.CornflowerBlue;

        /// <summary>
        /// Creates a new scene
        /// </summary>
        protected Scene(string name = null)
        {
            Name = name ?? GetType().Name;
            World = new World();
        }

        /// <summary>
        /// Initializes the scene with required services
        /// </summary>
        internal void InternalInitialize(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, 
            ContentManager content, MyServices.ResourceManager resources)
        {
            GraphicsDevice = graphicsDevice;
            SpriteBatch = spriteBatch;
            Content = content;
            Resources = resources;
            
            Initialize();
            LoadContent();
            
            IsInitialized = true;
        }

        /// <summary>
        /// Initialize scene logic (called once when scene is created)
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Load content for this scene
        /// </summary>
        public abstract void LoadContent();

        /// <summary>
        /// Update scene logic
        /// </summary>
        public abstract void Update(GameTime gameTime);

        /// <summary>
        /// Draw the scene
        /// </summary>
        public abstract void Draw(GameTime gameTime);

        /// <summary>
        /// Called when scene is being deactivated
        /// </summary>
        public virtual void OnDeactivate()
        {
            IsActive = false;
        }

        /// <summary>
        /// Called when scene is being activated
        /// </summary>
        public virtual void OnActivate()
        {
            IsActive = true;
        }

        /// <summary>
        /// Unload content when scene is disposed
        /// </summary>
        public virtual void UnloadContent()
        {
            World?.Clear();
        }

        /// <summary>
        /// Handle input for this scene
        /// </summary>
        public virtual void HandleInput(GameTime gameTime)
        {
        }

        /// <summary>
        /// Called when window size changes
        /// </summary>
        public virtual void OnWindowResize(int width, int height)
        {
        }

        #region IDisposable

        private bool _isDisposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    UnloadContent();
                    World?.Clear();
                    Content?.Unload();
                }
                _isDisposed = true;
            }
        }

        #endregion
    }
}
