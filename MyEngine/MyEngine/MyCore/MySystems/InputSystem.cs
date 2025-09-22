using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MyEngine.MyCore.Events;
using MyEngine.MyCore.MyComponents;
using MyEngine.MyCore.MyEntities;

namespace MyEngine.MyCore.MySystems
{
    public class InputSystem: ISystem, IEventSubscriber
    {
        private WorldManager _world;
        private EventBus _eventBus;

        private KeyboardState _previousKeyboardState;
        private KeyboardState _currentKeyboardState;
        private MouseState _previousMouseState;
        private MouseState _currentMouseState;
        private GamePadState[] _previousGamePadStates;
        private GamePadState[] _currentGamePadStates;
        private bool _inputEnabled = true;
        public InputSystem()
        {
            _previousGamePadStates = new GamePadState[4];
            _currentGamePadStates = new GamePadState[4];
        }

        public void Initialize(WorldManager world)
        {
            _world = world;
            _eventBus = world.Events;
        }

        public void SubscribeToEvents(EventBus eventBus)
        {
            // Input System puede reaccionar a eventos del juego
            eventBus.Subscribe<GamePausedEvent>(OnGamePaused);
            eventBus.Subscribe<GameResumedEvent>(OnGameResumed);
        }

        /// <summary>
        /// Updates input states
        /// </summary>
        public void Update(GameTime gameTime)
        {
            if (!_inputEnabled) return;

            // Update states
            _previousKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();

            _previousMouseState = _currentMouseState;
            _currentMouseState = Mouse.GetState();

            for (int i = 0; i < 4; i++)
            {
                _previousGamePadStates[i] = _currentGamePadStates[i];
                _currentGamePadStates[i] = GamePad.GetState((PlayerIndex)i);
            }

            // Process input for entities with InputComponent
            var inputEntities = _world.GetEntitiesWithComponents<InputComponent, RigidbodyComponent>();

            foreach (var entity in inputEntities)
            {
                var input = entity.GetComponent<InputComponent>();
                ProcessEntityInput(entity, input, gameTime);
            }

            PublishGlobalInputEvents();
        }


        private void ProcessEntityInput(MainEntity entity, InputComponent input, GameTime gameTime)
        {
            if (!input.IsEnabled) return;

                float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
                Vector2 vel = entity.GetComponent<RigidbodyComponent>().Velocity;
            Vector2 movement = Vector2.Zero;
                    // Process keyboard input
                if (input.UseKeyboard)
                {
                    movement = Vector2.Zero;

                    if (IsKeyDown(input.KeyBindings[InputAction.MoveUp]))
                        movement.Y -= vel.Y;
                    if (IsKeyDown(input.KeyBindings[InputAction.MoveDown]))
                        movement.Y += vel.Y;
                    if (IsKeyDown(input.KeyBindings[InputAction.MoveLeft]))
                        movement.X -= vel.X;
                    if (IsKeyDown(input.KeyBindings[InputAction.MoveRight]))
                        movement.X += vel.X;

                    if (movement != Vector2.Zero)
                    {
                        entity.GetComponent<RigidbodyComponent>().SetVelocity(movement);

                        // Check action keys
                        if (IsKeyDown(input.KeyBindings[InputAction.Action]))
                        {
                            input.InvocarAction(entity);
                        }

                        if (IsKeyDown(input.KeyBindings[InputAction.Jump]))
                        {

                            input.InvocarJump(entity);
                        }
                    }
                }

            // Process gamepad input
            if (input.UseGamepad && input.GamepadIndex < 4)
                {
                    movement = Vector2.Zero;

                    var gamePadState = _currentGamePadStates[input.GamepadIndex];

                    if (gamePadState.IsConnected)
                    {
                        Vector2 leftStick = gamePadState.ThumbSticks.Left;
                        leftStick.Y *= -1; // Invert Y axis

                        if (leftStick.LengthSquared() > 0.1f * 0.1f) // Dead zone
                        {


                            if (IsButtonDown(input.GamepadIndex, input.GamepadBindings[InputAction.MoveUp]))
                            {
                                movement.Y -= vel.Y;
                            }

                            if (IsButtonDown(input.GamepadIndex, input.GamepadBindings[InputAction.MoveDown]))
                            {
                                movement.Y += vel.Y;
                            }
                            if (IsButtonDown(input.GamepadIndex, input.GamepadBindings[InputAction.MoveLeft]))
                            {
                                movement.X -= vel.X;
                            }

                            if (IsButtonDown(input.GamepadIndex, input.GamepadBindings[InputAction.MoveRight]))
                            {
                                movement.X += vel.X;
                            }
                        }
                    }
                }

            if (movement != Vector2.Zero)
            {
                _eventBus.Publish<EntityDirectionEvent>(new EntityDirectionEvent
                {
                    EntityId = entity.Id,
                    Direction = movement,
                });
            }
                   
        }

        // Input query methods
        public bool IsKeyDown(Keys key) => _currentKeyboardState.IsKeyDown(key);
        public bool IsKeyUp(Keys key) => _currentKeyboardState.IsKeyUp(key);
        public bool WasKeyJustPressed(Keys key) => _currentKeyboardState.IsKeyDown(key) && !_previousKeyboardState.IsKeyDown(key);
        public bool WasKeyJustReleased(Keys key) => !_currentKeyboardState.IsKeyDown(key) && _previousKeyboardState.IsKeyDown(key);

        public bool IsButtonDown(int playerIndex, Buttons button) => _currentGamePadStates[playerIndex].IsButtonDown(button);
        public bool WasButtonJustPressed(int playerIndex, Buttons button) =>
            _currentGamePadStates[playerIndex].IsButtonDown(button) &&
            !_previousGamePadStates[playerIndex].IsButtonDown(button);

        private void PublishGlobalInputEvents()
        {
            // Eventos de pausa
            if (WasKeyJustPressed(Keys.Escape))
            {
                _eventBus.Publish(new GamePausedEvent { PauseReason = "User pressed Escape" });
            }

            // Eventos de UI
            if (WasKeyJustPressed(Keys.I))
            {
                _eventBus.Publish(new UIWindowOpenedEvent { WindowName = "Inventory" });
            }

            if (WasKeyJustPressed(Keys.M))
            {
                _eventBus.Publish(new UIWindowOpenedEvent { WindowName = "Map" });
            }
        }

        public bool WasMouseButtonJustPressed(MouseButton button)
        {
            return button switch
            {
                MouseButton.Left => _currentMouseState.LeftButton == ButtonState.Pressed &&
                                   _previousMouseState.LeftButton == ButtonState.Released,
                MouseButton.Right => _currentMouseState.RightButton == ButtonState.Pressed &&
                                    _previousMouseState.RightButton == ButtonState.Released,
                MouseButton.Middle => _currentMouseState.MiddleButton == ButtonState.Pressed &&
                                     _previousMouseState.MiddleButton == ButtonState.Released,
                _ => false
            };
        }

        private void OnGamePaused(GamePausedEvent e)
        {
            _inputEnabled = false; // Pausar input de gameplay, permitir solo input de UI
        }

        private void OnGameResumed(GameResumedEvent e)
        {
            _inputEnabled = true;
        }

        public enum MouseButton
        {
            Left,
            Right,
            Middle
        }
    }

   

}
