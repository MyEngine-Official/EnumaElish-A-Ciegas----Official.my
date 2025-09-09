using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;

namespace MonoGameLibrary.Entities
{
    public class Button
    {
        private Dictionary<MyButtonState, Sprite> _button = new Dictionary<MyButtonState, Sprite>();
        private MyButtonState _StateActive = MyButtonState.normal;
        private Vector2 position;
        private MouseState _previousMouseState;
        public event Action Clicked;

        // ----- NEW: Text properties -----
        /// <summary>Texto opcional que se dibujará centrado dentro del botón. Si es null o vacío no se dibuja.</summary>
        public string Text { get; set; }
        /// <summary>Fuente a usar para el texto. Si es null el texto no se dibuja.</summary>
        public SpriteFont Font { get; set; }
        /// <summary>Color del texto (por defecto White).</summary>
        public Color TextColor { get; set; } = Color.White;
        /// <summary>Escala adicional para el texto (1 = sin cambio). Se combina con la escala del sprite.</summary>
        public float TextScale { get; set; } = 1f;
        /// <summary>Desplazamiento extra aplicado a la posición del texto después de centrarlo.</summary>
        public Vector2 TextOffset { get; set; } = Vector2.Zero;

        public Button()
        {
            TouchPanel.EnabledGestures = GestureType.Tap;
        }

        public void AddSprite(MyButtonState state, Sprite sprite)
        {
            _button.TryAdd(state, sprite);
        }

        public void AddFilter(Color color)
        {
            _button[_StateActive].Color = color;
        }

        /// <summary>Draw del botón. Si Text y Font están asignados, dibuja el texto centrado dentro del sprite.</summary>
        public void Draw(SpriteBatch bash)
        {
            var sprite = _button[_StateActive];
            sprite.Draw(bash, position);

            // Dibuja texto centrado (solo si hay texto y fuente)
            if (!string.IsNullOrWhiteSpace(Text) && Font != null)
            {
                // Consideramos la escala del sprite al calcular el área disponible
                var scale = sprite.Scale; // Vector2
                float scaleX = scale.X;
                float scaleY = scale.Y;

                // Escala final del texto: TextScale * promedio de las escalas X/Y del sprite (útil cuando no es uniforme)
                float finalTextScale = TextScale * ((scaleX + scaleY) / 2f);

                Vector2 measured = Font.MeasureString(Text) * finalTextScale;

                float buttonWidth = sprite.Width * scaleX;
                float buttonHeight = sprite.Height * scaleY;

                Vector2 textPosition = position + new Vector2(
                    (buttonWidth - measured.X) / 2f,
                    (buttonHeight - measured.Y) / 2f
                ) + TextOffset;

                bash.DrawString(Font, Text, textPosition, TextColor, 0f, Vector2.Zero, finalTextScale, SpriteEffects.None, 0f);
            }
        }

        public Vector2 Position()
        {
            return position;
        }

        public void Position(Vector2 vector)
        {
            position = vector;
        }

        public void Scale(Vector2 scale)
        {
            // Aplica la misma escala a los estados conocidos
            if (_button.ContainsKey(MyButtonState.normal))
                _button[MyButtonState.normal].Scale = scale;
            if (_button.ContainsKey(MyButtonState.hover))
                _button[MyButtonState.hover].Scale = scale;
        }

        // Firma nueva
        public void Update(GameTime gameTime, Vector2 mouseWorld, MouseState mouseState, Matrix inverseScaleMatrix)
        {
            var sprite = _button[_StateActive];

            // Usar la posición y la escala del sprite para obtener bounds en COORDENADAS MUNDO
            Rectangle buttonBounds = new Rectangle(
                (int)Position().X,
                (int)Position().Y,
                (int)(sprite.Width * sprite.Scale.X),
                (int)(sprite.Height * sprite.Scale.Y)
            );

            // detectar hover usando mouseWorld (ya en coordenadas mundo)
            if (buttonBounds.Contains(mouseWorld))
                _StateActive = MyButtonState.hover;
            else
                _StateActive = MyButtonState.normal;

            // detectar click: comparando el MouseState pero usando mouseWorld para la posición
            if (mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed &&
                _previousMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released &&
                buttonBounds.Contains(mouseWorld))
            {
                Clicked?.Invoke();
            }

            _previousMouseState = mouseState;

            // TOUCH: transformar la posición del touch a MUNDO antes de comparar
            TouchCollection touchState = TouchPanel.GetState();
            foreach (var touch in touchState)
            {
                if (touch.State == TouchLocationState.Pressed)
                {
                    Vector2 touchWorld = Vector2.Transform(touch.Position, inverseScaleMatrix);
                    if (buttonBounds.Contains(touchWorld))
                        Clicked?.Invoke();
                }
            }
        }

        //public void Update(GameTime gameTime)
        //{
        //    var sprite = _button[_StateActive];

        //    // Área del botón (rectángulo para colisión)
        //    Rectangle buttonBounds = new Rectangle(
        //        (int)Position().X,
        //        (int)Position().Y,
        //        (int)(sprite.Width * sprite.Scale.X),
        //        (int)(sprite.Height * sprite.Scale.Y)
        //    );

        //    // ===== DETECCIÓN MOUSE =====
        //    var mouseState = Mouse.GetState();
        //    if (buttonBounds.Contains(mouseState.Position))
        //        _StateActive = ButtonState.hover;
        //    else
        //        _StateActive = ButtonState.normal;

        //    if (mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed &&
        //        _previousMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released &&
        //        buttonBounds.Contains(mouseState.Position))
        //    {
        //        Clicked?.Invoke();
        //    }
        //    _previousMouseState = mouseState;

        //    // ===== DETECCIÓN TOUCH =====
        //    TouchCollection touchState = TouchPanel.GetState();
        //    foreach (var touch in touchState)
        //    {
        //        if (touch.State == TouchLocationState.Pressed &&
        //            buttonBounds.Contains(touch.Position))
        //        {
        //            Clicked?.Invoke();
        //        }
        //    }
        //}
    }

    public enum MyButtonState
    {
        normal,
        hover
    }
}
