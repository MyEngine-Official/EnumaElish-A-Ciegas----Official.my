
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGameLibrary.Graphics;

namespace MonoGameLibrary.Events
{
    public class AccionesSprite
    {
        public readonly Dictionary<int, Rectangle> colisionesSpites = new Dictionary<int, Rectangle>();
        public AccionesSprite()
        {
            
        }

        public void Add(Sprite sprite)
        {
            Rectangle colision;
            colision = new Rectangle
            {
                X = (int)sprite.Origin.X,
                Y = (int)sprite.Origin.Y,
                Width = (int)sprite.Width,
                Height = (int)sprite.Height,
            };
            colisionesSpites[sprite.SpriteId] = colision;
        }

        public void Add(AnimatedSprite sprite)
        {
            Rectangle colision;
            colision = new Rectangle
            {
                X = (int)sprite.Origin.X,
                Y = (int)sprite.Origin.Y,
                Width = (int)sprite.Width,
                Height = (int)sprite.Height,
            };
            colisionesSpites[sprite.SpriteId] = colision;
        }

        public bool SetPosition(Sprite sprite, Vector2 posicion, HashSet<int> NoPuedeColisionar)
        {
            Rectangle newRectangle = new Rectangle(
                (int)posicion.X,
                (int)posicion.Y,
                (int)sprite.Width,
                (int)sprite.Height
            );

            foreach (var kvp in colisionesSpites)
            {
                if (NoPuedeColisionar.Contains(kvp.Key))
                {
                    if (kvp.Value.Intersects(newRectangle))
                    {
                        return true;
                    }
                }
            }

            // Si no hubo colisión, actualiza la posición del sprite en el diccionario
            sprite.Origin = posicion;
            colisionesSpites[sprite.SpriteId] = newRectangle;

            return false;
        }


        public bool SetPosition(AnimatedSprite sprite, Vector2 posicion, HashSet<int> NoPuedeColisionar)
        {
            Rectangle newRectangle = new Rectangle(
                (int)posicion.X,
                (int)posicion.Y,
                (int)sprite.Width,
                (int)sprite.Height
            );

            foreach (var kvp in colisionesSpites)
            {
                if (NoPuedeColisionar.Contains(kvp.Key))
                {
                    if (kvp.Value.Intersects(newRectangle))
                    {
                        return true;
                    }
                }
            }

            // Si no hubo colisión, actualiza la posición del sprite en el diccionario
            sprite.Origin = posicion;
            colisionesSpites[sprite.SpriteId] = newRectangle;
            return false;
        }

    }
}
