using System;
using System.Reflection.Metadata;
using EnumaElish.Logic.Content.SceneManager;
using EnumaElish.UI.Content.GameUI.GameOnline;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameLibrary.Entities;
using MonoGameLibrary.Events;
using MonoGameLibrary.Graphics;
using RenderingLibrary.Graphics;

namespace EnumaElish.UI.Content.GameUI.GameOnline
{
    public class PrimeraFaseCueva : GameScene
    {
        private TextureAtlas _textureAtlas;
        private Texture2D _pixelBlanco;
        private Player _jugador;
        private Tilemap _mapa;
        private Collisions colisiones;

        public PrimeraFaseCueva()
        {
            
        }
        public override void Initialize()
        {
            _pixelBlanco = new Texture2D(Core.GraphicsDevice, 1, 1);
            _pixelBlanco.SetData(new[] { Color.White });
            _textureAtlas = TextureAtlas.FromFile(Content, "utilidades/buttons/atlas_buttons.xml");

            _jugador = new Player(_textureAtlas);
            _jugador.AddAnimacion(PlayerState.CorrerArriba, "main");
            _jugador.ActivateAnimation(PlayerState.CorrerArriba);
            _jugador.EditarAnimacion(PlayerState.CorrerArriba, new Vector2(10));

            _CursorAtlas = TextureAtlas.FromFile(Content, "utilidades/cursor/cursor_atlas.xml");
            CursorGameObject = new GameObject(_CursorAtlas.CreateSprite("mainC"));
            colisiones = Collisions.FromFile(Content, "utilidades/tilemaps/acto_uno_online/acto_uno_online_colisiones.xml");
            
            _mapa = Tilemap.FromFile(Content, "utilidades/tilemaps/acto_uno_online/acto_uno.xml");
            _mapa.Scale = new Vector2(0.238f);

            base.Initialize();
        }
        public override void LoadContent()
        {
            SpriteManager.AddPlayer(_jugador);
            SpriteManager.SetPlayerPosition(new Vector2(200f));

            var c = colisiones.GetCollisionRects();
            SpriteManager.AddMapColisions(c, _mapa.Scale); 
            BaseLoadContent();
            base.LoadContent();
        }
        public override void Update(GameTime gameTime)
        {
            moverJugador();

            CamaraInitialize(_mapa, _jugador);
            BaseUpdate(gameTime);
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            Core.GraphicsDevice.Clear(Color.RoyalBlue);
            var brocha = Core.SpriteBatch;
            var transform = CamaraManager.GetTransform() * _scaleMatrix;
            brocha.Begin(transformMatrix: transform);


            _mapa.Draw(brocha);
            //SpriteManager.DebugDraw(brocha, _pixelBlanco);

            // ---- AÑADE ESTA LÍNEA AQUÍ ----
            // Dibuja las colisiones del mapa en un color visible, como rojo.
            //SpriteManager.DebugDrawMapCollisions(brocha, _pixelBlanco, Color.Red * 0.7f);

            brocha.End();
            BaseDraw();
            base.Draw(gameTime);
        }

        int SPEED = 5;
        private void moverJugador()
        {
            var speed = 3f;
            var estado = InputManager.Keyboard.CurrentState;
            Vector2 proximaPosicion = _jugador.Position(); // Empezamos con la posición actual

            // Calculamos la posición a la que el jugador quiere moverse
            if (estado.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.W))
            {
                proximaPosicion.Y -= speed;
            }
            if (estado.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.S))
            {
                proximaPosicion.Y += speed;
            }
            if (estado.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.A))
            {
                proximaPosicion.X -= speed;
            }
            if (estado.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D))
            {
                proximaPosicion.X += speed;
            }

            // Solo si ha habido un intento de movimiento (la posición ha cambiado)
            if (proximaPosicion != _jugador.Position())
            {
                // Enviamos la POSICIÓN FUTURA para que SpriteManager la valide.
                // El método SetPlayerPosition ya se encarga de mover al jugador si no hay colisión.
                // No necesitamos hacer nada más aquí.
                SpriteManager.SetPlayerPosition(proximaPosicion);
            }
        }
    }
}