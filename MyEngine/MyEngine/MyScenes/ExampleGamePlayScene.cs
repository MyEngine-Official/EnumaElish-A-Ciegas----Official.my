using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyEngine.MyCore.MyComponents;
using MyEngine.MyCore.MyEntities;
using MyEngine.MyCore.Events;
using MyEngine.MyGraphics;

namespace MyEngine.MyScenes
{
    /// <summary>
    /// Example gameplay scene demonstrating ECS usage
    /// </summary>
    public class ExampleGamePlayScene : Scene
    {
        private MainEntity _player;
        private MainEntity _enemy;
        private MainEntity _ground;
        private Texture2D _whitePixel;
        private Texture2D _playerTexture;
        private Texture2D _enemyTexture;
        private float _gameTime = 0f;

        public ExampleGamePlayScene() : base("GamePlay")
        {
            BackgroundColor = new Color(135, 206, 235); // Sky blue
        }

        public override void Initialize()
        {
            // Create textures for demo
            _whitePixel = new Texture2D(GraphicsDevice, 1, 1);
            _whitePixel.SetData(new[] { Color.White });

            // Create player texture (32x32 green square)
            _playerTexture = new Texture2D(GraphicsDevice, 32, 32);
            var playerData = new Color[32 * 32];
            for (int i = 0; i < playerData.Length; i++)
                playerData[i] = Color.Green;
            _playerTexture.SetData(playerData);

            // Create enemy texture (32x32 red square)
            _enemyTexture = new Texture2D(GraphicsDevice, 32, 32);
            var enemyData = new Color[32 * 32];
            for (int i = 0; i < enemyData.Length; i++)
                enemyData[i] = Color.Red;
            _enemyTexture.SetData(enemyData);

            // Create player entity
            CreatePlayer();

            // Create enemy entity
            CreateEnemy();

            // Create ground
            CreateGround();

            // Subscribe to events
            World.Events.Subscribe<CollisionEnterEvent>(OnCollision);
            World.Events.Subscribe<EntityDestroyedEvent>(OnEntityDestroyed);
        }

        private void CreatePlayer()
        {
            _player = World.CreateEntity("Player");

            // Position and physics
            _player.AddComponent(new TransformComponent 
            { 
                Position = new Vector2(100, 300),
                Scale = Vector2.One
            });

            _player.AddComponent(new RigidbodyComponent 
            { 
                Mass = 1.0f,
                Velocity = new Vector2(100, 0), // Move speed
                UseGravity = true,
                GravityScale = 500f
            });

            // Collision
            _player.AddComponent(new ColliderComponent(new Vector2(32, 32)) 
            { 
                Tag = "Player",
                IsTrigger = false
            });

            // Graphics
            var playerSprite = new SpriteComponent(new TextureRegion(_playerTexture))
            {
                Color = Color.White,
                LayerDepth = 0.5f
            };
            _player.AddComponent(playerSprite);

            // Animation (example with single frame)
            var animComponent = new AnimationComponent();
            animComponent.AddAnimation(
                AnimationAction.IdleRight, 
                new Animation(new[] { new TextureRegion(_playerTexture) }, TimeSpan.FromSeconds(1)));
            animComponent.AddAnimation(
                AnimationAction.RunRight,
                new Animation(new[] { new TextureRegion(_playerTexture) }, TimeSpan.FromMilliseconds(100)));
            animComponent.SetAnimation(AnimationAction.IdleRight);
            _player.AddComponent(animComponent);

            // Input
            var inputComponent = new InputComponent();
            inputComponent.UseKeyboard = true;
            inputComponent.AssignKeyBinding(InputAction.MoveLeft, Keys.A);
            inputComponent.AssignKeyBinding(InputAction.MoveRight, Keys.D);
            inputComponent.AssignKeyBinding(InputAction.MoveUp, Keys.W);
            inputComponent.AssignKeyBinding(InputAction.MoveDown, Keys.S);
            inputComponent.AssignKeyBinding(InputAction.Jump, Keys.Space);
            _player.AddComponent(inputComponent);

            // Life
            _player.AddComponent(new LifeComponent(100, 100));
        }

        private void CreateEnemy()
        {
            _enemy = World.CreateEntity("Enemy");

            _enemy.AddComponent(new TransformComponent 
            { 
                Position = new Vector2(500, 300),
                Scale = Vector2.One
            });

            _enemy.AddComponent(new RigidbodyComponent 
            { 
                Mass = 1.0f,
                Velocity = new Vector2(-50, 0), // Move towards player
                UseGravity = true,
                GravityScale = 500f
            });

            _enemy.AddComponent(new ColliderComponent(new Vector2(32, 32)) 
            { 
                Tag = "Enemy",
                IsTrigger = false
            });

            var enemySprite = new SpriteComponent(new TextureRegion(_enemyTexture))
            {
                Color = Color.White,
                LayerDepth = 0.5f
            };
            _enemy.AddComponent(enemySprite);

            _enemy.AddComponent(new LifeComponent(50, 50));
        }

        private void CreateGround()
        {
            _ground = World.CreateEntity("Ground");

            var groundY = GraphicsDevice.Viewport.Height - 100;
            _ground.AddComponent(new TransformComponent 
            { 
                Position = new Vector2(GraphicsDevice.Viewport.Width / 2, groundY),
                Scale = new Vector2(GraphicsDevice.Viewport.Width, 100)
            });

            // Ground is static (no rigidbody needed for static objects in this simple example)
            _ground.AddComponent(new ColliderComponent(new Vector2(GraphicsDevice.Viewport.Width, 100)) 
            { 
                Tag = "Ground",
                IsTrigger = false,
                IsStatic = true
            });

            var groundSprite = new SpriteComponent(new TextureRegion(_whitePixel))
            {
                Color = new Color(101, 67, 33), // Brown
                Scale = new Vector2(GraphicsDevice.Viewport.Width, 100),
                LayerDepth = 0.1f
            };
            _ground.AddComponent(groundSprite);
        }

        public override void LoadContent()
        {
            // Load any content files here if needed
            // For this example, we're creating textures procedurally
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            _gameTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Check for pause
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                // Return to menu
                World.Events.Publish(new SceneChangeRequestedEvent 
                { 
                    NewSceneName = "MainMenu",
                    TransitionType = "Fade"
                });
            }

            // Simple enemy AI - move towards player
            if (_enemy != null && _player != null)
            {
                var playerPos = _player.GetComponent<TransformComponent>().Position;
                var enemyPos = _enemy.GetComponent<TransformComponent>().Position;
                var enemyRb = _enemy.GetComponent<RigidbodyComponent>();

                var direction = playerPos.X - enemyPos.X;
                if (Math.Abs(direction) > 5) // Dead zone
                {
                    enemyRb.Velocity = new Vector2(Math.Sign(direction) * 50, enemyRb.Velocity.Y);
                }
            }

            // Set camera to follow player
            if (_player != null && RenderSystem?.Camera != null)
            {
                var playerPos = _player.GetComponent<TransformComponent>().Position;
                RenderSystem.Camera.Position = playerPos - new Vector2(
                    GraphicsDevice.Viewport.Width / 2,
                    GraphicsDevice.Viewport.Height / 2);
            }
        }

        protected override void OnDraw(GameTime gameTime)
        {
            // Draw UI overlay
            SpriteBatch.Begin();

            // Draw HUD
            DrawHUD();

            SpriteBatch.End();
        }

        private void DrawHUD()
        {
            // Draw health bar for player
            if (_player != null && _player.HasComponent<LifeComponent>())
            {
                var life = _player.GetComponent<LifeComponent>();
                var healthPercent = life.CurrentHealth / (float)life.MaxHealth;
                
                // Background
                SpriteBatch.Draw(_whitePixel, 
                    new Rectangle(10, 10, 200, 20), 
                    Color.DarkGray);
                
                // Health
                SpriteBatch.Draw(_whitePixel, 
                    new Rectangle(12, 12, (int)(196 * healthPercent), 16), 
                    Color.Green);
            }

            // Draw game info
            DrawText($"Time: {_gameTime:F1}", new Vector2(10, 40), Color.White);
            DrawText("Press ESC to return to menu", new Vector2(10, 60), Color.Gray);
            DrawText("Use WASD to move, SPACE to jump", new Vector2(10, 80), Color.Gray);
        }

        private void DrawText(string text, Vector2 position, Color color)
        {
            // Simple text rendering with rectangles (when no font is available)
            foreach (char c in text)
            {
                if (c != ' ')
                {
                    SpriteBatch.Draw(_whitePixel, 
                        new Rectangle((int)position.X, (int)position.Y, 6, 10), 
                        color * 0.5f);
                }
                position.X += 8;
            }
        }

        private void OnCollision(CollisionEnterEvent e)
        {
            // Handle collision between player and enemy
            var entity1 = World.GetAllEntities().Find(ent => ent.Id == e.Entity1Id);
            var entity2 = World.GetAllEntities().Find(ent => ent.Id == e.Entity2Id);

            if (entity1 == null || entity2 == null) return;

            var tag1 = entity1.GetComponent<ColliderComponent>()?.Tag;
            var tag2 = entity2.GetComponent<ColliderComponent>()?.Tag;

            // Player hits enemy
            if ((tag1 == "Player" && tag2 == "Enemy") || (tag1 == "Enemy" && tag2 == "Player"))
            {
                var player = tag1 == "Player" ? entity1 : entity2;
                var enemy = tag1 == "Enemy" ? entity1 : entity2;

                // Damage enemy
                if (enemy.HasComponent<LifeComponent>())
                {
                    var enemyLife = enemy.GetComponent<LifeComponent>();
                    enemyLife.TakeDamage(25);
                    
                    // Knockback
                    if (enemy.HasComponent<RigidbodyComponent>())
                    {
                        var rb = enemy.GetComponent<RigidbodyComponent>();
                        rb.AddForce(new Vector2(200 * Math.Sign(rb.Velocity.X), -100));
                    }

                    // Destroy if dead
                    if (enemyLife.CurrentHealth <= 0)
                    {
                        World.RemoveEntity(enemy);
                        _enemy = null; // Clear reference
                    }
                }
            }
        }

        private void OnEntityDestroyed(EntityDestroyedEvent e)
        {
            if (e.EntityType == "Enemy")
            {
                // Spawn a new enemy after 2 seconds
                // In a real game, you'd use a timer system
                Console.WriteLine("Enemy destroyed! Score +100");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _whitePixel?.Dispose();
                _playerTexture?.Dispose();
                _enemyTexture?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
