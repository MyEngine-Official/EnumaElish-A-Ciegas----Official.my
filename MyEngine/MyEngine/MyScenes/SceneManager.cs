using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MyEngine.Controllers;

namespace MyEngine.MyScenes
{
    /// <summary>
    /// Manages scene transitions and active scenes
    /// </summary>
    public class SceneManager
    {
        private readonly Dictionary<string, Scene> _scenes;
        private Scene _currentScene;
        private Scene _nextScene;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly SpriteBatch _spriteBatch;
        private readonly ContentManager _content;
        private readonly AudioController _audioController;
        
        // Transition effects
        private bool _isTransitioning;
        private float _transitionAlpha;
        private float _transitionSpeed;
        private Texture2D _transitionTexture;
        private TransitionType _transitionType;

        /// <summary>
        /// Gets the currently active scene
        /// </summary>
        public Scene CurrentScene => _currentScene;
        
        /// <summary>
        /// Is a scene transition in progress
        /// </summary>
        public bool IsTransitioning => _isTransitioning;

        /// <summary>
        /// Creates a new scene manager
        /// </summary>
        public SceneManager(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, 
            ContentManager content, AudioController resourceManager)
        {
            _graphicsDevice = graphicsDevice;
            _spriteBatch = spriteBatch;
            _content = content;
            _audioController = resourceManager;
            _scenes = new Dictionary<string, Scene>();
            _transitionSpeed = 2.0f;
            
            // Create a white pixel for transitions
            _transitionTexture = new Texture2D(graphicsDevice, 1, 1);
            _transitionTexture.SetData(new[] { Color.White });
        }

        /// <summary>
        /// Registers a scene with the manager
        /// </summary>
        public void RegisterScene(string name, Scene scene)
        {
            if (_scenes.ContainsKey(name))
                throw new ArgumentException($"Scene '{name}' is already registered.");
                
            _scenes[name] = scene;
        }

        /// <summary>
        /// Registers and immediately switches to a scene
        /// </summary>
        public void RegisterAndSwitch(string name, Scene scene)
        {
            RegisterScene(name, scene);
            SwitchToScene(name);
        }

        /// <summary>
        /// Switches to a registered scene
        /// </summary>
        public void SwitchToScene(string name, TransitionType transition = TransitionType.None)
        {
            if (!_scenes.ContainsKey(name))
                throw new ArgumentException($"Scene '{name}' is not registered.");
                
            _nextScene = _scenes[name];
            _transitionType = transition;
            
            if (transition != TransitionType.None)
            {
                _isTransitioning = true;
                _transitionAlpha = 0f;
            }
            else
            {
                PerformSceneSwitch();
            }
        }

        /// <summary>
        /// Switches to a new scene instance
        /// </summary>
        public void SwitchToScene(Scene scene, TransitionType transition = TransitionType.None)
        {
            _nextScene = scene;
            _transitionType = transition;
            
            if (transition != TransitionType.None)
            {
                _isTransitioning = true;
                _transitionAlpha = 0f;
            }
            else
            {
                PerformSceneSwitch();
            }
        }

        /// <summary>
        /// Performs the actual scene switch
        /// </summary>
        private void PerformSceneSwitch()
        {
            // Deactivate and dispose current scene
            if (_currentScene != null)
            {
                _currentScene.OnDeactivate();
                _currentScene.Dispose();
            }

            // Force garbage collection
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            // Initialize and activate new scene
            _currentScene = _nextScene;
            _nextScene = null;
            
            if (_currentScene != null && !_currentScene.IsInitialized)
            {
                _currentScene.InternalInitialize(_graphicsDevice, _spriteBatch, _content, _audioController);
            }
            
            _currentScene?.OnActivate();
        }

        /// <summary>
        /// Updates the scene manager and current scene
        /// </summary>
        public void Update(GameTime gameTime)
        {
            // Handle transitions
            if (_isTransitioning)
            {
                UpdateTransition(gameTime);
            }
            
            // Update current scene
            if (_currentScene != null && _currentScene.IsActive)
            {
                _currentScene.HandleInput(gameTime);
                _currentScene.Update(gameTime);
            }
        }

        /// <summary>
        /// Updates scene transition
        /// </summary>
        private void UpdateTransition(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            if (_transitionAlpha < 1f && _nextScene != null)
            {
                // Fade out
                _transitionAlpha += deltaTime * _transitionSpeed;
                if (_transitionAlpha >= 1f)
                {
                    _transitionAlpha = 1f;
                    PerformSceneSwitch();
                }
            }
            else if (_transitionAlpha > 0f && _nextScene == null)
            {
                // Fade in
                _transitionAlpha -= deltaTime * _transitionSpeed;
                if (_transitionAlpha <= 0f)
                {
                    _transitionAlpha = 0f;
                    _isTransitioning = false;
                }
            }
        }

        /// <summary>
        /// Draws the current scene
        /// </summary>
        public void Draw(GameTime gameTime)
        {
            // Clear with scene's background color
            if (_currentScene != null)
            {
                _graphicsDevice.Clear(_currentScene.BackgroundColor);
                _currentScene.Draw(gameTime);
            }
            
            // Draw transition overlay
            if (_isTransitioning && _transitionAlpha > 0f)
            {
                DrawTransition();
            }
        }

        /// <summary>
        /// Draws the transition overlay
        /// </summary>
        private void DrawTransition()
        {
            _spriteBatch.Begin();
            
            Color overlayColor = Color.Black * _transitionAlpha;
            Rectangle screenBounds = _graphicsDevice.Viewport.Bounds;
            
            switch (_transitionType)
            {
                case TransitionType.Fade:
                    _spriteBatch.Draw(_transitionTexture, screenBounds, overlayColor);
                    break;
                    
                case TransitionType.SlideLeft:
                    int slideX = (int)(screenBounds.Width * _transitionAlpha);
                    _spriteBatch.Draw(_transitionTexture, 
                        new Rectangle(screenBounds.Width - slideX, 0, slideX, screenBounds.Height), 
                        Color.Black);
                    break;
                    
                case TransitionType.SlideRight:
                    int slideX2 = (int)(screenBounds.Width * _transitionAlpha);
                    _spriteBatch.Draw(_transitionTexture, 
                        new Rectangle(0, 0, slideX2, screenBounds.Height), 
                        Color.Black);
                    break;
            }
            
            _spriteBatch.End();
        }

        /// <summary>
        /// Gets a registered scene by name
        /// </summary>
        public Scene GetScene(string name)
        {
            return _scenes.ContainsKey(name) ? _scenes[name] : null;
        }

        /// <summary>
        /// Removes a scene from the manager
        /// </summary>
        public void RemoveScene(string name)
        {
            if (_scenes.ContainsKey(name))
            {
                var scene = _scenes[name];
                if (scene == _currentScene)
                {
                    throw new InvalidOperationException("Cannot remove the currently active scene.");
                }
                
                scene.Dispose();
                _scenes.Remove(name);
            }
        }

        /// <summary>
        /// Called when window is resized
        /// </summary>
        public void OnWindowResize(int width, int height)
        {
            _currentScene?.OnWindowResize(width, height);
        }

        /// <summary>
        /// Disposes all scenes and resources
        /// </summary>
        public void Dispose()
        {
            foreach (var scene in _scenes.Values)
            {
                scene.Dispose();
            }
            _scenes.Clear();
            
            _transitionTexture?.Dispose();
            _currentScene = null;
            _nextScene = null;
        }
    }

    /// <summary>
    /// Types of scene transitions
    /// </summary>
    public enum TransitionType
    {
        None,
        Fade,
        SlideLeft,
        SlideRight
    }
}
