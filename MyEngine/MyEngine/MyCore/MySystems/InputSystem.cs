using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MyEngine.MyCore.MyComponents;
using MyEngine.MyCore.MyEntities;

namespace MyEngine.MyCore.MySystems
{
    public class InputSystem
    {
        private WorldManager _world;
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

        public void Initialize(WorldManager world)
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
            var inputEntities = _world.GetEntitiesWithComponents<InputComponent, RigidbodyComponent>();

            foreach (var entity in inputEntities)
            {
                var input = entity.GetComponent<InputComponent>();
                ProcessEntityInput(entity, input, gameTime);
            }
        }


        private void ProcessEntityInput(MainEntity entity, InputComponent input, GameTime gameTime)
        {
            if (!input.IsEnabled) return;

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            // Process keyboard input
            if (input.UseKeyboard)
            {
                Vector2 vel = entity.GetComponent<RigidbodyComponent>().Velocity;
                
                Vector2 movement = Vector2.Zero;

                if (IsKeyDown(input.MoveUpKey))
                    movement.Y -= vel.Y;
                if (IsKeyDown(input.MoveDownKey))
                    movement.Y += vel.Y;
                if (IsKeyDown(input.MoveLeftKey))
                    movement.X -= vel.X;
                if (IsKeyDown(input.MoveRightKey))
                    movement.X += vel.X;

                if (movement != Vector2.Zero)
                {
                    movement.Normalize();
                    ApplyMovement(entity, movement, vel, deltaTime);
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
        public enum MouseButton
        {
            Left,
            Right,
            Middle
        }
    }
}
