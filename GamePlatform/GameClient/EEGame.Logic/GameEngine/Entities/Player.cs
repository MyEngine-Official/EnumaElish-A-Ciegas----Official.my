using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;

namespace MonoGameLibrary.Entities
{
    public class Player
    {
        private Dictionary<PlayerState, AnimatedSprite> _animations = new Dictionary<PlayerState, AnimatedSprite>();
        private TextureAtlas _textureAtlas { get; set; }

        private PlayerState _currentAnimacion;

        private Vector2 _position;
        public Player(TextureAtlas atlasTexturas)
        {
            _textureAtlas = atlasTexturas;   
        }

        public void Draw(SpriteBatch batch)
        {
            _animations[_currentAnimacion].Draw(batch, Position());
        }
        public void AddAnimacion(PlayerState playerState, string nombreAnimacion)
        {
            _animations[playerState] = _textureAtlas.CreateAnimatedSprite(nombreAnimacion);
        }

        public void EditarAnimacion(PlayerState state, Vector2 scale)
        {
            AnimatedSprite animatedSprite = _animations[state];
            animatedSprite.Scale = scale;
        }

        public void ActivateAnimation(PlayerState playerState)
        {
            _currentAnimacion = playerState;
        }

        public void Update(GameTime gameTime)
        {
            _animations[_currentAnimacion].Update(gameTime);
        }

        public void AddAnimation(AnimatedSprite animated, PlayerState estado)
        {
            _animations.TryAdd(estado, animated);
        }
        public AnimatedSprite GetAnimation()
        {
            return _animations[_currentAnimacion];
        }

        public void Position(Vector2 newPosition)
        {
            _position = newPosition;
        }

        public void X(float Position_X)
        {
            _position.X = Position_X;
        }

        public float X()
        {
            return Position().X;
        }
        public void Y(float Position_Y)
        {
            _position.Y = Position_Y;
        }

        public float Y()
        {
            return Position().Y;
        }

        public void MoveRigth(float speed)
        {
            _position.X += speed;
        }

        public void MoveLeft(float speed)
        {
            _position.X -= speed;
        }

        public void MoveUp(float speed)
        {
            _position.Y -= speed;
        }

        public void MoveDown(float speed)
        {
            _position.Y += speed;
        }

        public Vector2 Position()
        {
            return _position;
        }

    }

    public enum PlayerState
    {
        CorrerDerecha,
        CorrerIzquierda,
        CorrerArriba,
        CorrerAbajo,
        Saltar,
        Ataque1,
        Ataque2,
        Ataque3,
        Transformacion
    }
}
