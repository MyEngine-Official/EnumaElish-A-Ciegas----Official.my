using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Audio;
using MonoGameLibrary.Entities;
using MonoGameLibrary.Events;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;
using MonoGameLibrary.Scenes;

namespace EnumaElish.Logic.Content.SceneManager
{
    public class GameScene : Scene
    {

        public Vector2 realMouse;
        public Vector2 mouseWorld;
        public MouseState mouseState;
        public GameScene()
        {
        }


        public readonly int baseWidth = 1920;
        public readonly int baseHeight = 1080;
        public Matrix _scaleMatrix;


        public InputManager InputManager = new InputManager();


        public SpriteManager SpriteManager = new SpriteManager();
        public AudioController AudioController = new AudioController();
        public Camera2D CamaraManager = new Camera2D();

        #region Propiedades para cambiar el aspecto del mouse

        /// <summary> Siempre hay que llenar estas propiedades para que el mouse funcione correctamente. Y EL JUEGO EN SI </summary>
        public GameObject CursorGameObject;
        /// <summary> Siempre hay que llenar estas propiedades para que el mouse funcione correctamente. Y EL JUEGO EN SI </summary>
        public TextureAtlas _CursorAtlas;
        public HashSet<int> MouseColisiones = new HashSet<int>();

        #endregion

        public void detectarMouse(GameTime time)
        {
            InputManager.Update(time);
            mouseState = InputManager.Mouse.CurrentState;
            realMouse = new Vector2(mouseState.X, mouseState.Y);

            // Matriz de transformación total invertida
            Matrix inverseTransform = Matrix.Invert(CamaraManager.GetTransform() * _scaleMatrix);

            // Transformamos la posición del mouse en pantalla a coordenadas del mundo
            mouseWorld = Vector2.Transform(realMouse, inverseTransform);

            if (InputManager.Mouse.WasMoved && CursorGameObject != null) // Añadida comprobación de null
            {
                // El cursor visual (UI) no debe usar coordenadas del mundo, sino las de la pantalla escalada
                Vector2 scaledMouse = Vector2.Transform(realMouse, Matrix.Invert(_scaleMatrix));
                SpriteManager.SetPosition(CursorGameObject._sprite, scaledMouse, MouseColisiones);
            }
        }
        public void RecalculateScale()
        {
            float scaleX = (float)Core.GraphicsDevice.Viewport.Width / baseWidth;
            float scaleY = (float)Core.GraphicsDevice.Viewport.Height / baseHeight;
            float scale = Math.Min(scaleX, scaleY); // Mantener proporción
            //_scaleMatrix = Matrix.CreateScale(scale, scale, 1f);
            _scaleMatrix = Matrix.Identity;
        }

        public void BaseLoadContent()
        {
            if(CursorGameObject != null)
            SpriteManager.Add(CursorGameObject._sprite, Vector2.One);

            RecalculateScale();
        }

        public void BaseUpdate(GameTime gameTime)
        {
            detectarMouse(gameTime);

            // --- ¡AQUÍ ESTÁ LA LÓGICA CLAVE! ---
            // En la clase de tu escena real (no en esta clase base),
            // deberías tener una referencia a tu jugador.
            // La línea sería algo como esto:
            //
            // Vector2 screenCenter = new Vector2(Core.GraphicsDevice.Viewport.Width / 2, Core.GraphicsDevice.Viewport.Height / 2);
            // CamaraManager.Position = tuJugador.Position - screenCenter;
            //
            // Esto hace que la cámara siempre se centre en el jugador.
            // Por ahora, lo dejaremos como un comentario para que lo implementes.

            SpriteManager.UpdateAnimated(gameTime);
            SpriteManager.UpdateButtons(gameTime, mouseWorld, mouseState, _scaleMatrix);
        }
        public void BaseDraw()
        {
            var brocha = Core.SpriteBatch;

            // Calculamos la matriz de transformación final para el mundo del juego
            Matrix worldTransform = CamaraManager.GetTransform() * _scaleMatrix;

            // 1. DIBUJAR EL MUNDO DEL JUEGO (se mueve con la cámara) 🎥
            brocha.Begin(transformMatrix: worldTransform, samplerState: SamplerState.PointClamp); // PointClamp es bueno para pixel art

            SpriteManager.Draw(brocha, CursorGameObject._sprite); // Dibuja todos tus sprites (jugador, mapa, etc.)

            brocha.End();


            // 2. DIBUJAR LA INTERFAZ (UI) (fija en la pantalla) ✨
            brocha.Begin(transformMatrix: _scaleMatrix, samplerState: SamplerState.PointClamp);

            if (CursorGameObject != null)
            {
                SpriteManager.SoloDrawSprite(CursorGameObject._sprite, brocha); // El cursor ahora se dibuja aquí
            }
            // Aquí también dibujarías la vida, el puntaje, etc.

            brocha.End();
        }


        public void CamaraInitialize(Tilemap _mapa, Player _jugador)
        {

            // ----------------------------------------------------
            // LÓGICA DE LA CÁMARA (nueva)
            // ----------------------------------------------------

            // Obtiene el tamaño de la ventana de visualización en píxeles.
            var viewport = Core.GraphicsDevice.Viewport;
            Vector2 screenCenter = new Vector2(viewport.Width / 2, viewport.Height / 2);

            // Calcula el tamaño del mundo del juego.
            float mapWidth = _mapa.Columns * _mapa.TileWidth;
            float mapHeight = _mapa.Rows * _mapa.TileHeight;

            // Calcula la posición deseada de la cámara para centrarla en el jugador.
            Vector2 cameraPosition = _jugador.Position() - screenCenter;

            // APLICAR LÍMITES
            // Limita la posición X de la cámara.
            // El límite inferior es 0 (no se mueve a la izquierda del mapa).
            // El límite superior es el ancho del mapa menos el ancho de la pantalla.
            float cameraMinX = 0;
            float cameraMaxX = mapWidth - viewport.Width;
            cameraPosition.X = MathHelper.Clamp(cameraPosition.X, cameraMinX, cameraMaxX);

            // Limita la posición Y de la cámara.
            // El límite inferior es 0 (no se mueve hacia arriba del mapa).
            // El límite superior es la altura del mapa menos la altura de la pantalla.
            float cameraMinY = 0;
            float cameraMaxY = mapHeight - viewport.Height;
            cameraPosition.Y = MathHelper.Clamp(cameraPosition.Y, cameraMinY, cameraMaxY);

            // Aplica la posición final a la cámara.
            CamaraManager.Position = cameraPosition;
            // ----------------------------------------------------
        }

    }
}
