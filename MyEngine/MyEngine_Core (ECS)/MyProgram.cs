using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyEngine_Core.ECS;
using MyEngine_Core.ECS.MySystems;
using MyEngine_Core.ECS.MyComponents;
using MyEngine_Core.MyAudio;
using MyEngine_Core.MyServices;
using MyEngine_Core.MyGraphics;
using System;

namespace MyEngine_Core
{
    public class MyProgram : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // ECS Core
        private World _world;

        // Systems
        private RenderSystem _renderSystem;
        private AnimationSystem _animationSystem;
        private PhysicsSystem _physicsSystem;
        private InputSystem _inputSystem;

        // Services
        private ResourceManager _resourceManager;
        private AudioController _audioController;

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
            _world = new World();

            // Initialize services
            _resourceManager = new ResourceManager();
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

            // Initialize systems with world
            _world.RegisterSystem(_renderSystem);
            _world.RegisterSystem(_animationSystem);
            _world.RegisterSystem(_physicsSystem);
            _world.RegisterSystem(_inputSystem);

            _renderSystem.Initialize(_world);
            _animationSystem.Initialize(_world);
            _physicsSystem.Initialize(_world);
            _inputSystem.Initialize(_world);

            // Create sample entities for testing
            //CreateSampleEntities();
        }

        /// <summary>
        /// Creates sample entities for testing the ECS system
        /// </summary>
        private void CreateSampleEntities()
        {
            // Example: Create a player entity
            var player = _world.CreateEntity();

            // Add transform component
            player.AddComponent(new TransformComponent
            {
                Position = new Vector2(_width / 2, _height / 2),
                Scale = Vector2.One,
                Rotation = 0f
            });

            // Add sprite component (you'll need to load a texture)
            // Texture2D playerTexture = Content.Load<Texture2D>("player");
            // player.AddComponent(new SpriteComponent(new TextureRegion(playerTexture, 0, 0, 32, 32)));

            // Add physics component
            player.AddComponent(new RigidbodyComponent
            {
                Mass = 1.0f,
                Velocity = Vector2.Zero
            });

            // Add collider component
            player.AddComponent(new ColliderComponent(32, 32)
            {
                Tag = "Player",
            });

            // Add input component
            var inputComponent = new InputComponent
            {
                MoveSpeed = 300f,
                UseKeyboard = true,
                UseGamepad = true
            };

            // Setup input callbacks
            inputComponent.OnJump = (entity) =>
            {
                if (entity.HasComponent<RigidbodyComponent>())
                {
                    var rb = entity.GetComponent<RigidbodyComponent>();
                    rb.Velocity.Y = -500f; // Jump velocity
                }
            };

            player.AddComponent(inputComponent);

            // Add animation component
            // var animComponent = new AnimationComponent();
            // Setup animations here...
            // player.AddComponent(animComponent);
        }

        protected override void Update(GameTime gameTime)
        {
            // Exit on Escape key
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Update systems in order
            _inputSystem.Update(gameTime);
            _physicsSystem.Update(gameTime);
            _animationSystem.Update(gameTime);
            _audioController.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Draw all entities
            _renderSystem.Draw(gameTime);

            base.Draw(gameTime);
        }

        protected override void UnloadContent()
        {
            // Clean up resources
            _audioController?.Dispose();
            _world?.Clear();

            base.UnloadContent();
        }

        /// <summary>
        /// Gets the ECS world instance
        /// </summary>
        public World World => _world;

        /// <summary>
        /// Gets the render system
        /// </summary>
        public RenderSystem RenderSystem => _renderSystem;

        /// <summary>
        /// Gets the physics system
        /// </summary>
        public PhysicsSystem PhysicsSystem => _physicsSystem;

        /// <summary>
        /// Gets the input system
        /// </summary>
        public InputSystem InputSystem => _inputSystem;

        /// <summary>
        /// Gets the animation system
        /// </summary>
        public AnimationSystem AnimationSystem => _animationSystem;

        /// <summary>
        /// Gets the audio controller
        /// </summary>
        public AudioController AudioController => _audioController;

        /// <summary>
        /// Gets the resource manager
        /// </summary>
        public ResourceManager ResourceManager => _resourceManager;
    }

    /// <summary>
    /// Program entry point
    /// </summary>
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new MyProgram("My ECS Game", 1280, 720, false))
            {
                game.Run();
            }
        }
    }
}