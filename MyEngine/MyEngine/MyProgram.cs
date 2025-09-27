using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyEngine.Controllers;
using MyEngine.MyCore;
using MyEngine.MyCore.MyComponents;
using MyEngine.MyCore.MyEntities;
using MyEngine.MyCore.MySystems;
using MyEngine.MyScenes;
using MyEngine.Scripting;
using MyEngine.Scripting.F_;
using System;
using System.Collections.Generic;
using System.Linq;
using static FSharp.Compiler.Symbols.FSharpImplementationFileDeclaration;

namespace MyEngine
{
    public class MyProgram : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Services
        private AudioController _audioController;
        private SceneManager _sceneManager;
        Scene _initialScene;
        // Configuration
        private readonly string _title;
        private readonly int _width;
        private readonly int _height;
        private readonly bool _fullScreen;
        private readonly bool _allowUserResizing;
        public MyProgram(string xmlGame, string title = "MyEngine Game", int width = 1280, int height = 720, 
            bool fullScreen = false, bool allowUserResizing = false)
        {
            _title = title;
            _width = width;
            _height = height;
            _fullScreen = fullScreen;
            _allowUserResizing = allowUserResizing;

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
            ReadXmlConfigurationGame(xmlGame);
        }

        private void ReadXmlConfigurationGame(string xmlGameConfig)
        {
            XMLGameManager gameManager = new XMLGameManager(xmlGameConfig);
            List<SceneInfo> escenas = gameManager.ListScenes();

            var primeraEscena = escenas.OrderBy(s => s.Id).FirstOrDefault();
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

public class SceneImplementation : Scene
{
    private XMLGameManager _gameManager;
    private int _id;
    private ContentManager cont;
    private Dictionary<MainEntity,IScript> Scripts { get; set; }
    public SceneImplementation(XMLGameManager gameManager, int id, string nombre, ContentManager content) : base(nombre)
    {
        _gameManager = gameManager;
        _id = id;
        cont = content;
    }

    public override void Initialize()
    {
        base.InternalInitialize(GraphicsDevice, SpriteBatch, cont, Audio);
        ResourceManager resourceManager = new ResourceManager(Content);
        List<EntityInfo> entityInfos = _gameManager.ListEntitiesByScene(_id);

        foreach(var entityInfo in entityInfos)
        {
            var entity = World.CreateEntity(entityInfo.Name);
            var componentInfo = _gameManager.ListEntityComponents(entityInfo.Id);

            foreach(var component in componentInfo)
                {
                    if (!component.Active) continue;

                    switch (component.Type)
                    {
                        case "SpriteComponent":
                            component.Attributes.TryGetValue("path", out string path);
                            component.Attributes.TryGetValue("spriteName", out string name);
                            entity.AddComponent<SpriteComponent>(resourceManager.CrearSprite(path, name));
                            break;
                        case "AnimationComponent":
                            component.Attributes.TryGetValue("path", out string route);
                            component.Attributes.TryGetValue("animationName", out string animationName);
                            entity.AddComponent<AnimationComponent>(resourceManager.CrearAnimacion(route, animationName));
                            break;
                        case "ButtonComponent":
                            component.Attributes.TryGetValue("keyboardKey", out string keyboardKey);
                            component.Attributes.TryGetValue("gamePadButton", out string gamePadButton);
                            component.Attributes.TryGetValue("x", out string x);
                            component.Attributes.TryGetValue("y", out string y);
                            component.Attributes.TryGetValue("width", out string width);
                            component.Attributes.TryGetValue("height", out string height);
                            entity.AddComponent<ButtonComponent>(resourceManager.CrearBoton((Keys)Enum.Parse(typeof(Keys), keyboardKey), (Buttons)Enum.Parse(typeof(Buttons), gamePadButton), new Vector2(float.Parse(x), float.Parse(y)), new Vector2(float.Parse(width), float.Parse(height))));
                            break;
                        case "ColliderComponent":
                            component.Attributes.TryGetValue("size", out string size);
                            component.Attributes.TryGetValue("offset", out string offset);
                            component.Attributes.TryGetValue("isTrigger", out string isTrigger);
                            component.Attributes.TryGetValue("tag", out string tag);
                            component.Attributes.TryGetValue("layer", out string layer);
                            component.Attributes.TryGetValue("cmask", out string cmask);
                            component.Attributes.TryGetValue("isEnabled", out string isEnabled);
                            string[] sizeValues = size.Split(',');
                            string[] offsetValues = offset.Split(',');
                            entity.AddComponent<ColliderComponent>(resourceManager.CrearColisionador(new Vector2(float.Parse(sizeValues[0]), float.Parse(sizeValues[1])), new Vector2(float.Parse(offsetValues[0]), float.Parse(offsetValues[1])), bool.Parse(isTrigger), tag, (CollisionLayer)Enum.Parse(typeof(CollisionLayer), layer), (CollisionLayer)Enum.Parse(typeof(CollisionLayer), cmask), bool.Parse(isEnabled)));
                            break;
                        case "TransformComponent":
                            component.Attributes.TryGetValue("position", out string position);
                            component.Attributes.TryGetValue("rotation", out string rotation);
                            component.Attributes.TryGetValue("scale", out string scale);
                            string[] positionValues = position.Split(',');
                            string[] scaleValues = scale.Split(',');
                            entity.AddComponent<TransformComponent>(resourceManager.CrearTransformacion(new Vector2(float.Parse(positionValues[0]), float.Parse(positionValues[1])), float.Parse(rotation), new Vector2(float.Parse(scaleValues[0]), float.Parse(scaleValues[1]))));
                            break;
                        case "InputComponent":
                            component.Attributes.TryGetValue("mouse", out string mouse);
                            component.Attributes.TryGetValue("gamePad", out string gamePad);
                            component.Attributes.TryGetValue("keyboard", out string keyboard);
                            component.Attributes.TryGetValue("isEnabled", out string inputIsEnabled);
                            component.Attributes.TryGetValue("gamePadIndex", out string gamePadIndex);
                            entity.AddComponent<InputComponent>(resourceManager.CrearInput(bool.Parse(mouse), bool.Parse(gamePad), bool.Parse(keyboard), bool.Parse(inputIsEnabled), int.Parse(gamePadIndex)));
                            break;
                        case "LifeComponent":
                            component.Attributes.TryGetValue("maxHealth", out string maxHealth);
                            entity.AddComponent<LifeComponent>(resourceManager.CrearVida(int.Parse(maxHealth)));
                            break;
                        case "RigidbodyComponent":
                            component.Attributes.TryGetValue("mass", out string mass);
                            component.Attributes.TryGetValue("acceleration", out string asceleration);
                            component.Attributes.TryGetValue("velocityX", out string velocityX);
                            component.Attributes.TryGetValue("velocityY", out string velocityY);
                            entity.AddComponent<RigidbodyComponent>(resourceManager.CrearRigidbody(float.Parse(mass), float.Parse(asceleration), new Vector2(float.Parse(velocityX), float.Parse(velocityY))));
                            break;
                        case "TilemapComponent":
                            component.Attributes.TryGetValue("path", out string tilemapPath);
                            entity.AddComponent<TilemapComponent>(resourceManager.CrearMapa(tilemapPath));
                            break;
                        case "TilemapCollisionsComponent":
                            component.Attributes.TryGetValue("path", out string tilemapCollisionPath);
                            entity.AddComponent<TilemapCollisionsComponent>(resourceManager.CrearColisionesMapa(tilemapCollisionPath));
                            break;
                        case "ScriptComponent":
                        component.Attributes.TryGetValue("path", out string ScriptPath);
                        component.Attributes.TryGetValue("languaje", out string languaje);

                        if (Enum.TryParse<ScriptLanguaje>(languaje, out ScriptLanguaje scriptLanguajeParseado))
                        {
                            entity.AddComponent<ScriptComponent>(resourceManager.CrearScripting(ScriptPath, scriptLanguajeParseado));
                        }
                        else
                        {
                            World.RemoveEntity(entity);
                            throw new Exception($"El lenguaje del script no es valido. EntidadId: {entity.Id}, con el nombre de {entityInfo.Name}");
                        }

                        break;
                    default:
                            World.RemoveEntity(entity);
                            throw new Exception($"El componente que se quiere agregar a la entidad, no existe. EntidadId: {entity.Id}, con el nombre de {entityInfo.Name}");
                    }

                    
                }

        }

        foreach (MainEntity entidad in World.GetAllEntities())
        {
            if (!entidad.HasComponent<ScriptComponent>()) continue;
            var scriptComp = entidad.GetComponent<ScriptComponent>();
            switch (scriptComp.Languaje)
            {
                case ScriptLanguaje.CSharp:
                    Scripts[entidad] = ScriptLoader.LoadScriptInstance(scriptComp.FilePath, entidad);
                    break;
                case ScriptLanguaje.FSharp:
                    throw new NotImplementedException("F# Todavia no esta soportado por el motor. Pero si mas adelante.");
                //Scripts[entidad.Id] = FSharpScriptCompiler.CompileScript(scriptComp.FilePath);
                case ScriptLanguaje.JavaScript:
                    throw new NotImplementedException("JavaScript Todavia no esta soportado por el motor. Pero si mas adelante.");
                case ScriptLanguaje.Python:
                    throw new NotImplementedException("Python Todavia no esta soportado por el motor. Pero si mas adelante.");
                case ScriptLanguaje.Lua:
                    throw new NotImplementedException("Lua Todavia no esta soportado por el motor. Pero si mas adelante.");
                case ScriptLanguaje.Ruby:
                    throw new NotImplementedException("Ruby Todavia no esta soportado por el motor. Pero si mas adelante.");
                default:
                    throw new Exception($"El lenguaje del script no es valido. EntidadId: {entidad.Id}, lenguaje solicitado fue {scriptComp.Languaje} con path {scriptComp.FilePath}");
            }
        }
    }

    public override void LoadContent()
    {
        base.OnActivate();
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        foreach (var script in Scripts)
        {
            script.Value.Update(gameTime, script.Key);
        }
        base.OnUpdate(gameTime);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }


}