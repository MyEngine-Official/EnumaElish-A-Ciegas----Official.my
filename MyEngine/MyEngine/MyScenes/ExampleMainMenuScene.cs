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
    /// Example main menu scene demonstrating scene system integration
    /// </summary>
    public class ExampleMainMenuScene : Scene
    {
        private MainEntity _titleText;
        private MainEntity _playButton;
        private MainEntity _exitButton;
        private Texture2D _whitePixel;
        private SpriteFont _font;
        private bool _isTransitioning = false;

        public ExampleMainMenuScene() : base("MainMenu")
        {
            BackgroundColor = new Color(20, 20, 30); // Dark blue background
        }

        public override void Initialize()
        {
            // Create a white pixel for drawing rectangles
            _whitePixel = new Texture2D(GraphicsDevice, 1, 1);
            _whitePixel.SetData(new[] { Color.White });

            // Create title entity
            _titleText = World.CreateEntity("Title");
            _titleText.AddComponent(new TransformComponent 
            { 
                Position = new Vector2(GraphicsDevice.Viewport.Width / 2, 100)
            });

            // Create Play button
            _playButton = World.CreateEntity("PlayButton");
            _playButton.AddComponent(new TransformComponent 
            { 
                Position = new Vector2(GraphicsDevice.Viewport.Width / 2 - 100, 250)
            });
            
            var playButtonSprite = new SpriteComponent(new TextureRegion(_whitePixel));
            playButtonSprite.Color = Color.Green;
            playButtonSprite.Scale = new Vector2(200, 50);
            _playButton.AddComponent(playButtonSprite);
            
            var playButtonComp = new ButtonComponent(
                Keys.Enter, 
                Buttons.A, 
                new Vector2(GraphicsDevice.Viewport.Width / 2 - 100, 250), 
                new Vector2(200, 50));
            _playButton.AddComponent(playButtonComp);

            // Create Exit button
            _exitButton = World.CreateEntity("ExitButton");
            _exitButton.AddComponent(new TransformComponent 
            { 
                Position = new Vector2(GraphicsDevice.Viewport.Width / 2 - 100, 320)
            });
            
            var exitButtonSprite = new SpriteComponent(new TextureRegion(_whitePixel));
            exitButtonSprite.Color = Color.Red;
            exitButtonSprite.Scale = new Vector2(200, 50);
            _exitButton.AddComponent(exitButtonSprite);
            
            var exitButtonComp = new ButtonComponent(
                Keys.Escape, 
                Buttons.B, 
                new Vector2(GraphicsDevice.Viewport.Width / 2 - 100, 320), 
                new Vector2(200, 50));
            _exitButton.AddComponent(exitButtonComp);

            // Subscribe to button events
            World.Events.Subscribe<UIButtonClickedEvent>(OnButtonClicked);
        }

        public override void LoadContent()
        {
            // Try to load a font if available, otherwise we'll draw without text
            try
            {
                // This would load a font if you have one in your Content pipeline
                // _font = Content.Load<SpriteFont>("defaultFont");
            }
            catch
            {
                // No font available, that's okay for this example
            }
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            // Check for keyboard shortcuts
            var keyboardState = Keyboard.GetState();
            
            if (!_isTransitioning)
            {
                if (keyboardState.IsKeyDown(Keys.Enter) || keyboardState.IsKeyDown(Keys.Space))
                {
                    StartGame();
                }
                else if (keyboardState.IsKeyDown(Keys.Escape) && keyboardState.IsKeyDown(Keys.LeftShift))
                {
                    // Exit game (Shift+Escape)
                    // This would typically call Game.Exit() but we don't have access here
                    // The main MyProgram handles this combination
                }
            }

            // Update button hover states based on mouse position
            var mouseState = Mouse.GetState();
            UpdateButtonHover(_playButton, mouseState);
            UpdateButtonHover(_exitButton, mouseState);
        }

        private void UpdateButtonHover(MainEntity button, MouseState mouseState)
        {
            if (!button.HasComponent<ButtonComponent>() || !button.HasComponent<SpriteComponent>())
                return;
                
            var buttonComp = button.GetComponent<ButtonComponent>();
            var sprite = button.GetComponent<SpriteComponent>();
            
            // Simple hover effect
            if (buttonComp.IsMouseOver(new Vector2(mouseState.X, mouseState.Y)))
            {
                sprite.Scale = new Vector2(210, 55); // Slightly larger on hover
            }
            else
            {
                sprite.Scale = new Vector2(200, 50); // Normal size
            }
        }

        protected override void OnDraw(GameTime gameTime)
        {
            // Draw UI text on top of entities
            SpriteBatch.Begin();

            // Draw title
            DrawText("MYENGINE DEMO", new Vector2(GraphicsDevice.Viewport.Width / 2 - 100, 100), Color.White, 2f);
            
            // Draw button labels
            DrawText("PLAY", new Vector2(GraphicsDevice.Viewport.Width / 2 - 30, 265), Color.Black, 1f);
            DrawText("EXIT", new Vector2(GraphicsDevice.Viewport.Width / 2 - 25, 335), Color.White, 1f);
            
            // Draw instructions
            DrawText("Press ENTER to Play", new Vector2(GraphicsDevice.Viewport.Width / 2 - 80, 400), Color.Gray, 0.75f);
            DrawText("Press SHIFT+ESC to Exit", new Vector2(GraphicsDevice.Viewport.Width / 2 - 90, 420), Color.Gray, 0.75f);

            SpriteBatch.End();
        }

        private void DrawText(string text, Vector2 position, Color color, float scale)
        {
            if (_font != null)
            {
                SpriteBatch.DrawString(_font, text, position, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
            else
            {
                // Fallback: Draw rectangles to simulate text (very basic)
                // This is just for demonstration when no font is loaded
                foreach (char c in text)
                {
                    if (c != ' ')
                    {
                        SpriteBatch.Draw(_whitePixel, 
                            new Rectangle((int)position.X, (int)position.Y, (int)(6 * scale), (int)(10 * scale)), 
                            color * 0.5f);
                    }
                    position.X += 8 * scale;
                }
            }
        }

        private void OnButtonClicked(UIButtonClickedEvent e)
        {
            if (_isTransitioning) return;

            if (e.ButtonId == _playButton.Id)
            {
                StartGame();
            }
            else if (e.ButtonId == _exitButton.Id)
            {
                // Exit game - would need reference to Game class
                World.Events.Publish(new GameExitRequestedEvent());
            }
        }

        private void StartGame()
        {
            if (_isTransitioning) return;
            _isTransitioning = true;

            // Publish scene change event
            World.Events.Publish(new SceneChangeRequestedEvent 
            { 
                NewSceneName = "GamePlay",
                TransitionType = "Fade"
            });

            // Note: In a real implementation, you would handle this event in MyProgram
            // or have a reference to SceneManager to switch scenes
            // For now, we'll just log that we want to change scenes
            Console.WriteLine("Requesting scene change to GamePlay scene...");
        }

        public override void OnDeactivate()
        {
            base.OnDeactivate();
            _isTransitioning = false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _whitePixel?.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    // Additional events for scene management
    public class SceneChangeRequestedEvent : GameEvent
    {
        public string NewSceneName { get; set; }
        public string TransitionType { get; set; }
    }

    public class GameExitRequestedEvent : GameEvent { }
}
