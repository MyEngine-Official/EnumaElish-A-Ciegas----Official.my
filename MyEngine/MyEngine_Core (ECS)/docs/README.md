# Documentación Completa del Motor de Juego EnumaElish

## Índice

1. [Introducción](#introducción)
2. [Arquitectura General](#arquitectura-general)
3. [Motor ECS (MyEngine_Core)](#motor-ecs-myengine_core)
4. [Motor Tradicional (MonoGameLibrary)](#motor-tradicional-monogamelibrary)
5. [Comparación de Motores](#comparación-de-motores)
6. [Estructura de Directorios](#estructura-de-directorios)
7. [Casos de Uso Recomendados](#casos-de-uso-recomendados)
8. [Integración y Consideraciones](#integración-y-consideraciones)

## Introducción

El proyecto EnumaElish cuenta con **dos motores de juego diferentes** implementados en C# con MonoGame Framework:

1. **MyEngine_Core (ECS)**: Un motor moderno basado en Entity Component System
2. **MonoGameLibrary**: Un motor tradicional basado en objetos

Ambos motores están diseñados para desarrollo de juegos 2D y ofrecen diferentes enfoques arquitectónicos según las necesidades del proyecto.

## Arquitectura General

### Tecnologías Base
- **Framework**: .NET 8.0
- **Gráficos**: MonoGame Framework DesktopGL 3.8
- **Plataforma**: Multiplataforma (Windows, Linux, macOS)

### Principios de Diseño
- **Modularidad**: Componentes independientes y reutilizables
- **Extensibilidad**: Fácil agregación de nuevas funcionalidades
- **Performance**: Optimización para juegos en tiempo real
- **Mantenibilidad**: Código limpio y bien documentado

## Motor ECS (MyEngine_Core)

### Filosofía del ECS

El motor ECS sigue el patrón **Entity-Component-System**, donde:
- **Entidades**: Contenedores únicos con ID
- **Componentes**: Datos puros sin lógica
- **Sistemas**: Lógica que opera sobre componentes

### Componentes Principales

#### 1. Clase World (world_manager.cs)
```csharp
public class World
```

**Responsabilidad**: Administrador central de todas las entidades y sistemas.

**Funcionalidades**:
- Creación y destrucción de entidades
- Registro de sistemas
- Consultas optimizadas de entidades por componentes
- Reciclado de IDs para optimización de memoria

**Métodos Clave**:
- `CreateEntity()`: Crea nueva entidad con ID único
- `GetEntitiesWithComponents<T1>()`: Consulta entidades con componente específico
- `RegisterSystem<T>()`: Registra sistema en el mundo

#### 2. Clase EntidadPadre (EntidadPadre.cs)
```csharp
public class EntidadPadre
```

**Responsabilidad**: Contenedor genérico para componentes.

**Estructura**:
- ID único inmutable
- Diccionario de componentes por tipo
- Métodos para gestión de componentes

**API Principal**:
- `AddComponent<T>(T component)`: Agrega componente
- `GetComponent<T>()`: Obtiene componente
- `HasComponent<T>()`: Verifica existencia de componente

### Componentes Disponibles

#### TransformComponent
```csharp
public class TransformComponent
{
    public Vector2 Position { get; set; }
    public float Rotation { get; set; }
    public Vector2 Scale { get; set; } = Vector2.One;
}
```
**Uso**: Posición, rotación y escala de entidades en el mundo 2D.

#### SpriteComponent
```csharp
public class SpriteComponent
{
    public TextureRegion Region { get; set; }
    public Color Color { get; set; } = Color.White;
    public Vector2 Origin { get; set; } = Vector2.Zero;
    public SpriteEffects Effects { get; set; } = SpriteEffects.None;
    public float LayerDepth { get; set; } = 0.0f;
}
```
**Uso**: Renderizado de sprites 2D con propiedades visuales.

#### RigidbodyComponent
```csharp
public class RigidbodyComponent
{
    public Vector2 Velocity;
    public float Mass;
}
```
**Uso**: Física básica con velocidad y masa.

#### ColliderComponent
```csharp
public class ColliderComponent
{
    public Vector2 Size { get; set; }
    public Vector2 Offset { get; set; }
    public bool IsTrigger { get; set; }
    public string Tag { get; set; }
    public int Layer { get; set; }
    public int CollisionMask { get; set; }
    public bool IsEnabled { get; set; }
}
```
**Uso**: Detección de colisiones con sistema de capas y tags.

#### InputComponent
```csharp
public class InputComponent
{
    public bool IsEnabled { get; set; } = true;
    public bool UseKeyboard { get; set; } = true;
    public bool UseGamepad { get; set; } = true;
    public bool UseMouse { get; set; } = false;
    public float MoveSpeed { get; set; } = 200f;
    // Callbacks para eventos
    public Action<EntidadPadre> OnJump { get; set; }
    public Action<EntidadPadre> OnAction { get; set; }
}
```
**Uso**: Manejo de input con callbacks personalizables.

#### AnimationComponent
```csharp
public class AnimationComponent
{
    public Dictionary<string, Animation> Animations { get; set; }
    public Animation CurrentAnimation { get; set; }
    public int CurrentFrame { get; set; }
    public bool IsPlaying { get; set; }
    public bool IsLooping { get; set; }
}
```
**Uso**: Sistema de animaciones con múltiples clips y eventos.

#### TilemapComponent
```csharp
public class TilemapComponent
{
    public int Rows { get; }
    public int Columns { get; }
    public Vector2 Scale { get; set; }
    // Métodos para manejo de tiles
    public void SetTile(int column, int row, int tilesetID);
    public TextureRegion GetTile(int column, int row);
}
```
**Uso**: Renderizado de mapas de tiles para backgrounds y niveles.

### Sistemas del Motor

#### RenderSystem
**Responsabilidad**: Renderizado de todas las entidades visuales.

**Funcionalidades**:
- Renderizado ordenado por LayerDepth
- Soporte para cámaras 2D
- Renderizado de sprites y tilemaps
- Batching optimizado con SpriteBatch

**Pipeline de Renderizado**:
1. Obtiene entidades con Transform + (Sprite o Tilemap)
2. Ordena por profundidad
3. Configura SpriteBatch con matriz de cámara
4. Renderiza tilemaps primero (background)
5. Renderiza sprites por orden de profundidad

#### PhysicsSystem
**Responsabilidad**: Simulación física y manejo de colisiones.

**Características**:
- Gravedad configurable
- Damping de velocidad
- Detección AABB (Axis-Aligned Bounding Box)
- Resolución de colisiones con separación
- Sistema de capas de colisión

**Flujo de Física**:
1. Aplica gravedad a entidades con masa
2. Aplica damping a velocidades
3. Actualiza posiciones basado en velocidad
4. Detecta y resuelve colisiones
5. Separa objetos colisionando

#### InputSystem
**Responsabilidad**: Procesamiento de entrada de usuario.

**Capacidades**:
- Teclado, ratón y gamepad
- Estados anterior y actual para detección de pulsaciones
- Callbacks configurables por entidad
- Soporte para múltiples gamepads
- Zonas muertas para analógicos

**Procesamiento**:
1. Actualiza estados de dispositivos
2. Procesa entidades con InputComponent
3. Detecta pulsaciones y movimientos
4. Ejecuta callbacks correspondientes
5. Aplica movimiento a Transform/Rigidbody

#### AnimationSystem
**Responsabilidad**: Actualización de animaciones.

**Características**:
- Múltiples animaciones por entidad
- Control de reproducción (play/pause/stop)
- Looping configurable
- Eventos de finalización y loop
- Cambio automático de TextureRegion en sprites

### Servicios del Motor

#### ResourceManager
**Responsabilidad**: Gestión centralizada de recursos.

**Capacidades**:
- Cache inteligente de recursos
- Carga bajo demanda
- Estadísticas de uso
- Liberación de memoria
- Soporte para múltiples tipos de recursos

**Tipos Soportados**:
- Texture2D
- SoundEffect
- Song
- SpriteFont
- Effect (Shaders)

#### AudioController
**Responsabilidad**: Control de audio del juego.

**Funcionalidades**:
- Reproducción de efectos de sonido
- Música de fondo
- Control de volumen global
- Pausa/resume de todo el audio
- Gestión automática de instancias

#### SceneManager
**Responsabilidad**: Gestión de escenas y transiciones.

**Características**:
- Registro de escenas por nombre
- Transiciones con efectos visuales
- Liberación automática de memoria
- Estados de escena (activa/inactiva)
- Redimensionado de ventana

### Clases Gráficas

#### Camara2D
**Responsabilidad**: Control de viewport y transformaciones.

**Propiedades**:
```csharp
public Vector2 Position { get; set; }
public Vector2 Scale { get; set; } = Vector2.One;
public float Rotation { get; set; }
public Vector2 Origin { get; set; }
public Rectangle? Bounds { get; set; }
```

**Funciones**:
- `GetTransform()`: Matriz de transformación
- `CenterOn(Vector2 position)`: Centra en posición
- `ApplyBounds()`: Aplica límites de movimiento

#### TextureRegion
**Responsabilidad**: Región específica de una textura.

**Uso**: Base para sprites, animaciones y tiles.

#### Animation
**Responsabilidad**: Secuencia de TextureRegions para animación.

**Propiedades**:
- `List<TextureRegion> Frames`: Frames de la animación
- `TimeSpan Delay`: Tiempo entre frames

#### Tileset y TilemapComponent
**Responsabilidad**: Sistema completo de tiles.

**Características**:
- Carga desde archivos XML
- Indexación 0-based
- Escalado independiente
- Tiles vacíos (valor -1)

### Programa Principal (MyProgram.cs)

La clase `MyProgram` hereda de `Game` y actúa como:
- **Punto de entrada** de la aplicación
- **Configurador** de sistemas
- **Coordinador** del game loop

**Secuencia de Inicialización**:
1. Configura GraphicsDevice
2. Crea World ECS
3. Inicializa servicios (ResourceManager, AudioController)
4. Registra sistemas en orden específico
5. Crea entidades de ejemplo

**Game Loop**:
- **Update**: Actualiza sistemas en orden (Input → Physics → Animation → Audio)
- **Draw**: Ejecuta RenderSystem

## Motor Tradicional (MonoGameLibrary)

### Filosofía Orientada a Objetos

El motor tradicional sigue un enfoque clásico donde cada GameObject contiene toda su lógica y datos.

### Componentes Principales

#### Core (Singleton)
```csharp
public class Core : Game
{
    public static Core Instance { get; }
    public static GraphicsDeviceManager Graphics { get; private set; }
    public static SpriteBatch SpriteBatch { get; private set; }
    public static InputManager Input { get; private set; }
    public static AudioController Audio { get; private set; }
}
```

**Responsabilidad**: Núcleo central del motor con acceso global.

**Características**:
- Patrón Singleton estricto
- Referencias estáticas globales
- Manejo de escenas
- Configuración de ventana

#### GameObject
```csharp
public class GameObject
{
    public Sprite _sprite { get; set; }
    public AnimatedSprite _animatedSprite { get; set; }
    public Vector2 position = new(1);
    public StaticOrAnimate type { get; set; } = StaticOrAnimate.Sprite;
}
```

**Responsabilidad**: Entidad de juego con componentes integrados.

**Métodos de Movimiento**:
- `MoveRight(float speed)`
- `MoveLeft(float speed)`
- `MoveUp(float speed)`
- `MoveDown(float speed)`

### Sistemas del Motor Tradicional

#### InputManager
**Estructura**:
```csharp
public class InputManager
{
    public KeyboardInfo Keyboard { get; private set; }
    public MouseInfo Mouse { get; private set; }
    public GamePadInfo[] GamePads { get; private set; }
}
```

**Características**:
- Encapsulación de estados de input
- Información detallada por dispositivo
- Actualización centralizada

#### AudioController
Similar al motor ECS, con misma funcionalidad de:
- Control de efectos de sonido
- Música de fondo
- Volúmenes globales
- Pausa/resume

#### Graphics System
**Sprite Class**:
```csharp
public class Sprite
{
    public TextureRegion Region { get; set; }
    public Color Color { get; set; } = Color.White;
    public float Rotation { get; set; } = 0.0f;
    public Vector2 Scale { get; set; } = Vector2.One;
    public Vector2 Origin { get; set; } = Vector2.Zero;
}
```

**Funcionalidades**:
- Renderizado directo con SpriteBatch
- Propiedades visuales integradas
- Métodos de utilidad (CenterOrigin)

### Scene System
Simple sistema de escenas con:
- Cambio de escena via `Core.ChangeScene()`
- Limpieza automática de memoria
- Estados de activación/desactivación

## Comparación de Motores

### Motor ECS vs Motor Tradicional

| Aspecto | Motor ECS | Motor Tradicional |
|---------|-----------|------------------|
| **Arquitectura** | Entity-Component-System | Orientado a Objetos |
| **Escalabilidad** | ⭐⭐⭐⭐⭐ Excelente | ⭐⭐⭐ Buena |
| **Performance** | ⭐⭐⭐⭐⭐ Optimizado | ⭐⭐⭐ Estándar |
| **Complejidad** | ⭐⭐⭐⭐ Alta | ⭐⭐ Baja |
| **Flexibilidad** | ⭐⭐⭐⭐⭐ Máxima | ⭐⭐⭐ Limitada |
| **Curva de Aprendizaje** | ⭐⭐⭐⭐ Empinada | ⭐⭐ Suave |
| **Mantenimiento** | ⭐⭐⭐⭐⭐ Excelente | ⭐⭐⭐ Bueno |

### Cuándo Usar Cada Motor

#### Motor ECS - Recomendado para:
- **Juegos complejos** con muchas entidades
- **Proyectos grandes** con equipos múltiples
- **Sistemas modulares** que requieren alta reutilización
- **Juegos con física avanzada** o muchos sistemas interactuando
- **Desarrollo a largo plazo** con expansiones futuras

#### Motor Tradicional - Recomendado para:
- **Prototipos rápidos** y game jams
- **Juegos pequeños** a medianos
- **Desarrolladores principiantes** aprendiendo conceptos
- **Proyectos con timeline ajustado**
- **Juegos simples** sin sistemas complejos

## Estructura de Directorios

```
EnumaElish/
├── MyEngine/
│   └── MyEngine_Core (ECS)/
│       ├── ECS/
│       │   ├── MyComponents/     # Componentes del sistema
│       │   ├── MyEntities/       # Entidades base
│       │   ├── MySystems/        # Sistemas ECS
│       │   └── world_manager.cs  # Administrador del mundo
│       ├── MyGraphics/           # Sistema gráfico
│       ├── MyAudio/              # Sistema de audio
│       ├── MyScenes/             # Manejo de escenas
│       ├── MyServices/           # Servicios del motor
│       └── MyProgram.cs          # Punto de entrada
├── GamePlatform/
│   └── GameClient/
│       └── EEGame.Logic/
│           └── GameEngine/       # Motor tradicional
│               ├── Entities/     # GameObjects
│               ├── Graphics/     # Sistema gráfico
│               ├── Input/        # Sistema de entrada
│               ├── Audio/        # Sistema de audio
│               ├── Scenes/       # Escenas simples
│               └── Core.cs       # Núcleo central
└── docs/                         # Documentación
```

## Casos de Uso Recomendados

### Escenarios Ideales para Motor ECS

1. **RPG con inventario complejo**:
   ```csharp
   // Entidad jugador con múltiples componentes
   var player = world.CreateEntity();
   player.AddComponent(new TransformComponent());
   player.AddComponent(new SpriteComponent());
   player.AddComponent(new StatsComponent());
   player.AddComponent(new InventoryComponent());
   player.AddComponent(new QuestComponent());
   ```

2. **Shooter con muchos proyectiles**:
   ```csharp
   // Sistema optimizado para cientos de balas
   var bullets = world.GetEntitiesWithComponents<TransformComponent, ProjectileComponent>();
   foreach (var bullet in bullets) {
       // Lógica de proyectil
   }
   ```

3. **Juego de estrategia con unidades múltiples**:
   ```csharp
   // Unidades con comportamientos modulares
   var unit = world.CreateEntity();
   unit.AddComponent(new TransformComponent());
   unit.AddComponent(new AIComponent());
   unit.AddComponent(new MovementComponent());
   unit.AddComponent(new CombatComponent());
   ```

### Escenarios Ideales para Motor Tradicional

1. **Juego de plataformas simple**:
   ```csharp
   public class Player : GameObject
   {
       public void Update(GameTime gameTime)
       {
           if (Core.Input.Keyboard.IsKeyDown(Keys.Right))
               MoveRight(200 * deltaTime);
       }
   }
   ```

2. **Puzzle game básico**:
   ```csharp
   public class Tile : GameObject
   {
       public bool IsActive { get; set; }
       public void Toggle() => IsActive = !IsActive;
   }
   ```

## Integración y Consideraciones

### Consideraciones Técnicas

#### Performance
- **Motor ECS**: Cache-friendly, procesamiento por lotes
- **Motor Tradicional**: Directo, menos overhead inicial

#### Memoria
- **Motor ECS**: Fragmentación mínima, pooling de entidades
- **Motor Tradicional**: Gestión manual, posible fragmentación

#### Debugging
- **Motor ECS**: Sistemas independientes, fácil debugging de lógica específica
- **Motor Tradicional**: Stack traces más directos

### Recomendaciones de Integración

1. **No mezclar motores** en el mismo proyecto
2. **Elegir motor antes del desarrollo** basado en requisitos
3. **Considerar migración** de tradicional a ECS para proyectos en crecimiento
4. **Usar motor ECS** para aprender patrones de arquitectura moderna
5. **Motor tradicional** para teaching y prototipos rápidos

### Futuras Mejoras Recomendadas

#### Motor ECS
1. **Sistema de eventos** para comunicación entre sistemas
2. **Archetype-based storage** para mejor performance
3. **Editor visual** para creación de entidades
4. **Profiler integrado** para análisis de performance
5. **Serialización** de mundos y entidades

#### Motor Tradicional
1. **Component pattern** básico para modularidad
2. **Pool de objetos** para GameObjects frecuentes
3. **Estado stack** para escenas complejas
4. **Sistema de tweening** integrado
5. **Debug renderer** para desarrollo

### Conclusión

Ambos motores representan enfoques válidos para el desarrollo de juegos 2D. La elección depende de:
- **Complejidad del proyecto**
- **Experiencia del equipo**
- **Requisitos de performance**
- **Timeline de desarrollo**
- **Planes futuros del proyecto**

El motor ECS ofrece mayor flexibilidad y escalabilidad a cambio de complejidad inicial, mientras que el motor tradicional proporciona simplicidad y rapidez de desarrollo para proyectos menores.
