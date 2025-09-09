using EnumaElish.UI.Content.GameUI;
using EnumaElish.UI.Content.GameUI.GameTests.OnlineDev;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;

namespace EnumaElish.OpenGl
{
    public class Game1 : Core
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
       
        public Game1() : base("EnumaElish: A Ciegas", 1920, 1080, true)
        {
            //_graphics = new GraphicsDeviceManager(this);
            //Content.RootDirectory = "Content";
            //Window.AllowUserResizing = true;
            IsMouseVisible = false;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            ChangeScene(new FaseDiurna());
        }
    }
}
