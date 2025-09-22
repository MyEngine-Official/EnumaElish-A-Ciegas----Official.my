using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MyEngine.MyCore.MyEntities;

namespace MyEngine.MyCore.MyComponents
{
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
        public int GamepadIndex { get; set; } = 0; // 0-3 for PlayerIndex
        public event Action<MainEntity> OnAction;
        public event Action<MainEntity> OnJump;

        public InputComponent(bool useMouse, bool useGamePad, bool useKeyboard, bool isEnabled, int gamepadIndex)
        {
            UseGamepad = useGamePad;
            UseKeyboard = useKeyboard;
            UseMouse = useMouse;
            IsEnabled = isEnabled;
            GamepadIndex = gamepadIndex;
        }
        public Dictionary<InputAction, Keys> KeyBindings { get; private set; } = new()
        {
            { InputAction.MoveUp, Keys.None },
            { InputAction.MoveDown, Keys.None },
            { InputAction.MoveLeft, Keys.None },
            { InputAction.MoveRight, Keys.None },
            { InputAction.Jump, Keys.None },
            { InputAction.Action, Keys.None },
            { InputAction.none, Keys.None }
        };

        public Dictionary<InputAction, Buttons> GamepadBindings { get; private set; } = new()
        {
            { InputAction.MoveUp, Buttons.None },
            { InputAction.MoveDown, Buttons.None },
            { InputAction.MoveLeft, Buttons.None },
            { InputAction.MoveRight, Buttons.None },
            { InputAction.Jump, Buttons.None },
            { InputAction.Action, Buttons.None },
            { InputAction.none, Buttons.None }
        };

        public Dictionary<MouseKeyAction, MouseAction> MouseBindings { get; private set; } = new()
        {
            { MouseKeyAction.LeftClick, MouseAction.none },
            { MouseKeyAction.RightClick, MouseAction.none },
            { MouseKeyAction.MiddleClick, MouseAction.none },
            { MouseKeyAction.Move, MouseAction.none },
            { MouseKeyAction.Scroll, MouseAction.none },
            { MouseKeyAction.none, MouseAction.none }
        };

        public void AssignKeyBinding(InputAction action, Keys key)
        {
            KeyBindings[action] = key;
        }
        public void AssignGamepadBinding(InputAction action, Buttons button)
        {
            GamepadBindings[action] = button;
        }
        public void AssignMouseBinding(MouseKeyAction action, MouseAction mouseAction)
        {
            MouseBindings[action] = mouseAction;
        }

        public void InvocarAction(MainEntity entity)
        {
            OnAction?.Invoke(entity);
        }
        public void InvocarJump(MainEntity entity)
        {
            OnJump?.Invoke(entity);
        }
    }

    public enum InputAction
    {
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight,
        Jump,
        Action,
        none
    }

    public enum MouseKeyAction
    {
        LeftClick,
        RightClick,
        MiddleClick,
        Move,
        Scroll,
        none
    }

    public enum MouseAction
    {
        Attack1,
        Attack2,
        Shield,
        none
    }

}
