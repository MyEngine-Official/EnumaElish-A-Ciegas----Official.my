# MyEngine - Sistema de Escenas Integrado

## 📋 Resumen de la Integración

El motor ahora tiene un sistema de escenas completamente integrado donde:

1. **MyProgram** es el punto de entrada que delega todo el control a **SceneManager**
2. Cada **Scene** tiene su propio **WorldManager** y sistemas ECS independientes
3. Las escenas están completamente aisladas entre sí
4. El flujo de actualización y renderizado está unificado

## 🏗️ Arquitectura Actualizada

```
MyProgram (Game)
    ├── SceneManager
    │   ├── CurrentScene
    │   │   ├── WorldManager (ECS)
    │   │   ├── RenderSystem
    │   │   ├── AnimationSystem
    │   │   ├── PhysicsSystem
    │   │   ├── InputSystem
    │   │   └── ButtonSystem
    │   └── Scene Registry
    └── AudioController (Global)
```

## 🚀 Uso Básico

### Ejemplo 1: Inicio Rápido

```csharp
using MyEngine;

class Program
{
    static void Main()
    {
        // El motor iniciará con la escena de menú por defecto
        using (var game = new MyProgram("Mi Juego", 1280, 720))
        {
            game.Run();
        }
    }
}
```

### Ejemplo 2: Escena Inicial Personalizada

```csharp
using MyEngine;
using MyEngine.MyScenes;

class Program
{
    static void Main()
    {
        // Crear tu escena inicial
        var miEscena = new MiEscenaPersonalizada();
        
        using (var game = new MyProgram(
            title: "Mi Juego",
            width: 1280,
            height: 720,
            initialScene: miEscena))
        {
            game.Run();
        }
    }
}
```

## 📄 Crear una Escena Personalizada

```csharp
using MyEngine.MyScenes;
using MyEngine.MyCore.MyComponents;
using Microsoft.Xna.Framework;

public class MiEscenaDeJuego : Scene
{
    private MainEntity jugador;

    public MiEscenaDeJuego() : base("MiEscena")
    {
        BackgroundColor = Color.SkyBlue;
    }

    public override void Initialize()
    {
        // Los sistemas ya están creados y registrados automáticamente
        // Solo necesitas crear tus entidades
        
        jugador = World.CreateEntity("Jugador");
        jugador.AddComponent(new TransformComponent 
        { 
            Position = new Vector2(100, 100) 
        });
        jugador.AddComponent(new RigidbodyComponent());
        jugador.AddComponent(new ColliderComponent(new Vector2(32, 32)));
        
        // Configurar input
        var input = new InputComponent();
        input.AssignKeyBinding(InputAction.MoveLeft, Keys.A);
        input.AssignKeyBinding(InputAction.MoveRight, Keys.D);
        jugador.AddComponent(input);
    }

    public override void LoadContent()
    {
        // Cargar texturas, sonidos, etc.
        // var texture = Content.Load<Texture2D>("player");
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        // Lógica específica de la escena (los sistemas se actualizan automáticamente)
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            // Cambiar de escena
            // Necesitarías acceso al SceneManager aquí
        }
    }

    protected override void OnDraw(GameTime gameTime)
    {
        // Dibujar UI o overlays (las entidades se dibujan automáticamente)
        SpriteBatch.Begin();
        // Dibujar HUD, texto, etc.
        SpriteBatch.End();
    }
}
```

## 🔄 Cambio de Escenas

### Opción 1: Usando Eventos (Recomendado)

```csharp
// Dentro de tu escena
World.Events.Subscribe<SceneChangeRequestedEvent>(OnSceneChangeRequested);

// Para cambiar de escena
World.Events.Publish(new SceneChangeRequestedEvent 
{ 
    NewSceneName = "GamePlay",
    TransitionType = "Fade"
});
```

### Opción 2: Extendiendo MyProgram

```csharp
public class MiJuego : MyProgram
{
    protected override void LoadContent()
    {
        base.LoadContent();
        
        // Registrar todas las escenas
        SceneManager.RegisterScene("Menu", new MenuScene());
        SceneManager.RegisterScene("Game", new GameScene());
        SceneManager.RegisterScene("GameOver", new GameOverScene());
        
        // Iniciar con el menú
        SceneManager.SwitchToScene("Menu");
    }
}
```

## 🎮 Flujo del Motor

### Inicialización
1. `MyProgram.Initialize()` → Inicializa servicios globales
2. `MyProgram.LoadContent()` → Crea SceneManager y carga escena inicial
3. `Scene.Initialize()` → Crea sistemas ECS para la escena
4. `Scene.LoadContent()` → Carga recursos de la escena

### Update Loop
1. `MyProgram.Update()`
2. → `SceneManager.Update()`
3. → → `Scene.Update()`
4. → → → `WorldManager.UpdateSystems()` (Input → Physics → Animation → Button)
5. → → → `Scene.OnUpdate()` (lógica específica)

### Draw Loop
1. `MyProgram.Draw()`
2. → `SceneManager.Draw()`
3. → → Clear con `Scene.BackgroundColor`
4. → → `Scene.Draw()`
5. → → → `RenderSystem.Draw()` (dibuja todas las entidades)
6. → → → `Scene.OnDraw()` (UI/overlays)

## ✅ Ventajas del Sistema Integrado

1. **Separación Clara**: Cada escena es independiente con su propio mundo ECS
2. **Gestión Automática**: Los sistemas se crean y actualizan automáticamente
3. **Transiciones Suaves**: SceneManager maneja las transiciones con efectos
4. **Limpieza de Memoria**: Las escenas se limpian completamente al cambiar
5. **Flexibilidad**: Puedes sobrescribir cualquier comportamiento

## 🔧 Configuración Avanzada

### Sistemas Personalizados por Escena

```csharp
public class MiEscenaConSistemasCustom : Scene
{
    protected override void InitializeSystems()
    {
        // Llamar a la base para sistemas por defecto
        base.InitializeSystems();
        
        // Agregar sistemas adicionales
        var miSistema = new MiSistemaPersonalizado();
        World.RegisterSystem<MiSistemaPersonalizado>(miSistema);
    }
}
```

### Desactivar Sistemas por Defecto

```csharp
public class EscenaSinFisica : Scene
{
    protected override void InitializeSystems()
    {
        // NO llamar a base.InitializeSystems()
        // Registrar solo los sistemas que necesitas
        
        RenderSystem = new RenderSystem(GraphicsDevice, SpriteBatch);
        AnimationSystem = new AnimationSystem();
        
        World.RegisterSystem<RenderSystem>(RenderSystem);
        World.RegisterSystem<AnimationSystem>(AnimationSystem);
        // No registrar PhysicsSystem, InputSystem, etc.
    }
}
```

## 📝 Notas Importantes

1. **No uses WorldManager global**: Cada escena tiene su propio WorldManager
2. **Los sistemas se actualizan automáticamente**: No necesitas llamar Update en cada sistema
3. **RenderSystem se encarga del dibujado**: Solo necesitas OnDraw para UI
4. **La limpieza es automática**: Al cambiar de escena, todo se limpia
5. **ESC + Shift para salir**: Combinación global definida en MyProgram

## 🎯 Mejores Prácticas

1. Mantén las escenas pequeñas y enfocadas
2. Usa eventos para comunicación entre sistemas
3. Carga recursos en LoadContent, no en Initialize
4. Libera recursos pesados en Dispose
5. Usa transiciones para cambios de escena suaves

## 🐛 Solución de Problemas

### Las entidades no se dibujan
- Verifica que tengan TransformComponent y SpriteComponent
- Asegúrate de que LayerDepth esté configurado correctamente

### Los sistemas no se actualizan
- Verifica que hayas llamado a base.InitializeSystems() o registrado los sistemas manualmente

### La escena no cambia
- Asegúrate de que el SceneManager tenga la escena registrada
- Verifica que no haya errores en OnDeactivate de la escena actual

## 📚 Ejemplo Completo

Ver los archivos:
- `ExampleMainMenuScene.cs` - Escena de menú con botones
- `ExampleGamePlayScene.cs` - Escena de juego con entidades
- `Program.cs` - Ejemplos de uso del motor

Estos ejemplos muestran el uso correcto del sistema de escenas integrado con el motor ECS.
