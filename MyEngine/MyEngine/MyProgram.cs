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

        // ECS Core
        private WorldManager _world;

        // Systems
        private RenderSystem _renderSystem;
        private AnimationSystem _animationSystem;
        private PhysicsSystem _physicsSystem;
        private InputSystem _inputSystem;
        private ButtonSystem _buttonSystem;

        // Services
        private AudioController _audioController;
        private SceneManager _sceneManager;

        // Configuration
        private readonly string _title;
        private readonly int _width;
        private readonly int _height;
        private readonly bool _fullScreen;
        public MyProgram(string title = "MyEngine Game", int width = 1280, int height = 720, bool fullScreen = false)
        {
            _title = title;
            _width = width;
            _height = height;
            _fullScreen = fullScreen;

            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // Configure graphics
            _graphics.PreferredBackBufferWidth = _width;
            _graphics.PreferredBackBufferHeight = _height;
            _graphics.IsFullScreen = _fullScreen;
            _graphics.SynchronizeWithVerticalRetrace = true;

            // Set window title
            Window.Title = _title;

            // Target 60 FPS
            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 60.0);
        }

        protected override void Initialize()
        {
            // Create the ECS world
            _world = new WorldManager();

            // Initialize services
            _audioController = new AudioController();

            base.Initialize();
        }


        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Create and register systems
            _renderSystem = new RenderSystem(GraphicsDevice, _spriteBatch);
            _animationSystem = new AnimationSystem();
            _physicsSystem = new PhysicsSystem();
            _inputSystem = new InputSystem();
            _buttonSystem = new ButtonSystem();

            // Initialize systems with world
            _world.RegisterSystem<RenderSystem>(_renderSystem);
            _world.RegisterSystem<AnimationSystem>(_animationSystem);
            _world.RegisterSystem<PhysicsSystem>(_physicsSystem);
            _world.RegisterSystem<InputSystem>(_inputSystem);
            _world.RegisterSystem<ButtonSystem>(_buttonSystem);

            _renderSystem.Initialize(_world);
            _animationSystem.Initialize(_world);
            _physicsSystem.Initialize(_world);
            _inputSystem.Initialize(_world);
            _buttonSystem.Initialize(_world);

            _sceneManager = new SceneManager(GraphicsDevice, _spriteBatch, Content, _audioController);

            base.LoadContent();
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _audioController.Update();
            _inputSystem.Update(gameTime);
            _buttonSystem.Update(gameTime);
            _physicsSystem.Update(gameTime);
            _animationSystem.Update(gameTime);
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _renderSystem.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
