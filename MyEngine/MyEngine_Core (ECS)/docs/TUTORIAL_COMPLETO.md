# Tutorial Paso a Paso - Motor de Juego EnumaElish

## Índice

1. [Configuración Inicial](#configuración-inicial)
2. [Tu Primer Juego con Motor ECS](#tu-primer-juego-con-motor-ecs)
3. [Trabajando con Componentes](#trabajando-con-componentes)
4. [Sistemas y Lógica de Juego](#sistemas-y-lógica-de-juego)
5. [Proyecto Completo: Plataformero Simple](#proyecto-completo-plataformero-simple)
6. [Motor Tradicional: Alternativa Simple](#motor-tradicional-alternativa-simple)
7. [Gestión de Recursos y Audio](#gestión-de-recursos-y-audio)
8. [Escenas y Transiciones](#escenas-y-transiciones)
9. [Tips y Mejores Prácticas](#tips-y-mejores-prácticas)

---

## Configuración Inicial

### Prerrequisitos
- .NET 8.0 SDK
- Visual Studio 2022 o VSCode
- Conocimiento básico de C#

### Estructura del Proyecto

```
MiJuego/
├── Content/
│   ├── Textures/
│   ├── Audio/
│   └── Content.mgcb
├── Game/
│   ├── Components/
│   ├── Systems/
│   ├── Scenes/
│   └── MyGame.cs
└── MiJuego.csproj
```

### Configuración del .csproj

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <OutputType>Exe</OutputType>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="MonoGame.Framework.DesktopGL" Version="3.8.*" />
  </ItemGroup>
  <!-- Referencia al motor ECS -->
  <ItemGroup>
    <ProjectReference Include="..\MyEngine_Core (ECS)\MyEngine_Core.csproj" />
  </ItemGroup>
</Project>
```

---

## Tu Primer Juego con Motor ECS

### Paso 1: Crear el Game Principal

```csharp
using MyEngine_Core;

namespace MiJuego
{
    public class MiJuego : MyProgram
    {
        public MiJuego() : base("Mi Primer Juego", 1024, 768, false)
        {
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            
            // Aquí crearemos nuestras entidades
            CrearJugador();
            CrearEnemigo();
        }

        private void CrearJugador()
        {
            var jugador = World.CreateEntity();
            
            // Componente de posición
            jugador.AddComponent(new TransformComponent
            {
                Position = new Vector2(100, 100),
                Scale = Vector2.One,
                Rotation = 0f
            });
            
            // Aquí agregaremos más componentes...
        }
        
        private void CrearEnemigo()
        {
            // Implementación del enemigo...
        }
    }

    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new MiJuego())
            {
                game.Run();
            }
        }
    }
}
```

### Paso 2: Agregar Sprites a las Entidades

```csharp
private void CrearJugador()
{
    var jugador = World.CreateEntity();
    
    // Transform Component
    jugador.AddComponent(new TransformComponent
    {
        Position = new Vector2(100, 400),
        Scale = Vector2.One,
        Rotation = 0f
    });
    
    // Cargar textura del jugador
    var texturaJugador = Content.Load<Texture2D>("player");
    var regionJugador = new TextureRegion(texturaJugador, 0, 0, 32, 32);
    
    // Sprite Component
    jugador.AddComponent(new SpriteComponent(regionJugador)
    {
        Color = Color.White,
        LayerDepth = 0.5f
    });
    
    // Physics Component
    jugador.AddComponent(new RigidbodyComponent
    {
        Mass = 1.0f,
        Velocity = Vector2.Zero
    });
    
    // Input Component para controles
    var inputComponent = new InputComponent
    {
        MoveSpeed = 200f,
        UseKeyboard = true
    };
    
    // Configurar callbacks de input
    inputComponent.OnJump = (entity) =>
    {
        var rigidbody = entity.GetComponent<RigidbodyComponent>();
        rigidbody.Velocity.Y = -400f; // Saltar
    };
    
    jugador.AddComponent(inputComponent);
    
    // Collider para detección de colisiones
    jugador.AddComponent(new ColliderComponent(32, 32)
    {
        Tag = "Player"
    });
}
```

---

## Trabajando con Componentes

### Componente Personalizado: Stats del Jugador

```csharp
using MyEngine_Core.ECS.MyComponents;

namespace MiJuego.Components
{
    public class PlayerStatsComponent
    {
        public int Health { get; set; } = 100;
        public int MaxHealth { get; set; } = 100;
        public int Score { get; set; } = 0;
        public int Lives { get; set; } = 3;
        
        public bool IsAlive => Health > 0 && Lives > 0;
        
        public void TakeDamage(int damage)
        {
            Health = Math.Max(0, Health - damage);
            if (Health <= 0 && Lives > 0)
            {
                Lives--;
                Health = MaxHealth; // Revivir con vida completa
            }
        }
        
        public void Heal(int amount)
        {
            Health = Math.Min(MaxHealth, Health + amount);
        }
        
        public void AddScore(int points)
        {
            Score += points;
        }
    }
}
```

### Componente de Enemigo

```csharp
namespace MiJuego.Components
{
    public class EnemyComponent
    {
        public float MoveSpeed { get; set; } = 50f;
        public Vector2 PatrolStart { get; set; }
        public Vector2 PatrolEnd { get; set; }
        public bool MovingToEnd { get; set; } = true;
        public int Damage { get; set; } = 10;
        public int ScoreValue { get; set; } = 100;
    }
}
```

### Usando Componentes Personalizados

```csharp
private void CrearJugador()
{
    var jugador = World.CreateEntity();
    
    // Componentes básicos
    jugador.AddComponent(new TransformComponent
    {
        Position = new Vector2(100, 400),
        Scale = Vector2.One
    });
    
    // Componente personalizado
    jugador.AddComponent(new PlayerStatsComponent
    {
        Health = 100,
        MaxHealth = 100,
        Lives = 3,
        Score = 0
    });
    
    // Otros componentes...
}
```

---

## Sistemas y Lógica de Juego

### Sistema Personalizado: Enemy AI

```csharp
using MyEngine_Core.ECS;
using MyEngine_Core.ECS.MySystems;
using MiJuego.Components;

namespace MiJuego.Systems
{
    public class EnemyAISystem : ISystem
    {
        private World _world;
        
        public void Initialize(World world)
        {
            _world = world;
        }
        
        public void Update(GameTime gameTime)
        {
            var enemies = _world.GetEntitiesWithComponents<TransformComponent, EnemyComponent>();
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            foreach (var enemy in enemies)
            {
                var transform = enemy.GetComponent<TransformComponent>();
                var enemyComp = enemy.GetComponent<EnemyComponent>();
                
                // Lógica de patrullaje
                Vector2 target = enemyComp.MovingToEnd ? enemyComp.PatrolEnd : enemyComp.PatrolStart;
                Vector2 direction = target - transform.Position;
                
                if (direction.LengthSquared() < 25f) // Llegó al destino
                {
                    enemyComp.MovingToEnd = !enemyComp.MovingToEnd;
                }
                else
                {
                    direction.Normalize();
                    transform.Position += direction * enemyComp.MoveSpeed * deltaTime;
                }
            }
        }
    }
}
```

### Sistema de Combate

```csharp
namespace MiJuego.Systems
{
    public class CombatSystem : ISystem
    {
        private World _world;
        
        public void Initialize(World world)
        {
            _world = world;
        }
        
        public void Update(GameTime gameTime)
        {
            var players = _world.GetEntitiesWithComponents<TransformComponent, PlayerStatsComponent, ColliderComponent>();
            var enemies = _world.GetEntitiesWithComponents<TransformComponent, EnemyComponent, ColliderComponent>();
            
            foreach (var player in players)
            {
                var playerTransform = player.GetComponent<TransformComponent>();
                var playerStats = player.GetComponent<PlayerStatsComponent>();
                var playerCollider = player.GetComponent<ColliderComponent>();
                
                foreach (var enemy in enemies)
                {
                    var enemyTransform = enemy.GetComponent<TransformComponent>();
                    var enemyComp = enemy.GetComponent<EnemyComponent>();
                    var enemyCollider = enemy.GetComponent<ColliderComponent>();
                    
                    // Verificar colisión
                    if (CheckCollision(playerTransform, playerCollider, enemyTransform, enemyCollider))
                    {
                        // Player toca enemigo
                        playerStats.TakeDamage(enemyComp.Damage);
                        
                        // Empujar al jugador
                        if (player.HasComponent<RigidbodyComponent>())
                        {
                            var rigidbody = player.GetComponent<RigidbodyComponent>();
                            Vector2 knockback = (playerTransform.Position - enemyTransform.Position);
                            knockback.Normalize();
                            rigidbody.Velocity += knockback * 300f;
                        }
                    }
                }
            }
        }
        
        private bool CheckCollision(TransformComponent t1, ColliderComponent c1, 
                                   TransformComponent t2, ColliderComponent c2)
        {
            Rectangle bounds1 = new Rectangle(
                (int)(t1.Position.X + c1.Offset.X),
                (int)(t1.Position.Y + c1.Offset.Y),
                (int)(c1.Size.X * t1.Scale.X),
                (int)(c1.Size.Y * t1.Scale.Y)
            );
            
            Rectangle bounds2 = new Rectangle(
                (int)(t2.Position.X + c2.Offset.X),
                (int)(t2.Position.Y + c2.Offset.Y),
                (int)(c2.Size.X * t2.Scale.X),
                (int)(c2.Size.Y * t2.Scale.Y)
            );
            
            return bounds1.Intersects(bounds2);
        }
    }
}
```

### Registrar Sistemas Personalizados

```csharp
protected override void LoadContent()
{
    base.LoadContent();
    
    // Crear y registrar sistemas personalizados
    var enemyAI = new EnemyAISystem();
    var combatSystem = new CombatSystem();
    
    World.RegisterSystem(enemyAI);
    World.RegisterSystem(combatSystem);
    
    enemyAI.Initialize(World);
    combatSystem.Initialize(World);
    
    // Crear entidades
    CrearJugador();
    CrearEnemigos();
}

protected override void Update(GameTime gameTime)
{
    base.Update(gameTime); // Actualiza sistemas base
    
    // Actualizar nuestros sistemas personalizados
    World.GetSystem<EnemyAISystem>().Update(gameTime);
    World.GetSystem<CombatSystem>().Update(gameTime);
}
```

---

## Proyecto Completo: Plataformero Simple

### Estructura Completa del Juego

```csharp
using MyEngine_Core;
using MyEngine_Core.ECS.MyComponents;
using MyEngine_Core.MyGraphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiJuego.Components;
using MiJuego.Systems;

namespace MiJuego
{
    public class PlataformeroGame : MyProgram
    {
        private EnemyAISystem _enemyAI;
        private CombatSystem _combatSystem;
        private Texture2D _playerTexture;
        private Texture2D _enemyTexture;
        private Texture2D _platformTexture;
        
        public PlataformeroGame() : base("Plataformero Simple", 1024, 768, false)
        {
        }
        
        protected override void LoadContent()
        {
            base.LoadContent();
            
            // Cargar texturas
            _playerTexture = Content.Load<Texture2D>("player");
            _enemyTexture = Content.Load<Texture2D>("enemy");
            _platformTexture = Content.Load<Texture2D>("platform");
            
            // Configurar sistemas
            ConfigurarSistemas();
            
            // Crear mundo del juego
            CrearMundo();
        }
        
        private void ConfigurarSistemas()
        {
            // Sistemas personalizados
            _enemyAI = new EnemyAISystem();
            _combatSystem = new CombatSystem();
            
            World.RegisterSystem(_enemyAI);
            World.RegisterSystem(_combatSystem);
            
            _enemyAI.Initialize(World);
            _combatSystem.Initialize(World);
            
            // Configurar física
            PhysicsSystem.Gravity = new Vector2(0, 800f); // Gravedad hacia abajo
        }
        
        private void CrearMundo()
        {
            CrearJugador();
            CrearPlataformas();
            CrearEnemigos();
        }
        
        private void CrearJugador()
        {
            var jugador = World.CreateEntity();
            
            // Transform
            jugador.AddComponent(new TransformComponent
            {
                Position = new Vector2(100, 400),
                Scale = Vector2.One
            });
            
            // Sprite
            var playerRegion = new TextureRegion(_playerTexture, 0, 0, 32, 32);
            jugador.AddComponent(new SpriteComponent(playerRegion)
            {
                LayerDepth = 0.5f
            });
            
            // Physics
            jugador.AddComponent(new RigidbodyComponent
            {
                Mass = 1.0f,
                Velocity = Vector2.Zero
            });
            
            // Collider
            jugador.AddComponent(new ColliderComponent(30, 30)
            {
                Tag = "Player",
                Offset = new Vector2(1, 1)
            });
            
            // Input
            var inputComponent = new InputComponent
            {
                MoveSpeed = 250f,
                UseKeyboard = true
            };
            
            // Configurar salto
            inputComponent.OnJump = (entity) =>
            {
                var rb = entity.GetComponent<RigidbodyComponent>();
                if (Math.Abs(rb.Velocity.Y) < 10f) // Solo saltar si está en el suelo
                {
                    rb.Velocity.Y = -400f;
                }
            };
            
            jugador.AddComponent(inputComponent);
            
            // Stats del jugador
            jugador.AddComponent(new PlayerStatsComponent
            {
                Health = 100,
                MaxHealth = 100,
                Lives = 3,
                Score = 0
            });
        }
        
        private void CrearPlataformas()
        {
            // Plataforma del suelo
            CrearPlataforma(new Vector2(0, 700), new Vector2(1024, 68));
            
            // Plataformas flotantes
            CrearPlataforma(new Vector2(200, 600), new Vector2(200, 32));
            CrearPlataforma(new Vector2(500, 500), new Vector2(200, 32));
            CrearPlataforma(new Vector2(300, 350), new Vector2(150, 32));
        }
        
        private void CrearPlataforma(Vector2 position, Vector2 size)
        {
            var plataforma = World.CreateEntity();
            
            plataforma.AddComponent(new TransformComponent
            {
                Position = position,
                Scale = new Vector2(size.X / _platformTexture.Width, size.Y / _platformTexture.Height)
            });
            
            var platformRegion = new TextureRegion(_platformTexture, 0, 0, _platformTexture.Width, _platformTexture.Height);
            plataforma.AddComponent(new SpriteComponent(platformRegion)
            {
                LayerDepth = 0.8f,
                Color = Color.Brown
            });
            
            plataforma.AddComponent(new ColliderComponent(size.X, size.Y)
            {
                Tag = "Platform"
            });
        }
        
        private void CrearEnemigos()
        {
            // Enemigo en plataforma flotante
            CrearEnemigo(new Vector2(200, 550), new Vector2(200, 400), new Vector2(380, 550));
            CrearEnemigo(new Vector2(500, 450), new Vector2(500, 450), new Vector2(680, 450));
        }
        
        private void CrearEnemigo(Vector2 position, Vector2 patrolStart, Vector2 patrolEnd)
        {
            var enemigo = World.CreateEntity();
            
            enemigo.AddComponent(new TransformComponent
            {
                Position = position,
                Scale = Vector2.One
            });
            
            var enemyRegion = new TextureRegion(_enemyTexture, 0, 0, 32, 32);
            enemigo.AddComponent(new SpriteComponent(enemyRegion)
            {
                LayerDepth = 0.4f,
                Color = Color.Red
            });
            
            enemigo.AddComponent(new ColliderComponent(30, 30)
            {
                Tag = "Enemy"
            });
            
            enemigo.AddComponent(new EnemyComponent
            {
                MoveSpeed = 50f,
                PatrolStart = patrolStart,
                PatrolEnd = patrolEnd,
                MovingToEnd = true,
                Damage = 20,
                ScoreValue = 100
            });
        }
        
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime); // Sistemas base (Input, Physics, Animation, Audio)
            
            // Nuestros sistemas personalizados
            _enemyAI.Update(gameTime);
            _combatSystem.Update(gameTime);
        }
    }
}
```

### Content Pipeline

Crear un archivo `Content.mgcb` en la carpeta Content:

```
#begin player.png
/importer:TextureImporter
/processor:TextureProcessor
/build:player.png

#begin enemy.png
/importer:TextureImporter  
/processor:TextureProcessor
/build:enemy.png

#begin platform.png
/importer:TextureImporter
/processor:TextureProcessor
/build:platform.png
```

---

## Motor Tradicional: Alternativa Simple

### Juego Simple con Motor Tradicional

```csharp
using MonoGameLibrary;
using MonoGameLibrary.Entities;
using MonoGameLibrary.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiJuegoSimple
{
    public class JuegoSimple : Core
    {
        private Player _player;
        private List<Enemy> _enemies;
        
        public JuegoSimple() : base("Juego Simple", 1024, 768, false)
        {
            _enemies = new List<Enemy>();
        }
        
        protected override void LoadContent()
        {
            base.LoadContent();
            
            // Crear jugador
            var playerTexture = Content.Load<Texture2D>("player");
            var playerRegion = new TextureRegion(playerTexture, 0, 0, 32, 32);
            var playerSprite = new Sprite(playerRegion);
            
            _player = new Player(playerSprite);
            _player.Position(new Vector2(100, 400));
            
            // Crear enemigos
            CrearEnemigos();
        }
        
        private void CrearEnemigos()
        {
            var enemyTexture = Content.Load<Texture2D>("enemy");
            var enemyRegion = new TextureRegion(enemyTexture, 0, 0, 32, 32);
            
            for (int i = 0; i < 3; i++)
            {
                var enemySprite = new Sprite(enemyRegion);
                var enemy = new Enemy(enemySprite);
                enemy.Position(new Vector2(300 + i * 150, 400));
                _enemies.Add(enemy);
            }
        }
        
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            // Actualizar jugador
            _player.Update(gameTime);
            
            // Actualizar enemigos
            foreach (var enemy in _enemies)
            {
                enemy.Update(gameTime);
            }
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            SpriteBatch.Begin();
            
            // Dibujar jugador
            _player._sprite.Draw(SpriteBatch, _player.Position());
            
            // Dibujar enemigos
            foreach (var enemy in _enemies)
            {
                enemy._sprite.Draw(SpriteBatch, enemy.Position());
            }
            
            SpriteBatch.End();
            
            base.Draw(gameTime);
        }
    }
    
    public class Player : GameObject
    {
        private float _speed = 200f;
        
        public Player(Sprite sprite) : base(sprite) { }
        
        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            if (Core.Input.Keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left))
                MoveLeft(_speed * deltaTime);
            if (Core.Input.Keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right))
                MoveRight(_speed * deltaTime);
            if (Core.Input.Keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up))
                MoveUp(_speed * deltaTime);
            if (Core.Input.Keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down))
                MoveDown(_speed * deltaTime);
        }
    }
    
    public class Enemy : GameObject
    {
        private float _speed = 50f;
        private float _direction = 1f;
        private Vector2 _startPosition;
        private float _patrolDistance = 100f;
        
        public Enemy(Sprite sprite) : base(sprite) 
        {
            _startPosition = Position();
        }
        
        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            // Movimiento de patrullaje
            MoveRight(_direction * _speed * deltaTime);
            
            // Cambiar dirección cuando alcance los límites
            if (Math.Abs(Position().X - _startPosition.X) > _patrolDistance)
            {
                _direction *= -1;
            }
        }
    }
    
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new JuegoSimple())
            {
                game.Run();
            }
        }
    }
}
```

---

## Gestión de Recursos y Audio

### Usando ResourceManager

```csharp
// En LoadContent()
protected override void LoadContent()
{
    base.LoadContent();
    
    // Configurar ResourceManager
    ResourceManager.SetContentManager(Content);
    
    // Cargar recursos con cache
    var playerTexture = ResourceManager.LoadTexture("player");
    var jumpSound = ResourceManager.LoadSoundEffect("jump");
    var backgroundMusic = ResourceManager.LoadSong("background_music");
    
    // Los recursos se cachean automáticamente
    CrearJugadorConRecursos();
}

private void CrearJugadorConRecursos()
{
    var jugador = World.CreateEntity();
    
    // Usar recursos cacheados
    var playerTexture = ResourceManager.LoadTexture("player");
    var playerRegion = new TextureRegion(playerTexture, 0, 0, 32, 32);
    
    jugador.AddComponent(new SpriteComponent(playerRegion));
    
    // Configurar audio en input
    var inputComponent = new InputComponent();
    inputComponent.OnJump = (entity) =>
    {
        // Reproducir sonido de salto
        var jumpSound = ResourceManager.LoadSoundEffect("jump");
        AudioController.PlaySoundEffect(jumpSound);
        
        // Lógica de salto
        var rb = entity.GetComponent<RigidbodyComponent>();
        rb.Velocity.Y = -400f;
    };
    
    jugador.AddComponent(inputComponent);
}

// Reproducir música de fondo
private void IniciarAudio()
{
    var backgroundMusic = ResourceManager.LoadSong("background_music");
    AudioController.PlaySong(backgroundMusic, true);
}
```

### Sistema de Audio Avanzado

```csharp
namespace MiJuego.Systems
{
    public class AudioSystem : ISystem
    {
        private World _world;
        private Dictionary<string, SoundEffect> _soundEffects;
        
        public void Initialize(World world)
        {
            _world = world;
            _soundEffects = new Dictionary<string, SoundEffect>();
            
            // Precargar efectos de sonido comunes
            _soundEffects["jump"] = ResourceManager.LoadSoundEffect("jump");
            _soundEffects["hit"] = ResourceManager.LoadSoundEffect("hit");
            _soundEffects["coin"] = ResourceManager.LoadSoundEffect("coin");
        }
        
        public void PlaySound(string soundName, float volume = 1.0f)
        {
            if (_soundEffects.ContainsKey(soundName))
            {
                AudioController.PlaySoundEffect(_soundEffects[soundName], volume, 0f, 0f, false);
            }
        }
        
        public void PlaySoundAtPosition(string soundName, Vector2 soundPosition, Vector2 listenerPosition)
        {
            // Calcular volumen y paneo basado en posición
            float distance = Vector2.Distance(soundPosition, listenerPosition);
            float maxDistance = 500f;
            float volume = Math.Max(0f, 1f - (distance / maxDistance));
            
            float pan = MathHelper.Clamp((soundPosition.X - listenerPosition.X) / 200f, -1f, 1f);
            
            if (_soundEffects.ContainsKey(soundName) && volume > 0.01f)
            {
                AudioController.PlaySoundEffect(_soundEffects[soundName], volume, 0f, pan, false);
            }
        }
    }
}
```

---

## Escenas y Transiciones

### Crear una Escena Personalizada

```csharp
using MyEngine_Core.MyScenes;
using Microsoft.Xna.Framework;

namespace MiJuego.Scenes
{
    public class MenuPrincipal : Scene
    {
        private List<Button> _botones;
        
        public MenuPrincipal() : base("MenuPrincipal")
        {
            BackgroundColor = Color.DarkBlue;
            _botones = new List<Button>();
        }
        
        public override void Initialize()
        {
            // Inicialización de la escena
        }
        
        public override void LoadContent()
        {
            // Cargar recursos específicos del menú
            var buttonTexture = Resources.LoadTexture("button");
            var font = Resources.LoadFont("menu_font");
            
            CrearBotones();
        }
        
        private void CrearBotones()
        {
            // Crear botón "Jugar"
            var botonJugar = World.CreateEntity();
            
            botonJugar.AddComponent(new TransformComponent
            {
                Position = new Vector2(400, 300),
                Scale = Vector2.One
            });
            
            var buttonTexture = Resources.LoadTexture("button");
            var buttonRegion = new TextureRegion(buttonTexture, 0, 0, 200, 50);
            
            botonJugar.AddComponent(new SpriteComponent(buttonRegion));
            
            // Componente de botón (personalizado)
            botonJugar.AddComponent(new ButtonComponent
            {
                OnClick = () => 
                {
                    // Cambiar a escena del juego
                    var gameScene = new GameScene();
                    // SceneManager.SwitchToScene(gameScene, TransitionType.Fade);
                }
            });
        }
        
        public override void Update(GameTime gameTime)
        {
            // Lógica del menú
        }
        
        public override void Draw(GameTime gameTime)
        {
            // El RenderSystem se encarga del dibujado automáticamente
        }
        
        public override void UnloadContent()
        {
            _botones.Clear();
            base.UnloadContent();
        }
    }
}
```

### Componente de Botón Personalizado

```csharp
using Microsoft.Xna.Framework;

namespace MiJuego.Components
{
    public class ButtonComponent
    {
        public Action OnClick { get; set; }
        public Action OnHover { get; set; }
        public bool IsHovered { get; set; }
        public bool IsPressed { get; set; }
        public Color NormalColor { get; set; } = Color.White;
        public Color HoverColor { get; set; } = Color.LightGray;
        public Color PressedColor { get; set; } = Color.Gray;
    }
}
```

### Sistema de UI

```csharp
namespace MiJuego.Systems
{
    public class UISystem : ISystem
    {
        private World _world;
        
        public void Initialize(World world)
        {
            _world = world;
        }
        
        public void Update(GameTime gameTime)
        {
            var buttons = _world.GetEntitiesWithComponents<TransformComponent, SpriteComponent, ButtonComponent>();
            
            // Obtener posición del mouse (necesitarías implementar esto en InputSystem)
            Vector2 mousePosition = GetMousePosition();
            bool mouseClicked = WasMouseJustClicked();
            
            foreach (var button in buttons)
            {
                var transform = button.GetComponent<TransformComponent>();
                var sprite = button.GetComponent<SpriteComponent>();
                var buttonComp = button.GetComponent<ButtonComponent>();
                
                // Verificar si el mouse está sobre el botón
                Rectangle buttonBounds = new Rectangle(
                    (int)transform.Position.X,
                    (int)transform.Position.Y,
                    sprite.Region.Width,
                    sprite.Region.Height
                );
                
                bool wasHovered = buttonComp.IsHovered;
                buttonComp.IsHovered = buttonBounds.Contains(mousePosition);
                
                // Cambiar color basado en estado
                if (buttonComp.IsPressed)
                {
                    sprite.Color = buttonComp.PressedColor;
                }
                else if (buttonComp.IsHovered)
                {
                    sprite.Color = buttonComp.HoverColor;
                    if (!wasHovered)
                    {
                        buttonComp.OnHover?.Invoke();
                    }
                }
                else
                {
                    sprite.Color = buttonComp.NormalColor;
                }
                
                // Manejar click
                if (buttonComp.IsHovered && mouseClicked)
                {
                    buttonComp.OnClick?.Invoke();
                }
            }
        }
        
        private Vector2 GetMousePosition()
        {
            // Implementar obtención de posición del mouse
            return Vector2.Zero;
        }
        
        private bool WasMouseJustClicked()
        {
            // Implementar detección de click
            return false;
        }
    }
}
```

---

## Tips y Mejores Prácticas

### 1. Organización de Componentes

```csharp
// ✅ Bueno: Componentes como datos puros
public class HealthComponent
{
    public int Current { get; set; }
    public int Maximum { get; set; }
    public bool IsDead => Current <= 0;
}

// ❌ Malo: Lógica en componentes
public class BadHealthComponent
{
    public int Health { get; set; }
    
    // No hagas esto - la lógica va en sistemas
    public void TakeDamage(int amount)
    {
        Health -= amount;
        if (Health <= 0)
        {
            // Lógica de muerte
        }
    }
}
```

### 2. Sistemas Eficientes

```csharp
// ✅ Bueno: Cache las consultas caras
public class MovementSystem : ISystem
{
    private World _world;
    private List<EntidadPadre> _movableEntities;
    
    public void Initialize(World world)
    {
        _world = world;
        // Cache inicial
        RefreshMovableEntities();
    }
    
    public void Update(GameTime gameTime)
    {
        // Usar cache en lugar de consultar cada frame
        foreach (var entity in _movableEntities)
        {
            // Lógica de movimiento
        }
    }
    
    public void RefreshMovableEntities()
    {
        _movableEntities = _world.GetEntitiesWithComponents<TransformComponent, VelocityComponent>();
    }
}
```

### 3. Manejo de Recursos

```csharp
// ✅ Bueno: Carga recursos una vez
public class GameAssets
{
    public static Dictionary<string, Texture2D> Textures { get; private set; }
    public static Dictionary<string, SoundEffect> Sounds { get; private set; }
    
    public static void LoadAll(ContentManager content)
    {
        Textures = new Dictionary<string, Texture2D>
        {
            ["player"] = content.Load<Texture2D>("player"),
            ["enemy"] = content.Load<Texture2D>("enemy"),
            ["bullet"] = content.Load<Texture2D>("bullet")
        };
        
        Sounds = new Dictionary<string, SoundEffect>
        {
            ["shoot"] = content.Load<SoundEffect>("shoot"),
            ["hit"] = content.Load<SoundEffect>("hit")
        };
    }
}
```

### 4. Debugging de Entidades

```csharp
// Sistema de debug útil
public class DebugSystem : ISystem
{
    private World _world;
    
    public void Initialize(World world)
    {
        _world = world;
    }
    
    public void DrawDebugInfo(SpriteBatch spriteBatch, SpriteFont font)
    {
        var entities = _world.GetAllEntities();
        int yOffset = 10;
        
        spriteBatch.DrawString(font, $"Entities: {entities.Count}", new Vector2(10, yOffset), Color.White);
        yOffset += 20;
        
        foreach (var entity in entities.Take(10)) // Solo primeras 10
        {
            string components = string.Join(", ", GetComponentNames(entity));
            spriteBatch.DrawString(font, $"Entity {entity.Id}: {components}", 
                                  new Vector2(10, yOffset), Color.Yellow);
            yOffset += 15;
        }
    }
    
    private List<string> GetComponentNames(EntidadPadre entity)
    {
        var names = new List<string>();
        
        if (entity.HasComponent<TransformComponent>()) names.Add("Transform");
        if (entity.HasComponent<SpriteComponent>()) names.Add("Sprite");
        if (entity.HasComponent<RigidbodyComponent>()) names.Add("Rigidbody");
        // ... agregar más según necesidad
        
        return names;
    }
}
```

### 5. Patrón de Factory para Entidades

```csharp
public static class EntityFactory
{
    public static EntidadPadre CreatePlayer(World world, Vector2 position)
    {
        var player = world.CreateEntity();
        
        player.AddComponent(new TransformComponent { Position = position });
        player.AddComponent(new SpriteComponent(GameAssets.GetPlayerSprite()));
        player.AddComponent(new RigidbodyComponent { Mass = 1.0f });
        player.AddComponent(new ColliderComponent(32, 32) { Tag = "Player" });
        player.AddComponent(new InputComponent { MoveSpeed = 200f });
        player.AddComponent(new PlayerStatsComponent());
        
        return player;
    }
    
    public static EntidadPadre CreateEnemy(World world, Vector2 position, EnemyType type)
    {
        var enemy = world.CreateEntity();
        
        enemy.AddComponent(new TransformComponent { Position = position });
        enemy.AddComponent(new SpriteComponent(GameAssets.GetEnemySprite(type)));
        enemy.AddComponent(new ColliderComponent(32, 32) { Tag = "Enemy" });
        
        switch (type)
        {
            case EnemyType.Patrol:
                enemy.AddComponent(new EnemyComponent { MoveSpeed = 50f });
                break;
            case EnemyType.Chase:
                enemy.AddComponent(new ChaseComponent { ChaseSpeed = 100f });
                break;
        }
        
        return enemy;
    }
}

public enum EnemyType
{
    Patrol,
    Chase,
    Static
}
```

### Conclusión del Tutorial

Este tutorial te ha guiado a través de:

1. **Configuración básica** de ambos motores
2. **Creación de entidades y componentes** personalizados
3. **Implementación de sistemas** complejos
4. **Desarrollo de un juego completo** paso a paso
5. **Gestión de recursos y audio**
6. **Sistemas de escenas y UI**
7. **Mejores prácticas y patrones**

El motor ECS ofrece mayor flexibilidad y escalabilidad, mientras que el motor tradicional es más directo para proyectos simples. Elige el que mejor se adapte a tu proyecto y experiencia.

**¡Ya estás listo para crear increíbles juegos 2D!** 🎮