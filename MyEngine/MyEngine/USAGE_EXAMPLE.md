# MyEngine - Ejemplo de Uso del Sistema Mejorado

## Ejemplo: Crear una Entidad con Animaciones Direccionales

```csharp
// En tu clase de juego o escena
var world = new WorldManager();

// Crear una entidad de jugador
var player = world.CreateEntity("Player");

// Agregar componentes básicos
player.AddComponent(new TransformComponent { Position = new Vector2(100, 100) });
player.AddComponent(new RigidbodyComponent { Mass = 1.0f });
player.AddComponent(new ColliderComponent(new Vector2(32, 48)) { Tag = "Player" });

// Crear componente de sprites
var texture = Content.Load<Texture2D>("player_spritesheet");
var region = new TextureRegion(texture, 0, 0, 32, 48);
player.AddComponent(new SpriteComponent(region));

// Crear componente de animación
var animationComponent = new AnimationComponent();

// Agregar animaciones direccionales
// RunRight (frames 0-3 en la fila 0)
animationComponent.AddAnimationFromSpriteSheet(
    AnimationAction.RunRight, 
    new TextureRegion(texture, 0, 0, 32, 48), 
    4, 32, 48, 
    TimeSpan.FromMilliseconds(150));

// RunUp (frames 0-3 en la fila 1)  
animationComponent.AddAnimationFromSpriteSheet(
    AnimationAction.RunUp, 
    new TextureRegion(texture, 0, 48, 32, 48), 
    4, 32, 48, 
    TimeSpan.FromMilliseconds(150));

// JumpRight (frames 0-2 en la fila 2)
animationComponent.AddAnimationFromSpriteSheet(
    AnimationAction.JumpRigth, 
    new TextureRegion(texture, 0, 96, 32, 48), 
    3, 32, 48, 
    TimeSpan.FromMilliseconds(200));

// JumpUp (frames 0-2 en la fila 3)
animationComponent.AddAnimationFromSpriteSheet(
    AnimationAction.JumpUp, 
    new TextureRegion(texture, 0, 144, 32, 48), 
    3, 32, 48, 
    TimeSpan.FromMilliseconds(200));

// Agregar más animaciones...
animationComponent.AddAnimationFromSpriteSheet(
    AnimationAction.StopRight, 
    new TextureRegion(texture, 0, 192, 32, 48), 
    1, 32, 48, 
    TimeSpan.FromMilliseconds(1000));

animationComponent.AddAnimationFromSpriteSheet(
    AnimationAction.StopUp, 
    new TextureRegion(texture, 32, 192, 32, 48), 
    1, 32, 48, 
    TimeSpan.FromMilliseconds(1000));

player.AddComponent(animationComponent);

// Agregar controles de input
var inputComponent = new InputComponent();
inputComponent.AssignKeyBinding(InputAction.MoveLeft, Keys.A);
inputComponent.AssignKeyBinding(InputAction.MoveRight, Keys.D);
inputComponent.AssignKeyBinding(InputAction.MoveUp, Keys.W);
inputComponent.AssignKeyBinding(InputAction.MoveDown, Keys.S);
inputComponent.AssignKeyBinding(InputAction.Jump, Keys.Space);

player.AddComponent(inputComponent);
```

## Ejemplo: Crear Botón UI con EventBus

```csharp
// Crear entidad de botón
var menuButton = world.CreateEntity("MenuButton");

// Posición del botón
menuButton.AddComponent(new TransformComponent { Position = new Vector2(400, 300) });

// Sprite del botón
var buttonTexture = Content.Load<Texture2D>("button");
var buttonRegion = new TextureRegion(buttonTexture, 0, 0, 200, 60);
menuButton.AddComponent(new SpriteComponent(buttonRegion));

// Configurar input del botón
var buttonComponent = new ButtonComponent(Keys.Enter, Buttons.A, new Vector2(400, 300), new Vector2(200, 60));
menuButton.AddComponent(buttonComponent);

// Suscribirse a eventos de botón
world.Events.Subscribe<UIButtonClickedEvent>(OnButtonClicked);
world.Events.Subscribe<KeyPressedEvent>(OnKeyPressed);

// Handlers de eventos
private void OnButtonClicked(UIButtonClickedEvent e)
{
    if (e.ButtonId == menuButton.Id)
    {
        // Cambiar de escena, abrir menú, etc.
        world.Events.Publish(new SceneChangeRequestedEvent 
        { 
            NewSceneName = "GameScene",
            TransitionType = "Fade"
        });
    }
}

private void OnKeyPressed(KeyPressedEvent e)
{
    if (e.Key == Keys.Enter)
    {
        // Reproducir sonido de confirmación
        world.Events.Publish(new PlaySoundEvent 
        { 
            SoundName = "button_confirm",
            Volume = 0.8f
        });
    }
}
```

## Flujo de Eventos Automático

Con las mejoras implementadas, el sistema ahora funciona automáticamente:

```
1. Jugador presiona tecla → InputSystem genera EntityDirectionEvent
2. AnimationSystem recibe el evento → Cambia animación y aplica rotación/reflexión
3. PhysicsSystem mueve la entidad → Genera EntityMovedEvent  
4. Colisión detectada → PhysicsSystem genera CollisionEnterEvent
5. AnimationSystem recibe colisión → Reproduce animación de impacto
6. Botón presionado → ButtonSystem genera UIButtonClickedEvent
7. Tu código recibe el evento → Ejecuta acción correspondiente
```

## Configuración de Sistemas

```csharp
// En MyProgram.LoadContent()
protected override void LoadContent()
{
    _spriteBatch = new SpriteBatch(GraphicsDevice);

    // Crear sistemas
    _renderSystem = new RenderSystem(GraphicsDevice, _spriteBatch);
    _animationSystem = new AnimationSystem();
    _physicsSystem = new PhysicsSystem();
    _inputSystem = new InputSystem();
    _buttonSystem = new ButtonSystem();

    // Registrar sistemas (Initialize y SubscribeToEvents se llaman automáticamente)
    _world.RegisterSystem<RenderSystem>(_renderSystem);
    _world.RegisterSystem<AnimationSystem>(_animationSystem);
    _world.RegisterSystem<PhysicsSystem>(_physicsSystem);
    _world.RegisterSystem<InputSystem>(_inputSystem);
    _world.RegisterSystem<ButtonSystem>(_buttonSystem);

    // Habilitar debug del EventBus en desarrollo
    _world.Events.EnableDebugLogging = true;
}

// En MyProgram.Update()
protected override void Update(GameTime gameTime)
{
    // Un solo método actualiza todos los sistemas en el orden correcto
    _world.UpdateSystems(gameTime);
    
    base.Update(gameTime);
}

// En MyProgram.Draw()  
protected override void Draw(GameTime gameTime)
{
    GraphicsDevice.Clear(Color.CornflowerBlue);
    
    // RenderSystem maneja todo el renderizado
    _renderSystem.Draw(gameTime);
    
    base.Draw(gameTime);
}
```

## Ventajas del Sistema Mejorado

### ✅ Consistencia
- Todos los sistemas usan EventBus de manera uniforme
- No más eventos de C# mezclados con GameEvent

### ✅ Animaciones Inteligentes  
- Rotación/reflexión automática basada en dirección de movimiento
- Solo necesitas crear animaciones Up y Right
- El sistema genera Left (flipX de Right) y Down (flipY de Up) automáticamente

### ✅ Orden de Ejecución Garantizado
```
Input → EventBus → Physics → EventBus → Animation → EventBus → UI → EventBus
```

### ✅ Debugging Fácil
```csharp
// Ver estadísticas de eventos
_world.Events.PrintStats();

// Habilitar logging detallado
_world.Events.EnableDebugLogging = true;
```

### ✅ Extensibilidad
```csharp
// Agregar nuevos eventos fácilmente
public class CustomGameEvent : GameEvent
{
    public string CustomData { get; set; }
}

// Suscribirse desde cualquier lugar
world.Events.Subscribe<CustomGameEvent>(HandleCustomEvent);
```

## Casos de Uso Comunes

### Enemigo que Reacciona al Jugador
```csharp
world.Events.Subscribe<EntityMovedEvent>(e => 
{
    if (IsPlayer(e.EntityId))
    {
        // Hacer que los enemigos reaccionen
        UpdateEnemyBehavior(e.NewPosition);
    }
});
```

### Sistema de Logros
```csharp
world.Events.Subscribe<ItemCollectedEvent>(e => AchievementSystem.CheckItemCollection(e));
world.Events.Subscribe<CollisionEnterEvent>(e => AchievementSystem.CheckCombat(e));
```

### Audio Reactivo
```csharp
world.Events.Subscribe<AnimationStartedEvent>(e => 
{
    if (e.AnimationName == AnimationAction.JumpRigth)
        world.Events.Publish(new PlaySoundEvent { SoundName = "jump" });
});
```
