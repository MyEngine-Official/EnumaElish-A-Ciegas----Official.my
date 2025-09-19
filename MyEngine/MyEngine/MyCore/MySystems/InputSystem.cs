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
    public class InputSystem: ISystem
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

                if (WasKeyJustPressed(input.KeyBindings[InputAction.MoveUp]))
                    movement.Y -= vel.Y;
                if (WasKeyJustPressed(input.KeyBindings[InputAction.MoveDown]))
                    movement.Y += vel.Y;
                if (WasKeyJustPressed(input.KeyBindings[InputAction.MoveLeft]))
                    movement.X -= vel.X;
                if (WasKeyJustPressed(input.KeyBindings[InputAction.MoveRight]))
                    movement.X += vel.X;

                if (movement != Vector2.Zero)
                {
                    entity.GetComponent<RigidbodyComponent>().SetVelocity(movement);

                    // Check action keys
                    if (WasKeyJustPressed(input.KeyBindings[InputAction.Action]))
                    {
                        input.InvocarAction(entity);
                    }

                    if (WasKeyJustPressed(input.KeyBindings[InputAction.Jump]))
                    {

                        input.InvocarJump(entity);
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


                            if (WasButtonJustPressed(input.GamepadIndex, input.GamepadBindings[InputAction.MoveUp]))
                            {
                                movement.Y -= vel.Y;
                            }

                            if (WasButtonJustPressed(input.GamepadIndex, input.GamepadBindings[InputAction.MoveUp]))
                            {
                                movement.Y += vel.Y;
                            }
                            if (WasButtonJustPressed(input.GamepadIndex, input.GamepadBindings[InputAction.MoveUp]))
                            {
                                movement.X -= vel.X;
                            }

                            if (WasButtonJustPressed(input.GamepadIndex, input.GamepadBindings[InputAction.MoveUp]))
                            {
                                movement.X += vel.X;
                            }
                        }
                    }
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
