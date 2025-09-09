using System;
using System.Linq;
using System.Reflection.Metadata;
using EnumaElish.Logic.Content.SceneManager;
using EnumaElish.UI.Content.GameUI.GameTests.OnlineDev;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameLibrary.Entities;
using MonoGameLibrary.Events;
using MonoGameLibrary.Graphics;

namespace EnumaElish.UI.Content.GameUI.GameTests.OnlineDev
{
    public class plantilla_prueba_1 : GameScene
    {
        private Texture2D _pixelBlanco;

        private TextureAtlas _textureAtlas;
        private Tilemap _mapa;
        private Collisions colisiones;
        private Player _player;

        public plantilla_prueba_1()
        {
            
        }
        public override void Initialize()
        {
            // Dentro del método Initialize(), añade estas líneas para crear la textura
            _pixelBlanco = new Texture2D(Core.GraphicsDevice, 1, 1);
            _pixelBlanco.SetData(new[] { Color.White });
            _textureAtlas = TextureAtlas.FromFile(Content, "utilidades/buttons/textureAtlasBotones");
            _CursorAtlas = TextureAtlas.FromFile(Content, "utilidades/cursor/cursor_atlas.xml");
            CursorGameObject = new GameObject(_CursorAtlas.CreateSprite("mainC"));
            colisiones = Collisions.FromFile(Content, "utilidades/pruebas_testing/capa_colisions.xml");
            _mapa = Tilemap.FromFile(Content, "utilidades/pruebas_testing/capa_mapa.xml");
            //_mapa.Scale = new Vector2(0.2f);

            _player = new Player(_textureAtlas);
            _player.AddAnimacion(PlayerState.CorrerArriba, "main");
            _player.AddAnimacion(PlayerState.Ataque1, "main");
            _player.AddAnimacion(PlayerState.Ataque2, "main");
            _player.AddAnimacion(PlayerState.Ataque3, "main");
            _player.AddAnimacion(PlayerState.Transformacion, "main");
            _player.AddAnimacion(PlayerState.Saltar, "main");
            _player.AddAnimacion(PlayerState.CorrerIzquierda, "main");
            _player.AddAnimacion(PlayerState.CorrerDerecha, "main");
            _player.AddAnimacion(PlayerState.CorrerAbajo, "main");
            _player.ActivateAnimation(PlayerState.CorrerArriba);
            _player.Position(new Vector2(500, 500));
            SpriteManager.AddPlayer(_player);

            base.Initialize();
        }
        // Dentro de plantilla_prueba_1.cs

        public override void LoadContent()
        {
            var c = colisiones.GetCollisionRects();

            // ---- MODIFICA ESTA LÍNEA ----
            // Ahora pasamos también la escala del mapa
            SpriteManager.AddMapColisions(c, _mapa.Scale);

            BaseLoadContent();
            base.LoadContent();
        }
        public override void Update(GameTime gameTime)
        {
            InputManager.Keyboard.Update();
            _player.Update(gameTime);
            BaseUpdate(gameTime);
            moverPersonaje();
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            Core.GraphicsDevice.Clear(Color.RoyalBlue);
            var brocha = Core.SpriteBatch;
            brocha.Begin(transformMatrix : _scaleMatrix);

            _mapa.Draw(brocha);
            SpriteManager.DebugDraw(brocha, _pixelBlanco);

            // ---- AÑADE ESTA LÍNEA AQUÍ ----
            // Dibuja las colisiones del mapa en un color visible, como rojo.
            SpriteManager.DebugDrawMapCollisions(brocha, _pixelBlanco, Color.Red * 0.7f);
            // -----------------------------

            brocha.End();

            BaseDraw();
            base.Draw(gameTime);
        }

        private void moverPersonaje()
        {
            var speed = 3f;
            var estado = InputManager.Keyboard.CurrentState;
            Vector2 proximaPosicion = _player.Position(); // Empezamos con la posición actual

            // Calculamos la posición a la que el jugador quiere moverse
            if (estado.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.W))
            {
                proximaPosicion.Y -= speed;
            }
            else if (estado.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.S))
            {
                proximaPosicion.Y += speed;
            }
            else if (estado.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.A))
            {
                proximaPosicion.X -= speed;
            }
            else if (estado.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D))
            {
                proximaPosicion.X += speed;
            }

            // Solo si ha habido un intento de movimiento (la posición ha cambiado)
            if (proximaPosicion != _player.Position())
            {
                // Enviamos la POSICIÓN FUTURA para que SpriteManager la valide.
                // El método SetPlayerPosition ya se encarga de mover al jugador si no hay colisión.
                // No necesitamos hacer nada más aquí.
                SpriteManager.SetPlayerPosition(proximaPosicion);
            }
        }
    }
}