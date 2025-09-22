using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using MyEngine.MyCore.Events;
using MyEngine.MyCore.MyComponents;

namespace MyEngine.MyCore.MySystems
{
    public class ButtonSystem : ISystem, IEventSubscriber
    {
        private WorldManager _world;
        private EventBus _eventBus;

        // Input state tracking
        private KeyboardState _previousKeyboardState;
        private KeyboardState _currentKeyboardState;
        private GamePadState[] _previousGamePadStates = new GamePadState[4];
        private GamePadState[] _currentGamePadStates = new GamePadState[4];
        private TouchCollection _previousTouchState;
        private TouchCollection _currentTouchState;

        private bool _isEnabled = true;

        public void Initialize(WorldManager world)
        {
            _world = world;
            _eventBus = world.Events;
            
            // Initialize input states
            _currentKeyboardState = Keyboard.GetState();
            _currentTouchState = TouchPanel.GetState();
            
            for (int i = 0; i < 4; i++)
            {
                _currentGamePadStates[i] = GamePad.GetState((PlayerIndex)i);
            }
        }

        public void SubscribeToEvents(EventBus eventBus)
        {
            // ButtonSystem puede reaccionar a eventos de UI si es necesario
            eventBus.Subscribe<GamePausedEvent>(OnGamePaused);
            eventBus.Subscribe<GameResumedEvent>(OnGameResumed);
        }

        public void Update(GameTime gameTime)
        {
            if (!_isEnabled) return;

            // Update input states
            _previousKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();

            _previousTouchState = _currentTouchState;
            _currentTouchState = TouchPanel.GetState();

            for (int i = 0; i < 4; i++)
            {
                _previousGamePadStates[i] = _currentGamePadStates[i];
                _currentGamePadStates[i] = GamePad.GetState((PlayerIndex)i);
            }

            // Process all entities with ButtonComponent
            var buttonEntities = _world.GetEntitiesWithComponents<ButtonComponent>();

            foreach (var entity in buttonEntities)
            {
                var buttonComponent = entity.GetComponent<ButtonComponent>();
                ProcessButtonEntity(entity, buttonComponent);
            }
        }

        private void ProcessButtonEntity(MyCore.MyEntities.MainEntity entity, ButtonComponent buttonComponent)
        {
            // Process keyboard input
            foreach (var key in buttonComponent.KeyEvent)
            {
                if (WasKeyJustPressed(key))
                {
                    _eventBus.Publish(new KeyPressedEvent
                    {
                        Key = key,
                        EntityId = entity.Id
                    });

                    _eventBus.Publish(new UIButtonClickedEvent
                    {
                        ButtonId = entity.Id,
                        ButtonName = $"Button_{entity.Id}",
                        ClickPosition = Vector2.Zero // For keyboard, position is not relevant
                    });
                }

                if (WasKeyJustReleased(key))
                {
                    _eventBus.Publish(new KeyReleasedEvent
                    {
                        Key = key,
                        EntityId = entity.Id
                    });
                }
            }

            // Process gamepad input
            for (int playerId = 0; playerId < 4; playerId++)
            {
                if (!_currentGamePadStates[playerId].IsConnected) continue;

                foreach (var button in buttonComponent.ButtonEvent)
                {
                    if (WasButtonJustPressed(playerId, button))
                    {
                        _eventBus.Publish(new GamePadButtonEvent
                        {
                            Button = button,
                            PlayerId = playerId,
                            EntityId = entity.Id,
                            IsPressed = true
                        });

                        _eventBus.Publish(new UIButtonClickedEvent
                        {
                            ButtonId = entity.Id,
                            ButtonName = $"Button_{entity.Id}",
                            ClickPosition = Vector2.Zero
                        });
                    }

                    if (WasButtonJustReleased(playerId, button))
                    {
                        _eventBus.Publish(new GamePadButtonEvent
                        {
                            Button = button,
                            PlayerId = playerId,
                            EntityId = entity.Id,
                            IsPressed = false
                        });
                    }
                }
            }

            // Process touch input
            foreach (var touchArea in buttonComponent.TouchEvent)
            {
                Vector2 center = touchArea.Key;
                Vector2 size = touchArea.Value;

                Rectangle touchRect = new Rectangle(
                    (int)(center.X - size.X / 2f),
                    (int)(center.Y - size.Y / 2f),
                    (int)size.X,
                    (int)size.Y
                );

                ProcessTouchInput(entity, touchRect, center);
            }
        }

        private void ProcessTouchInput(MyCore.MyEntities.MainEntity entity, Rectangle touchRect, Vector2 center)
        {
            // Check for new touches
            foreach (var currentTouch in _currentTouchState)
            {
                if (currentTouch.State == TouchLocationState.Pressed || 
                    currentTouch.State == TouchLocationState.Moved)
                {
                    if (touchRect.Contains(currentTouch.Position))
                    {
                        // Check if this is a new touch (not in previous state)
                        bool isNewTouch = !_previousTouchState.Any(prevTouch => 
                            prevTouch.Id == currentTouch.Id && 
                            touchRect.Contains(prevTouch.Position));

                        if (isNewTouch || currentTouch.State == TouchLocationState.Pressed)
                        {
                            _eventBus.Publish(new TouchEvent
                            {
                                Position = center,
                                Size = new Vector2(touchRect.Width, touchRect.Height),
                                EntityId = entity.Id,
                                IsPressed = true
                            });

                            _eventBus.Publish(new UIButtonClickedEvent
                            {
                                ButtonId = entity.Id,
                                ButtonName = $"Button_{entity.Id}",
                                ClickPosition = currentTouch.Position
                            });
                        }
                    }
                }
            }

            // Check for touch releases
            foreach (var prevTouch in _previousTouchState)
            {
                if (prevTouch.State == TouchLocationState.Pressed || 
                    prevTouch.State == TouchLocationState.Moved)
                {
                    if (touchRect.Contains(prevTouch.Position))
                    {
                        // Check if this touch is no longer active
                        bool touchReleased = !_currentTouchState.Any(currentTouch => 
                            currentTouch.Id == prevTouch.Id && 
                            (currentTouch.State == TouchLocationState.Pressed || 
                             currentTouch.State == TouchLocationState.Moved));

                        if (touchReleased)
                        {
                            _eventBus.Publish(new TouchEvent
                            {
                                Position = center,
                                Size = new Vector2(touchRect.Width, touchRect.Height),
                                EntityId = entity.Id,
                                IsPressed = false
                            });
                        }
                    }
                }
            }
        }

        // Input query helper methods
        private bool WasKeyJustPressed(Keys key)
        {
            return _currentKeyboardState.IsKeyDown(key) && !_previousKeyboardState.IsKeyDown(key);
        }

        private bool WasKeyJustReleased(Keys key)
        {
            return !_currentKeyboardState.IsKeyDown(key) && _previousKeyboardState.IsKeyDown(key);
        }

        private bool WasButtonJustPressed(int playerId, Buttons button)
        {
            return _currentGamePadStates[playerId].IsButtonDown(button) && 
                   !_previousGamePadStates[playerId].IsButtonDown(button);
        }

        private bool WasButtonJustReleased(int playerId, Buttons button)
        {
            return !_currentGamePadStates[playerId].IsButtonDown(button) && 
                   _previousGamePadStates[playerId].IsButtonDown(button);
        }

        // Event handlers
        private void OnGamePaused(GamePausedEvent e)
        {
            _isEnabled = false; // Solo permitir input de UI durante pausa
        }

        private void OnGameResumed(GameResumedEvent e)
        {
            _isEnabled = true;
        }
    }
}
