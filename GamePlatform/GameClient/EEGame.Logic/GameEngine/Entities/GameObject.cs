using Microsoft.Xna.Framework;
using MonoGameLibrary.Graphics;

namespace MonoGameLibrary.Entities
{
    public class GameObject
    {
        public Sprite _sprite { get; set; }
        public AnimatedSprite _animatedSprite { get;set; }
        public Vector2 position = new(1);
        public StaticOrAnimate type { get; set; } = StaticOrAnimate.Sprite;

        public GameObject(Sprite sprite)
        {
            Add(sprite);
        }

        public GameObject(AnimatedSprite sprite)
        {
            Add(sprite);
        }

        public new StaticOrAnimate GetType()
        {
            return type;
        }

        public void Add(Sprite sprite)
        {
            type = StaticOrAnimate.Sprite;
            _sprite = sprite;
        }

        public void Add(AnimatedSprite sprite)
        {
            type = StaticOrAnimate.AnimatedSprite;
            _animatedSprite = sprite;
        }

        public Vector2 Position()
        {
            return position;
        }

        public void Position(Vector2 NewPosition)
        {
            position = NewPosition;
        }

        public float Y()
        {
            return position.Y;
        }

        public void Y(float positionY)
        {
            position.Y = positionY;
        }

        public float X()
        {
            return position.X;
        }

        public void X(float positionX)
        {
             position.X = positionX;
        }
        public void MoveRigth(float speed)
        {
            position.X += speed;
        }

        public void MoveLeft(float speed)
        {
            position.X -= speed;
        }

        public void MoveUp(float speed)
        {
            position.Y -= speed;
        }

        public void MoveDown(float speed)
        {
            position.Y += speed;
        }

    }

    public enum StaticOrAnimate
    {
        Sprite,
        AnimatedSprite
    }
}
