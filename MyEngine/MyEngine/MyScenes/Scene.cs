using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MyEngine.Controllers;
using MyEngine.MyCore;
using MyEngine.MyCore.MySystems;

namespace MyEngine.MyScenes
{
    /// <summary>
    /// Base class for all game scenes
    /// </summary>
    public abstract class Scene : IDisposable
    {
        /// <summary>
        /// The ECS world for this scene
        /// </summary>
        protected WorldManager World { get; private set; }
        
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
        protected AudioController Audio { get; private set; }
        
        /// <summary>
        /// Systems for this scene
        /// </summary>
        protected RenderSystem RenderSystem { get; private set; }
        protected AnimationSystem AnimationSystem { get; private set; }
        protected PhysicsSystem PhysicsSystem { get; private set; }
        protected InputSystem InputSystem { get; private set; }
        protected ButtonSystem ButtonSystem { get; private set; }
        
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
            World = new WorldManager();
        }

        /// <summary>
        /// Initializes the scene with required services
        /// </summary>
        internal void InternalInitialize(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, 
            ContentManager content, AudioController audio)
        {
            GraphicsDevice = graphicsDevice;
            SpriteBatch = spriteBatch;
            Content = content;
            Audio = audio;
            
            // Create and register default systems
            InitializeSystems();
            
            // Call scene-specific initialization
            Initialize();
            LoadContent();
            
            IsInitialized = true;
        }
        
        /// <summary>
        /// Initializes the default systems for this scene
        /// </summary>
        protected virtual void InitializeSystems()
        {
            // Create systems
            RenderSystem = new RenderSystem(GraphicsDevice, SpriteBatch);
            AnimationSystem = new AnimationSystem();
            PhysicsSystem = new PhysicsSystem();
            InputSystem = new InputSystem();
            ButtonSystem = new ButtonSystem();
            
            // Register systems with world
            World.RegisterSystem<RenderSystem>(RenderSystem);
            World.RegisterSystem<AnimationSystem>(AnimationSystem);
            World.RegisterSystem<PhysicsSystem>(PhysicsSystem);
            World.RegisterSystem<InputSystem>(InputSystem);
            World.RegisterSystem<ButtonSystem>(ButtonSystem);
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
        public virtual void Update(GameTime gameTime)
        {
            // Update all systems through WorldManager
            World.UpdateSystems(gameTime);
            
            // Call scene-specific update
            OnUpdate(gameTime);
        }

        /// <summary>
        /// Draw the scene
        /// </summary>
        public virtual void Draw(GameTime gameTime)
        {
            // Use RenderSystem to draw all entities
            RenderSystem?.Draw(gameTime);
            
            // Call scene-specific draw for UI or overlays
            OnDraw(gameTime);
        }
        
        /// <summary>
        /// Scene-specific update logic (called after systems update)
        /// </summary>
        protected virtual void OnUpdate(GameTime gameTime) { }
        
        /// <summary>
        /// Scene-specific draw logic (called after render system)
        /// </summary>
        protected virtual void OnDraw(GameTime gameTime) { }

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
