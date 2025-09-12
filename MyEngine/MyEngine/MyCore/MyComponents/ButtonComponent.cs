using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace MyEngine.MyCore.MyComponents
{
    public class ButtonComponent
    {
        /// <summary>
        /// primer argumento es posicion, el segundo es tamano
        /// </summary>
        public Dictionary<Vector2, Vector2> TouchEvent = new Dictionary<Vector2, Vector2>();
        public List<Keys> KeyEvent = new List<Keys>();
        public List<Buttons> ButtonEvent = new List<Buttons>();

        public ButtonComponent(Keys key, Buttons button, Vector2 position, Vector2 size) 
        {
            KeyEvent.Add(key);
            ButtonEvent.Add(button);
            TouchEvent.Add(position, size);
        }
        public ButtonComponent(Keys key, Buttons button)
        {
            KeyEvent.Add(key);
            ButtonEvent.Add(button);
        }
    }
}
