using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MyEngine_Core.ECS.MyEntities;

namespace MyEngine_Core.ECS.MyComponents
{
    /// <summary>
    /// Component for handling entity input
    /// </summary>
    public class InputComponent
    {
        // Control flags
        /// <summary>
        /// Is input processing enabled for this entity
        /// </summary>
        public bool IsEnabled { get; set; } = true;
        
        /// <summary>
        /// Should keyboard input be processed
        /// </summary>
        public bool UseKeyboard { get; set; } = true;
        
        /// <summary>
        /// Should gamepad input be processed
        /// </summary>
        public bool UseGamepad { get; set; } = true;
        
        /// <summary>
        /// Should mouse input be processed
        /// </summary>
        public bool UseMouse { get; set; } = false;
        
        /// <summary>
        /// Index of the gamepad to use (0-3)
        /// </summary>
        public int GamepadIndex { get; set; } = 0;

        // Movement settings
        /// <summary>
        /// Movement speed in pixels per second
        /// </summary>
        public float MoveSpeed { get; set; } = 200f;
        
        /// <summary>
        /// Acceleration rate for smooth movement
        /// </summary>
        public float Acceleration { get; set; } = 1000f;
        
        /// <summary>
        /// Should the entity follow the mouse cursor
        /// </summary>
        public bool FollowMouse { get; set; } = false;
        
        /// <summary>
        /// Dead zone for analog stick input
        /// </summary>
        public float DeadZone { get; set; } = 0.1f;

        // Keyboard mappings
        public Keys MoveUpKey { get; set; } = Keys.W;
        public Keys MoveDownKey { get; set; } = Keys.S;
        public Keys MoveLeftKey { get; set; } = Keys.A;
        public Keys MoveRightKey { get; set; } = Keys.D;
        public Keys JumpKey { get; set; } = Keys.Space;
        public Keys ActionKey { get; set; } = Keys.E;
        public Keys InteractKey { get; set; } = Keys.F;
        public Keys SprintKey { get; set; } = Keys.LeftShift;
        public Keys CrouchKey { get; set; } = Keys.LeftControl;

        // Gamepad button mappings
        public Buttons JumpButton { get; set; } = Buttons.A;
        public Buttons ActionButton { get; set; } = Buttons.X;
        public Buttons InteractButton { get; set; } = Buttons.Y;
        public Buttons SprintButton { get; set; } = Buttons.RightTrigger;
        public Buttons CrouchButton { get; set; } = Buttons.LeftTrigger;

        // State tracking
        /// <summary>
        /// Current mouse position in world coordinates
        /// </summary>
        public Vector2 MousePosition { get; set; }
        
        /// <summary>
        /// Current movement input vector
        /// </summary>
        public Vector2 MovementInput { get; set; }
        
        /// <summary>
        /// Is the sprint modifier active
        /// </summary>
        public bool IsSprinting { get; set; }
        
        /// <summary>
        /// Is the crouch modifier active
        /// </summary>
        public bool IsCrouching { get; set; }

        // Action callbacks
        /// <summary>
        /// Called when jump input is pressed
        /// </summary>
        public Action<EntidadPadre> OnJump { get; set; }
        
        /// <summary>
        /// Called when action input is pressed
        /// </summary>
        public Action<EntidadPadre> OnAction { get; set; }
        
        /// <summary>
        /// Called when interact input is pressed
        /// </summary>
        public Action<EntidadPadre> OnInteract { get; set; }
        
        /// <summary>
        /// Called when mouse is clicked
        /// </summary>
        public Action<EntidadPadre, Vector2> OnMouseClick { get; set; }
        
        /// <summary>
        /// Called when mouse right button is clicked
        /// </summary>
        public Action<EntidadPadre, Vector2> OnMouseRightClick { get; set; }
        
        /// <summary>
        /// Called every frame with movement input
        /// </summary>
        public Action<EntidadPadre, Vector2> OnMove { get; set; }
        
        /// <summary>
        /// Called when sprint state changes
        /// </summary>
        public Action<EntidadPadre, bool> OnSprintChanged { get; set; }
        
        /// <summary>
        /// Called when crouch state changes
        /// </summary>
        public Action<EntidadPadre, bool> OnCrouchChanged { get; set; }

        /// <summary>
        /// Creates a new input component with default settings
        /// </summary>
        public InputComponent()
        {
        }

        /// <summary>
        /// Creates a new input component with custom move speed
        /// </summary>
        public InputComponent(float moveSpeed)
        {
            MoveSpeed = moveSpeed;
        }
        
        /// <summary>
        /// Resets all input states
        /// </summary>
        public void Reset()
        {
            MovementInput = Vector2.Zero;
            IsSprinting = false;
            IsCrouching = false;
        }
        
        /// <summary>
        /// Gets the current effective move speed considering modifiers
        /// </summary>
        public float GetEffectiveMoveSpeed()
        {
            if (IsCrouching)
                return MoveSpeed * 0.5f;
            if (IsSprinting)
                return MoveSpeed * 1.5f;
            return MoveSpeed;
        }
    }
}
