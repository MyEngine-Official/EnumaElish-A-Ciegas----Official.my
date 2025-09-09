using EnumaElish.UI.Content.GameUI;
using EnumaElish.UI.Content.GameUI.GameTests.OnlineDev;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Scenes;

namespace EnumaElish.GameUI
{
    public class Game1 : Core
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Game1() : base("EnumaElish: A Ciegas", 1920, 1280, true)
        {
            IsMouseVisible = false;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            Core.ChangeScene(new FaseDiurna());

            base.LoadContent();
        }
    }
}
