using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.Graphics
{
    public class SaveAndDrawAll
    {
        private readonly Dictionary<Sprite, Vector2> _sprites = [];
        private readonly Dictionary<AnimatedSprite, Vector2> _animatedSprites = [];
        private readonly List<int> _spriteIds = [];
        private readonly List<int> _animatedSpriteIds = [];

        public void Add(Sprite sprite, Vector2 posicion)
        {
            if (sprite == null) return;
            sprite.SpriteId = (_sprites.Count() + _animatedSprites.Count()) + 1;
            _sprites[sprite] = posicion;
        }

        public void Add(AnimatedSprite animatedSprite, Vector2 posicion)
        {
            if (animatedSprite == null) return;
            animatedSprite.SpriteId = (_sprites.Count() + _animatedSprites.Count()) + 1;
            _animatedSprites[animatedSprite] = posicion;
        }

        public void DrawAll(SpriteBatch batch, GameTime gameTime)
        {

            foreach (var kvp in _sprites)
            {
                if(!_spriteIds.Contains(kvp.Key.SpriteId))
                kvp.Key.Draw(batch, kvp.Value);
            }

            foreach (var kvp in _animatedSprites)
            {
                kvp.Key.Update(gameTime);
                if (!_animatedSpriteIds.Contains(kvp.Key.SpriteId))
                    kvp.Key.Draw(batch, kvp.Value);
            }
        }

        public void SetPosition(Sprite sprite, Vector2 nuevaPosicion)
        {
            if (_sprites.ContainsKey(sprite))
                _sprites[sprite] = nuevaPosicion;
        }
        public void SetPosition(AnimatedSprite animatedSprite, Vector2 nuevaPosicion)
        {
            if (_animatedSprites.ContainsKey(animatedSprite))
                _animatedSprites[animatedSprite] = nuevaPosicion;
        }


        public void DrawAllExcept(SpriteBatch batch, HashSet<Sprite> excludeSprites)
        {
            foreach (var kvp in _sprites)
            {
                if (!excludeSprites.Contains(kvp.Key) && !_spriteIds.Contains(kvp.Key.SpriteId))
                    kvp.Key.Draw(batch, kvp.Value);
            }
        }

        public void DrawAllExcept(SpriteBatch batch, HashSet<AnimatedSprite> excludeSprites, GameTime gameTime)
        {
            foreach (var kvp in _animatedSprites)
            {
                kvp.Key.Update(gameTime);
                if (!excludeSprites.Contains(kvp.Key) && !_animatedSpriteIds.Contains(kvp.Key.SpriteId))
                    kvp.Key.Draw(batch, kvp.Value);
            }
        }

        public void Delete(Sprite sprite)
        {
            _sprites.Remove(sprite);
        }

        public void Delete(AnimatedSprite sprite)
        {
            _animatedSprites.Remove(sprite);
        }

        public void OcultarHastaNuevoAviso(Sprite sprite)
        {
            _spriteIds.Add(sprite.SpriteId);
        }

        public void OcultarHastaNuevoAviso(AnimatedSprite spriteAnimado)
        {
            _spriteIds.Add(spriteAnimado.SpriteId);
        }

        public void MostrarSprite(Sprite sprite)
        {
            try
            {
                _spriteIds.Remove(sprite.SpriteId);
            }
            catch { }
            
        }

        public void MostrarSprite(AnimatedSprite spriteAnimado)
        {
            try
            {
                _animatedSpriteIds.Remove(spriteAnimado.SpriteId);
            }
            catch { }
        }
    }
}
