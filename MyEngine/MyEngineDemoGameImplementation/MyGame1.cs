using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyEngine_Core;
using MyEngine_Core.ECS.MyComponents;
using MyEngine_Core.ECS.MyEntities;
using MyEngine_Core.ECS.MySystems;
using MyEngine_Core.MyGraphics;

namespace MyEngineDemoGameImplementation
{
    public class MyGame1 : MyProgram
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private TextureAtlas _textureAtlas;

        // Definir capas de colisión
        private const int PLAYER_LAYER = 0;
        private const int ENEMY_LAYER = 1;
        private const int ENVIRONMENT_LAYER = 2;

        public MyGame1() : base("Juego Top-Down", 1920, 1080, false)
        {
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            _textureAtlas = TextureAtlas.FromFile(Content, "Resources/Group1/atlas_buttons.xml");

            // ✅ ¡CRUCIAL! Configurar para modo top-down
            PhysicsSystem.SetTopDownMode();
            Debug.WriteLine("🎮 Configurado para modo Top-Down");

            // Crear entidades
            CrearJugador();
            CrearEnemigo();
            CrearParedesTopDown(); // Cambiar suelo por paredes
        }

        private void CrearJugador()
        {
            var jugador = World.CreateEntity();

            // Transform - Centro de pantalla
            jugador.AddComponent(new TransformComponent
            {
                Position = new Vector2(200), // Centro de 1920x1080
                Scale = new Vector2(1f),
                Rotation = 0f
            });

            // Sprites
            jugador.AddComponent<SpriteComponent>(_textureAtlas.CreateSprite("DOT"));
            jugador.GetComponent<SpriteComponent>().CenterOrigin();
            jugador.GetComponent<SpriteComponent>().Color = Color.MediumVioletRed;

            // ✅ FÍSICAS TOP-DOWN
            jugador.AddComponent<RigidbodyComponent>(new RigidbodyComponent()
            {
                Mass = 1.0f, // Masa para colisiones, pero sin gravedad
                Velocity = Vector2.Zero
            });

            // ✅ COLISIONES
            var playerCollider = new ColliderComponent(new Vector2(32, 32))
            {
                Layer = PLAYER_LAYER,
                CollisionMask = (1 << ENEMY_LAYER) | (1 << ENVIRONMENT_LAYER),
                Tag = "Player",
                IsEnabled = true
            };
            jugador.AddComponent(playerCollider);

            // ✅ CONTROLES TOP-DOWN FUNCIONALES
            var inputComp = new InputComponent(250f) // Velocidad de movimiento
            {
                UseKeyboard = true,
                UseGamepad = true,
                MoveUpKey = Keys.W,
                MoveDownKey = Keys.S,
                MoveLeftKey = Keys.A,
                MoveRightKey = Keys.D,
                JumpKey = Keys.Space, // Reutilizar como "acción"
            };

            // ✅ CALLBACK DE MOVIMIENTO CONTINUO
            inputComp.OnMove = MoverJugadorTopDown;
            inputComp.OnJump = AccionJugador;

            jugador.AddComponent(inputComp);

            // Animación
            jugador.AddComponent<AnimationComponent>(_textureAtlas.CreateAnimatedSprite("box"));
            jugador.GetComponent<AnimationComponent>().OnAnimationLoop += OnPlayerAnimationLoop;
            AnimationSystem.PlayAnimation(jugador, "box", true);

            Debug.WriteLine("🟦 Jugador top-down creado");
        }

        private void CrearEnemigo()
        {
            var enemigo = World.CreateEntity();

            enemigo.AddComponent(new TransformComponent
            {
                Position = new Vector2(600, 300),
                Scale = new Vector2(1f),
                Rotation = 0f
            });

            enemigo.AddComponent<SpriteComponent>(_textureAtlas.CreateSprite("DOT"));
            enemigo.GetComponent<SpriteComponent>().CenterOrigin();
            enemigo.GetComponent<SpriteComponent>().Color = Color.Red;

            // ✅ ENEMIGO TOP-DOWN - SIN GRAVEDAD
            enemigo.AddComponent<RigidbodyComponent>(new RigidbodyComponent()
            {
                Mass = 0.8f,
                Velocity = new Vector2(80, 50) // Movimiento inicial diagonal
            });

            var enemyCollider = new ColliderComponent(new Vector2(30, 30))
            {
                Layer = ENEMY_LAYER,
                CollisionMask = (1 << PLAYER_LAYER) | (1 << ENVIRONMENT_LAYER),
                Tag = "Enemy",
                IsEnabled = true
            };
            enemigo.AddComponent(enemyCollider);

            // Animación
            enemigo.AddComponent<AnimationComponent>(_textureAtlas.CreateAnimatedSprite("box"));
            enemigo.GetComponent<AnimationComponent>().OnAnimationLoop += OnEnemyAnimationLoop;
            AnimationSystem.PlayAnimation(enemigo, "box", true);

            Debug.WriteLine("🟥 Enemigo top-down creado");
        }

        private void CrearParedesTopDown()
        {
            var paredes = new[]
            {
                // Bordes de pantalla
                new { Pos = new Vector2(960, 30), Size = new Vector2(1920, 60), Color = Color.Gray },    // Superior
                new { Pos = new Vector2(960, 1050), Size = new Vector2(1920, 60), Color = Color.Gray },  // Inferior
                new { Pos = new Vector2(30, 540), Size = new Vector2(60, 1080), Color = Color.Gray },    // Izquierda
                new { Pos = new Vector2(1890, 540), Size = new Vector2(60, 1080), Color = Color.Gray },  // Derecha
                
                // Obstáculos internos
                new { Pos = new Vector2(400, 200), Size = new Vector2(80, 160), Color = Color.DarkGray },
                new { Pos = new Vector2(800, 400), Size = new Vector2(120, 80), Color = Color.DarkGray },
                new { Pos = new Vector2(1200, 600), Size = new Vector2(80, 200), Color = Color.DarkGray }
            };

            foreach (var paredData in paredes)
            {
                var pared = World.CreateEntity();

                pared.AddComponent(new TransformComponent
                {
                    Position = paredData.Pos,
                    Scale = new Vector2(1f)
                });

                pared.AddComponent<SpriteComponent>(_textureAtlas.CreateSprite("DOT"));
                pared.GetComponent<SpriteComponent>().CenterOrigin();
                pared.GetComponent<SpriteComponent>().Color = paredData.Color;

                // ✅ PARED ESTÁTICA
                pared.AddComponent<RigidbodyComponent>(new RigidbodyComponent()
                {
                    Mass = 0, // Inmóvil
                    Velocity = Vector2.Zero
                });

                var wallCollider = new ColliderComponent(paredData.Size)
                {
                    Layer = ENVIRONMENT_LAYER,
                    CollisionMask = 0,
                    Tag = "Wall",
                    IsEnabled = true
                };
                pared.AddComponent(wallCollider);
            }

            Debug.WriteLine("🟫 Paredes top-down creadas");
        }

        // ==================== CALLBACKS TOP-DOWN ====================

        private void MoverJugadorTopDown(EntidadPadre jugador, Vector2 direccion)
        {
            if (jugador.HasComponent<RigidbodyComponent>())
            {
                var rb = jugador.GetComponent<RigidbodyComponent>();
                var inputComp = jugador.GetComponent<InputComponent>();

                // ✅ MOVIMIENTO TOP-DOWN DIRECTO
                rb.Velocity = direccion * inputComp.MoveSpeed;

                // ✅ ROTACIÓN HACIA DIRECCIÓN DE MOVIMIENTO
                if (direccion.LengthSquared() > 0)
                {
                    var transform = jugador.GetComponent<TransformComponent>();
                    transform.Rotation = (float)Math.Atan2(direccion.Y, direccion.X);
                }

                if (direccion != Vector2.Zero)
                {
                    Debug.WriteLine($"🎮 Jugador moviéndose: {direccion}");
                }
            }
        }

        private void AccionJugador(EntidadPadre jugador)
        {
            Debug.WriteLine("⚡ Acción principal del jugador");

            // Ejemplo: Cambiar color temporalmente
            if (jugador.HasComponent<SpriteComponent>())
            {
                var sprite = jugador.GetComponent<SpriteComponent>();
                sprite.Color = sprite.Color == Color.Blue ? Color.Cyan : Color.Blue;
            }
        }

        private void OnPlayerAnimationLoop(EntidadPadre jugador)
        {
            // Animación loop
        }

        private void OnEnemyAnimationLoop(EntidadPadre enemigo)
        {
            // Animación loop enemigo
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // ✅ IA SIMPLE PARA ENEMIGOS
            ActualizarIAEnemigos(gameTime);

            // ✅ DEBUG CONTROLS
            var keyboard = Keyboard.GetState();

            // Alternar modos con teclas
            if (keyboard.IsKeyDown(Keys.F1))
            {
                PhysicsSystem.SetTopDownMode();
                Debug.WriteLine("🎮 Modo Top-Down");
            }

            if (keyboard.IsKeyDown(Keys.F2))
            {
                PhysicsSystem.SetPlatformerMode();
                Debug.WriteLine("🏃 Modo Plataformer");
            }
        }

        private void ActualizarIAEnemigos(GameTime gameTime)
        {
            var enemigos = World.GetAllEntities()
                .Where(e => e.HasComponent<ColliderComponent>() &&
                           e.GetComponent<ColliderComponent>().Tag == "Enemy");

            foreach (var enemigo in enemigos)
            {
                if (enemigo.HasComponent<RigidbodyComponent>())
                {
                    var rb = enemigo.GetComponent<RigidbodyComponent>();
                    var transform = enemigo.GetComponent<TransformComponent>();

                    // Rebotar en bordes de pantalla
                    if (transform.Position.X <= 100 || transform.Position.X >= 1820)
                    {
                        rb.Velocity = new Vector2(-rb.Velocity.X, rb.Velocity.Y);
                    }

                    if (transform.Position.Y <= 100 || transform.Position.Y >= 980)
                    {
                        rb.Velocity = new Vector2(rb.Velocity.X, -rb.Velocity.Y);
                    }

                    // Rotación hacia dirección de movimiento
                    if (rb.Velocity.LengthSquared() > 0)
                    {
                        transform.Rotation = (float)Math.Atan2(rb.Velocity.Y, rb.Velocity.X);
                    }
                }
            }
        }
    }
}