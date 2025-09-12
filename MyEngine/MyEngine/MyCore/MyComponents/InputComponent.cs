using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

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

        public Keys MoveUpKey { get; set; }
        public Keys MoveDownKey { get; set; }
        public Keys MoveLeftKey { get; set; }
        public Keys MoveRightKey { get; set; }
        public Keys ActionKey { get; set; }
        public void SetMovements(Keys up, Keys right, Keys left, Keys down)
        {
            MoveDownKey = down;
            MoveLeftKey = left;
            MoveRightKey = right;
            MoveUpKey = up;
        }
    }
}
