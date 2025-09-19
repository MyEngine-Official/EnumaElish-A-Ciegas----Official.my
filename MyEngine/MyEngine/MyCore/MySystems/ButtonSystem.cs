using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using MyEngine.MyCore.MyComponents;

namespace MyEngine.MyCore.MySystems
{
    public class ButtonSystem : ISystem
    {
        private WorldManager _world;

        public void Initialize(WorldManager world)
        {
            _world = world;
        }

        public event Action<Keys, int> KeyPressed;
        public event Action<Keys, int> KeyReleased;
        public event Action<Buttons, int> ButtonPressed;
        public event Action<Buttons, int> ButtonReleased;
        public event Action<Vector2, int> TouchPressed;
        public event Action<Vector2, int> TouchReleased;

        private List<Buttons> buttonsPressed = new List<Buttons>();
        private List<Keys> keysPressed = new List<Keys>();
        private Dictionary<Vector2, Vector2> touchPressed = new Dictionary<Vector2, Vector2>();
        /// <summary>
        /// Updates all buttons components
        /// </summary>
        public void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            TouchLocation TouchState = new TouchLocation();

            var ButtonEntities = _world.GetEntitiesWithComponents<ButtonComponent>();

            foreach (var buttonComponent in ButtonEntities)
            {

               ButtonComponent btn = buttonComponent.GetComponent<ButtonComponent>();

                for (int i = 0; i < 5; i++)
                {
                    GamePadState mando = GamePad.GetState(i);
                    if (mando.IsConnected)
                    {
                        foreach (var button in btn.ButtonEvent)
                        {
                            if (mando.IsButtonDown(button))
                            {
                                if (!buttonsPressed.Contains(button))
                                {
                                    ButtonPressed.Invoke(button, buttonComponent.Id);
                                    buttonsPressed.Add(button);
                                }
                            }
                            else
                            {
                                if (buttonsPressed.Contains(button))
                                {
                                    buttonsPressed.Remove(button);
                                    ButtonReleased.Invoke(button, buttonComponent.Id);
                                }
                            }
                        }
                    }
                }

                foreach (Keys key in btn.KeyEvent)
                {
                    keyboardState = Keyboard.GetState();

                    if (keyboardState.GetPressedKeys().Contains(key))
                    {
                        if (!keysPressed.Contains(key))
                        {
                            KeyPressed.Invoke(key, buttonComponent.Id);
                            keysPressed.Add(key);
                        }
                    }
                    else
                    {
                        if (keysPressed.Contains(key))
                        {
                            keysPressed.Remove(key);
                            KeyReleased.Invoke(key, buttonComponent.Id);
                        }
                    }
                }

                foreach (var location in btn.TouchEvent)
                {
                    Vector2 center = location.Key;    // Centro del rectángulo
                    Vector2 size = location.Value;    // Ancho y alto

                    float left = center.X - size.X / 2f;
                    float top = center.Y - size.Y / 2f;

                    Rectangle rect = new Rectangle(
                        (int)left,
                        (int)top,
                        (int)size.X,
                        (int)size.Y
                    );

                    if (rect.Contains(TouchState.Position))
                    {
                        if (!touchPressed.Contains(location))
                        {
                            TouchPressed.Invoke(location.Key, buttonComponent.Id);
                            touchPressed.Add(location.Key, location.Value);
                        }
                    }
                    else
                    {
                        if (touchPressed.Contains(location))
                        {
                            TouchReleased.Invoke(location.Key, buttonComponent.Id);
                            touchPressed.Remove(location.Key);
                        }
                    }
                }
            }
        }

    }
}
