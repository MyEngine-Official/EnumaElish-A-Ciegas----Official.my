using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyEngine.Controllers;
using MyEngine.MyCore;
using MyEngine.MyCore.MySystems;
using MyEngine.MyScenes;

namespace MyEngine
{
    public class MyProgram : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Services
        private AudioController _audioController;
        private SceneManager _sceneManager;

        // Configuration
        private readonly string _title;
        private readonly int _width;
        private readonly int _height;
        private readonly bool _fullScreen;
        private readonly bool _allowUserResizing;
        private Scene _initialScene;
        public MyProgram(string title = "MyEngine Game", int width = 1280, int height = 720, 
            bool fullScreen = false, bool allowUserResizing = false, Scene initialScene = null)
        {
            _title = title;
            _width = width;
            _height = height;
            _fullScreen = fullScreen;
            _allowUserResizing = allowUserResizing;
            _initialScene = initialScene;

            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // Configure graphics
            _graphics.PreferredBackBufferWidth = _width;
            _graphics.PreferredBackBufferHeight = _height;
            _graphics.IsFullScreen = _fullScreen;
            _graphics.SynchronizeWithVerticalRetrace = true;

            // Set window properties
            Window.Title = _title;
            Window.AllowUserResizing = _allowUserResizing;
            
            if (_allowUserResizing)
            {
                Window.ClientSizeChanged += OnWindowResize;
            }

            // Target 60 FPS
            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 60.0);
        }

        protected override void Initialize()
        {
            // Initialize services
            _audioController = new AudioController();

            base.Initialize();
        }


        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Create scene manager
            _sceneManager = new SceneManager(GraphicsDevice, _spriteBatch, Content, _audioController);

            // Load initial scene
            if (_initialScene != null)
            {
                // Use provided initial scene
                _sceneManager.SwitchToScene(_initialScene);
            }
            else
            {
                // Create default example scene
                var defaultScene = new ExampleMainMenuScene();
                _sceneManager.SwitchToScene(defaultScene);
            }

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            // Global exit condition (can be overridden in scenes)
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed && 
                Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                Exit();

            // Update services
            _audioController.Update();
            
            // Update scene manager (handles current scene update)
            _sceneManager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Scene manager handles clearing and drawing
            _sceneManager.Draw(gameTime);

            base.Draw(gameTime);
        }

        /// <summary>
        /// Handles window resize events
        /// </summary>
        private void OnWindowResize(object sender, EventArgs e)
        {
            if (_graphics.GraphicsDevice.Viewport.Width > 0 && _graphics.GraphicsDevice.Viewport.Height > 0)
            {
                _sceneManager?.OnWindowResize(
                    _graphics.GraphicsDevice.Viewport.Width, 
                    _graphics.GraphicsDevice.Viewport.Height);
            }
        }

        /// <summary>
        /// Clean up resources
        /// </summary>
        protected override void UnloadContent()
        {
            _sceneManager?.Dispose();
            _audioController?.Dispose();
            base.UnloadContent();
        }

        /// <summary>
        /// Gets the scene manager for external access (e.g., for scene switching)
        /// </summary>
        public SceneManager SceneManager => _sceneManager;
    }
}
