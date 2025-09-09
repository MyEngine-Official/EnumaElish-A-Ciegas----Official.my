using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Entities;
using MonoGameLibrary.Graphics;

namespace MonoGameLibrary.Events
{
    /// <summary>
    /// Gestiona dibujado, animación, posición y colisiones de sprites y animated sprites.
    /// </summary>
    public class SpriteManager
    {

        private readonly Dictionary<Sprite, Vector2> _sprites = new();
        private readonly Dictionary<AnimatedSprite, Vector2> _animatedSprites = new();
        private readonly Dictionary<Button, Vector2> _buttons = new Dictionary<Button, Vector2>();
        private readonly Dictionary<int, Rectangle> _colisionesSpites = new Dictionary<int, Rectangle>();
        private readonly List<int> _hiddenSpriteIds = new();
        private readonly List<Rectangle> _colisionesMapa = new List<Rectangle>();
        private Player _player;
        private Rectangle _playerColition = new Rectangle();
        public List<Sprite> colisionesJugador = new List<Sprite>();
        public SpriteManager()
        {
            
        }

        // Añade este método dentro de la clase SpriteManager
        public void DebugDrawMapCollisions(SpriteBatch batch, Texture2D pixel, Color color, int outlineThickness = 1)
        {
            if (pixel == null) return;

            batch.Draw(pixel, _playerColition, color);

            foreach (Rectangle r in _colisionesMapa)
            {
                // Top
                batch.Draw(pixel, new Rectangle(r.X, r.Y, r.Width, outlineThickness), color);
                // Bottom
                batch.Draw(pixel, new Rectangle(r.X, r.Y + r.Height - outlineThickness, r.Width, outlineThickness), color);
                // Left
                batch.Draw(pixel, new Rectangle(r.X, r.Y, outlineThickness, r.Height), color);
                // Right
                batch.Draw(pixel, new Rectangle(r.X + r.Width - outlineThickness, r.Y, outlineThickness, r.Height), color);
            }
        }

        #region Añadir Sprites

        public void AddPlayer(Player player)
        {
            _player = player;

            _playerColition = new Rectangle
            {
                Height = (int)player.GetAnimation().Height,
                Width = (int)player.GetAnimation().Width,
                X = (int)player.Position().X,
                Y = (int)player.Position().Y
            };
        }

        // Dentro de la clase SpriteManager (sólo muestro los métodos cambiados / añadidos)
        // Asegúrate de reemplazar Add(...), SetPosition(...) y añadir ComputeCollisionRect y DebugDraw.

        private Rectangle ComputeCollisionRect(Sprite sprite, Vector2 position)
        {
            // Tener en cuenta Origin y Scale para calcular el rect en coordenadas MUNDO
            Vector2 origin = sprite.Origin; // asumimos propiedad existente
            int x = (int)(position.X - origin.X * sprite.Scale.X);
            int y = (int)(position.Y - origin.Y * sprite.Scale.Y);
            int width = (int)(sprite.Width );
            int height = (int)(sprite.Height);
            return new Rectangle(x, y, width, height);
        }

        public void Add(Sprite sprite, Vector2 posicion)
        {
            sprite.SpriteId = (_sprites.Count + _animatedSprites.Count) + 1;
            _sprites.Add(sprite, posicion);

            Rectangle spriteColision = ComputeCollisionRect(sprite, posicion);
            _colisionesSpites.Add(sprite.SpriteId, spriteColision);
        }

        public void Add(Button button, Vector2 posicion)
        {
            button.Position(posicion);
            _buttons.Add(button, posicion);
        }

        public void Add(AnimatedSprite sprite, Vector2 posicion)
        {
            sprite.SpriteId = (_sprites.Count + _animatedSprites.Count) + 1;
            _animatedSprites.Add(sprite, posicion);

            Rectangle spriteColision = ComputeCollisionRect(sprite, posicion);
            _colisionesSpites.Add(sprite.SpriteId, spriteColision);
        }

       

        // ---------------------------
        // Debug: dibujar rects de colisión (llámalo desde tu Draw con la spriteBatch ya Begin(transformMatrix:_scaleMatrix))
        // ---------------------------
        public void DebugDraw(SpriteBatch batch, Texture2D pixel, int outlineThickness = 2)
        {
            if (pixel == null) return;

            foreach (var kv in _colisionesSpites)
            {
                Rectangle r = kv.Value;
                // Top
                batch.Draw(pixel, new Rectangle(r.X, r.Y, r.Width, outlineThickness), Color.Red * 0.8f);
                // Bottom
                batch.Draw(pixel, new Rectangle(r.X, r.Y + r.Height - outlineThickness, r.Width, outlineThickness), Color.Red * 0.8f);
                // Left
                batch.Draw(pixel, new Rectangle(r.X, r.Y, outlineThickness, r.Height), Color.Red * 0.8f);
                // Right
                batch.Draw(pixel, new Rectangle(r.X + r.Width - outlineThickness, r.Y, outlineThickness, r.Height), Color.Red * 0.8f);
            }


        }

        #endregion

        #region Gestionar Visibilidad Sprites
        public void OcultarSprites(Sprite sprite)
        {
            _hiddenSpriteIds.Add(sprite.SpriteId);
        }
        public void MostrarSprite(Sprite sprite)
        {
            _hiddenSpriteIds.Remove(sprite.SpriteId);
        }
        #endregion

        #region actualizar Posicion

        public bool SetPlayerPosition(Vector2 NewPosition)
        {
            Rectangle colisionPropia = new Rectangle(
               (int)NewPosition.X,
               (int)NewPosition.Y,
               (int)_player.GetAnimation().Width,
               (int)_player.GetAnimation().Height
           );

            bool colision = false;

           
                // En SpriteManager.SetPlayerPosition
                foreach (var mapa in _colisionesMapa)
                {
                    if (colisionPropia.Intersects(mapa))
                    {
                        colision = true;
                        break; // ¡Usa break para salir en cuanto encuentres una colisión!
                    }
                
                }

            foreach (var s in _sprites)
            {
                if (colision == true) continue;
                if (!colisionesJugador.Any(a => a.SpriteId == s.Key.SpriteId)) continue;

                _colisionesSpites.TryGetValue(s.Key.SpriteId, out Rectangle colisionOtro);
                colision = colisionPropia.Intersects(colisionOtro);
            }

            if (colision == true) return colision;

            foreach (var s in _animatedSprites)
            {
                if (colision == true) continue;
                if (!colisionesJugador.Any(a => a.SpriteId == s.Key.SpriteId)) continue;

                _colisionesSpites.TryGetValue(s.Key.SpriteId, out Rectangle colisionOtro);
                colision = colisionPropia.Intersects(colisionOtro);
            }

            if (!colision)
            {
                _player.Position(NewPosition);
                _playerColition = colisionPropia;
            }

            return colision;

        }

        public void SetPosicion(Button button, Vector2 posicion)
        {
            button.Position(posicion);
            _buttons[button] = posicion;
        }

        // Dentro de SpriteManager.cs

        // MODIFICA ESTE MÉTODO
        public void AddMapColisions(Rectangle[] colisiones, Vector2 scale)
        {
            _colisionesMapa.Clear(); // Limpiamos por si se llama más de una vez
            foreach (var rect in colisiones)
            {
                // VALIDACIÓN MEJORADA:
                // Ignoramos cualquier rectángulo que no tenga área (ancho o alto sea 0).
                // Esto cubre el caso de Rectangle.Empty y otros rectángulos degenerados.
                if (rect.Width == 0 || rect.Height == 0)
                {
                    continue;
                }

                // Creamos un nuevo rectángulo con la posición y el tamaño escalados
                var rectEscalado = new Rectangle(
                    (int)(rect.X * scale.X),
                    (int)(rect.Y * scale.Y),
                    (int)(rect.Width * scale.X),
                    (int)(rect.Height * scale.Y)
                );
                _colisionesMapa.Add(rectEscalado);
            }
        }
        public bool SetPosition(Sprite sprite, Vector2 NewPosition, HashSet<int> colisionar)
        {
            Rectangle colisionPropia = ComputeCollisionRect(sprite, NewPosition);

            bool colision = false;

            foreach (var s in _sprites)
            {
                if (s.Key.SpriteId == sprite.SpriteId || colision == true) continue;
                if (!colisionar.Contains(s.Key.SpriteId)) continue;

                _colisionesSpites.TryGetValue(s.Key.SpriteId, out Rectangle colisionOtro);
                colision = colisionPropia.Intersects(colisionOtro);
            }

            foreach (var s in _animatedSprites)
            {
                if (s.Key.SpriteId == sprite.SpriteId || colision == true) continue;
                if (!colisionar.Contains(s.Key.SpriteId)) continue;

                _colisionesSpites.TryGetValue(s.Key.SpriteId, out Rectangle colisionOtro);
                colision = colisionPropia.Intersects(colisionOtro);
            }

            if (!colision)
            {
                _sprites[sprite] = NewPosition;
                _colisionesSpites[sprite.SpriteId] = colisionPropia;
            }

            return colision;
        }

        public bool SetPosition(AnimatedSprite sprite, Vector2 NewPosition, HashSet<int> colisionar)
        {
            Rectangle colisionPropia = ComputeCollisionRect(sprite, NewPosition);

            bool colision = false;

            foreach (var s in _sprites)
            {
                if (s.Key.SpriteId == sprite.SpriteId || colision == true) continue;
                if (!colisionar.Contains(s.Key.SpriteId)) continue;

                _colisionesSpites.TryGetValue(s.Key.SpriteId, out Rectangle colisionOtro);
                colision = colisionPropia.Intersects(colisionOtro);
            }

            foreach (var s in _animatedSprites)
            {
                if (s.Key.SpriteId == sprite.SpriteId || colision == true) continue;
                if (!colisionar.Contains(s.Key.SpriteId)) continue;

                _colisionesSpites.TryGetValue(s.Key.SpriteId, out Rectangle colisionOtro);
                colision = colisionPropia.Intersects(colisionOtro);
            }

            if (!colision)
            {
                _animatedSprites[sprite] = NewPosition;
                _colisionesSpites[sprite.SpriteId] = colisionPropia;
            }

            return colision;
        }
        #endregion

        #region Dibujar

        public void Draw(SpriteBatch batch)
        {

            if (_player != null) 
            {
                _player.Draw(batch);
            }

            foreach (var kvp in _sprites)
                {
                    if (!_hiddenSpriteIds.Contains(kvp.Key.SpriteId))
                        kvp.Key.Draw(batch, kvp.Value);
                }

            foreach (var kvp in _animatedSprites)
            {
                if (!_hiddenSpriteIds.Contains(kvp.Key.SpriteId))
                    kvp.Key.Draw(batch, kvp.Value);
            }

            foreach(var kvp in _buttons)
            {
                kvp.Key.Draw(batch);
            }

        }

        public void Draw(SpriteBatch batch, Sprite noDibujar)
        {

            if (_player != null)
            {
                _player.Draw(batch);
            }

            foreach (var kvp in _sprites)
            {
                if(kvp.Key.SpriteId == noDibujar.SpriteId) continue;
                if (!_hiddenSpriteIds.Contains(kvp.Key.SpriteId))
                    kvp.Key.Draw(batch, kvp.Value);
            }

            foreach (var kvp in _animatedSprites)
            {
                if (kvp.Key.SpriteId == noDibujar.SpriteId) continue;

                if (!_hiddenSpriteIds.Contains(kvp.Key.SpriteId))
                    kvp.Key.Draw(batch, kvp.Value);
            }

            foreach (var kvp in _buttons)
            {
                kvp.Key.Draw(batch);
            }

        }

        public void SoloDrawSprite(Sprite sprite, SpriteBatch spriteBatch)
        {
            var s = _sprites.FirstOrDefault(s => s.Key.SpriteId == sprite.SpriteId);

            if(s.Key != null)
            s.Key.Draw(spriteBatch, s.Value);

        }

        public void SoloDrawAnimatedSprite(AnimatedSprite sprite, SpriteBatch spriteBatch)
        {
            var s = _animatedSprites.FirstOrDefault(s => s.Key.SpriteId == sprite.SpriteId);

            if (s.Key != null)
                s.Key.Draw(spriteBatch, s.Value);

        }

        public void UpdateAnimated(GameTime gameTime)
        {
            if (_player != null)
            {
                _player.Update(gameTime);
            }
            foreach (var kvp in _animatedSprites)
                {
                    kvp.Key.Update(gameTime);
                }
        }

        public void UpdateButtons(GameTime gameTime, Vector2 mouseWorld, MouseState mouseState, Matrix _scaleMatrix)
        {
            foreach (var kvp in _buttons)
            {
                kvp.Key.Update(gameTime, mouseWorld, mouseState, Matrix.Invert(_scaleMatrix));
            }
        }

        #endregion

        #region remover
        public void Remove(AnimatedSprite sprite)
        {
            _animatedSprites.Remove(sprite);
            _colisionesSpites.Remove(sprite.SpriteId);
            _hiddenSpriteIds.Remove(sprite.SpriteId);
        }

        public void RemoveAll() 
        {
            _sprites.Clear();
            _animatedSprites.Clear();
            _colisionesSpites.Clear();
            _hiddenSpriteIds.Clear();
        }
        #endregion

    }
}
