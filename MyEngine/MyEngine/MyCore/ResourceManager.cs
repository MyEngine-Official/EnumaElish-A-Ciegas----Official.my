using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using MyEngine.MyCore.MyComponents;
using MyEngine.MyGraphics;

namespace MyEngine.MyCore
{
    public class ResourceManager
    {
        ContentManager _contentManager;
        private TextureAtlas _textureAtlas;
        public ResourceManager(ContentManager content)
        {
            _contentManager = content;
        }
        public AnimationComponent CrearAnimacion(string route, string animationName)
        {
            _textureAtlas = TextureAtlas.FromFile(_contentManager, route);
            AnimationComponent animComp = _textureAtlas.CreateAnimatedSprite(animationName);
            return animComp;
        }

        public SpriteComponent CrearSprite(string TextureAtlasRoute, string spriteName)
        {
            _textureAtlas = TextureAtlas.FromFile(_contentManager, TextureAtlasRoute);
            SpriteComponent spriteComp = _textureAtlas.CreateSprite(spriteName);
            return spriteComp;
        }

        public ButtonComponent CrearBoton(Keys key, Buttons GamePadButton, Vector2 position, Vector2 size)
        {
            ButtonComponent buttonComp = new ButtonComponent(key, GamePadButton, position, size);
            return buttonComp;
        }

        public ColliderComponent CrearColisionador(Vector2 size, Vector2 offset, bool isTrigger, string tag, CollisionLayer layer, CollisionLayer cmask, bool isEnabled)
        {
            ColliderComponent colliderComp = new ColliderComponent(size, offset,isTrigger,tag,layer,cmask,isEnabled);
            return colliderComp;
        }

        public TransformComponent CrearTransformacion(Vector2 position, float rotation, Vector2 scale)
        {
            TransformComponent transformComp = new TransformComponent(position, rotation, scale);
            return transformComp;
        }

        public InputComponent CrearInput(bool mouse,bool gamePad,bool keyboard,bool isEnabled, int gamePadIndex)
        {
            InputComponent inputComp = new InputComponent(mouse, gamePad, keyboard, isEnabled, gamePadIndex);
            return inputComp;
        }

        public LifeComponent CrearVida(int maxHealth)
        {
            LifeComponent lifeComp = new LifeComponent(maxHealth);
            return lifeComp;
        }

        public RigidbodyComponent CrearRigidbody(float mass, float asceleration, Vector2 velocity)
        {
            RigidbodyComponent rigidbodyComp = new RigidbodyComponent();
            rigidbodyComp.SetVelocity(velocity);
            rigidbodyComp.SetMass(mass);
            rigidbodyComp.SetAcceleration(asceleration);

            return rigidbodyComp;
        }

        public TilemapCollisionsComponent CrearColisionesMapa(string path)
        {
            TilemapCollisionsComponent tilemapCollisionsComp = TilemapCollisionsComponent.FromFile(_contentManager, path);
            return tilemapCollisionsComp;
        }

        public TilemapComponent CrearMapa(string path)
        {
            TilemapComponent tilemapComp = TilemapComponent.FromFile(_contentManager, path);
            return tilemapComp;
        }

    }
}
