using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyEngine.MyCore.MyComponents;
using MyEngine.MyGraphics;

namespace MyEngine.MyCore.MyComponents
{
    public class SpriteComponent
    {
        public TextureRegion Region { get; set; }
        public Color Color { get; set; } = Color.White;
        public Vector2 Origin { get; set; } = Vector2.Zero;
        public SpriteEffects Effects { get; set; } = SpriteEffects.None;
        public float LayerDepth { get; set; } = 0.0f;
        public SpriteComponent(TextureRegion region)
        {
            Region = region;
        }


        /// <summary>
        /// Coloca el origen del sprite al centro.
        /// </summary>
        public void CenterOrigin()
        {
            Origin = new Vector2(Region.Width, Region.Height) * 0.5f;
        }

        /// <summary>
        /// Submit this sprite for drawing to the current batch.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch instance used for batching draw calls.</param>
        /// <param name="position">The xy-coordinate position to render this sprite at.</param>
        public void Draw(SpriteBatch spriteBatch, TransformComponent transform)
        {
            Region.Draw(spriteBatch, transform.Position, Color, transform.Rotation, Origin, transform.Scale, Effects, LayerDepth);
        }
    }
}
