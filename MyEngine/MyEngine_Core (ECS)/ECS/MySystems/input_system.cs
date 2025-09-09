using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MyEngine_Core.ECS.MyComponents;
using MyEngine_Core.ECS.MyEntities;

namespace MyEngine_Core.ECS.MySystems
{
    public class InputSystem : ISystem
    {
        private World _world;
        private KeyboardState _previousKeyboardState;
        private KeyboardState _currentKeyboardState;
        private MouseState _previousMouseState;
        private MouseState _currentMouseState;
        private GamePadState[] _previousGamePadStates;
        private GamePadState[] _currentGamePadStates;

        public InputSystem()
        {
            _previousGamePadStates = new GamePadState[4];
            _currentGamePadStates = new GamePadState[4];
        }

        public void Initialize(World world)
        {
            _world = world;
        }

        /// <summary>
        /// Updates input states
        /// </summary>
        public void Update(GameTime gameTime)
        {
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
            var inputEntities = _world.GetEntitiesWithComponents<InputComponent>();
            
            foreach (var entity in inputEntities)
            {
                var input = entity.GetComponent<InputComponent>();
                ProcessEntityInput(entity, input, gameTime);
            }
        }

        private void ProcessEntityInput(EntidadPadre entity, InputComponent input, GameTime gameTime)
        {
            if (!input.IsEnabled) return;

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Process keyboard input
            if (input.UseKeyboard)
            {
                Vector2 movement = Vector2.Zero;

                if (IsKeyDown(input.MoveUpKey))
                    movement.Y -= 1;
                if (IsKeyDown(input.MoveDownKey))
                    movement.Y += 1;
                if (IsKeyDown(input.MoveLeftKey))
                    movement.X -= 1;
                if (IsKeyDown(input.MoveRightKey))
                    movement.X += 1;

                if (movement != Vector2.Zero)
                {
                    movement.Normalize();
                    ApplyMovement(entity, movement, input.MoveSpeed, deltaTime);
                }

                // Check action keys
                if (WasKeyJustPressed(input.ActionKey))
                {
                    input.OnAction?.Invoke(entity);
                }

                if (WasKeyJustPressed(input.JumpKey))
                {
                    input.OnJump?.Invoke(entity);
                }
            }

            // Process gamepad input
            if (input.UseGamepad && input.GamepadIndex < 4)
            {
                var gamePadState = _currentGamePadStates[input.GamepadIndex];
                
                if (gamePadState.IsConnected)
                {
                    Vector2 leftStick = gamePadState.ThumbSticks.Left;
                    leftStick.Y *= -1; // Invert Y axis
                    
                    if (leftStick.LengthSquared() > 0.1f * 0.1f) // Dead zone
                    {
                        ApplyMovement(entity, leftStick, input.MoveSpeed, deltaTime);
                    }

                    if (WasButtonJustPressed(input.GamepadIndex, Buttons.A))
                    {
                        input.OnJump?.Invoke(entity);
                    }

                    if (WasButtonJustPressed(input.GamepadIndex, Buttons.X))
                    {
                        input.OnAction?.Invoke(entity);
                    }
                }
            }

            // Process mouse input
            if (input.UseMouse && entity.HasComponent<TransformComponent>())
            {
                var transform = entity.GetComponent<TransformComponent>();
                input.MousePosition = new Vector2(_currentMouseState.X, _currentMouseState.Y);
                
                if (input.FollowMouse)
                {
                    Vector2 direction = input.MousePosition - transform.Position;
                    if (direction.LengthSquared() > 1f)
                    {
                        direction.Normalize();
                        ApplyMovement(entity, direction, input.MoveSpeed, deltaTime);
                    }
                }

                if (WasMouseButtonJustPressed(MouseButton.Left))
                {
                    input.OnMouseClick?.Invoke(entity, input.MousePosition);
                }
            }
        }

        private void ApplyMovement(EntidadPadre entity, Vector2 direction, float speed, float deltaTime)
        {
            if (entity.HasComponent<RigidbodyComponent>())
            {
                // Apply force to rigidbody
                var rigidbody = entity.GetComponent<RigidbodyComponent>();
                rigidbody.Velocity += direction * speed * deltaTime;
            }
            else if (entity.HasComponent<TransformComponent>())
            {
                // Direct position update
                var transform = entity.GetComponent<TransformComponent>();
                transform.Position += direction * speed * deltaTime;
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

        public Vector2 GetMousePosition() => new Vector2(_currentMouseState.X, _currentMouseState.Y);
        public Vector2 GetMouseDelta() => new Vector2(
            _currentMouseState.X - _previousMouseState.X, 
            _currentMouseState.Y - _previousMouseState.Y);
    }

    public enum MouseButton
    {
        Left,
        Right,
        Middle
    }
}

namespace MyEngine_Core.ECS.MyComponents
{
    /// <summary>
    /// Component for handling player input
    /// </summary>
    public class InputComponent
    {
        // Control flags
        public bool IsEnabled { get; set; } = true;
        public bool UseKeyboard { get; set; } = true;
        public bool UseGamepad { get; set; } = true;
        public bool UseMouse { get; set; } = false;
        public int GamepadIndex { get; set; } = 0;

        // Movement
        public float MoveSpeed { get; set; } = 200f;
        public bool FollowMouse { get; set; } = false;

        // Keyboard mappings
        public Keys MoveUpKey { get; set; } = Keys.W;
        public Keys MoveDownKey { get; set; } = Keys.S;
        public Keys MoveLeftKey { get; set; } = Keys.A;
        public Keys MoveRightKey { get; set; } = Keys.D;
        public Keys JumpKey { get; set; } = Keys.Space;
        public Keys ActionKey { get; set; } = Keys.E;

        // Mouse state
        public Vector2 MousePosition { get; set; }

        // Action callbacks
        public Action<EntidadPadre> OnJump { get; set; }
        public Action<EntidadPadre> OnAction { get; set; }
        public Action<EntidadPadre, Vector2> OnMouseClick { get; set; }
    }
}