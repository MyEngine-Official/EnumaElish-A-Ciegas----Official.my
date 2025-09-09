using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnumaElish.Logic.Content.SceneManager;
using EnumaElish.UI.Content.GameUI.GameOnline;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameLibrary.Entities;
using MonoGameLibrary.Events;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Scenes;

namespace EnumaElish.UI.Content.GameUI.GameTests.OnlineDev
{
    public class FaseDiurna : GameScene
    {
      
        private TextureAtlas _textureAtlas;
        private Button obj;


        public FaseDiurna()
        {
            
        }

            public override void Initialize()
            {
            _textureAtlas = TextureAtlas.FromFile(Content, "utilidades/buttons/textureAtlasBotones");
            _CursorAtlas = TextureAtlas.FromFile(Content, "utilidades/cursor/cursor_atlas.xml");

            CursorGameObject = new GameObject(_CursorAtlas.CreateSprite("mainC"));
            obj = new Button();
            obj.Clicked += Obj_Clicked;

            obj.AddSprite(MyButtonState.normal, _textureAtlas.CreateSprite("Next"));
            obj.AddSprite(MyButtonState.hover, _textureAtlas.CreateSprite("NextHover"));
            obj.Position(new Vector2(200, 10));

            base.Initialize();
        }

        private void Obj_Clicked()
        {
            Core.ChangeScene(new PrimeraFaseCueva());
        }

        public override void LoadContent()
        {
            BaseLoadContent();

            SpriteManager.Add(obj, Vector2.One);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            BaseUpdate(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {

            Core.GraphicsDevice.Clear(Color.RoyalBlue);
            //var brocha = Core.SpriteBatch;
            //brocha.Begin(transformMatrix: _scaleMatrix);


            //brocha.End();

            BaseDraw();
            base.Draw(gameTime);
        }
    }
}

