using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyEngine.MyCore.MyComponents;
using MyEngine.MyGraphics;

namespace MyEngine.MyCore.MySystems
{
    public class RenderSystem : ISystem
    {
        private SpriteBatch _spriteBatch;
        private Camara2D _camera;
        private WorldManager _world;
        private GraphicsDevice _graphicsDevice;

        public RenderSystem(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            _graphicsDevice = graphicsDevice;
            _spriteBatch = spriteBatch;
            _camera = new Camara2D(graphicsDevice.Viewport);
        }

        public void Initialize(WorldManager world)
        {
            _world = world;
        }


        /// <summary>
        /// Gets or sets the camera used for rendering
        /// </summary>
        public Camara2D Camera
        {
            get => _camera;
            set => _camera = value;
        }

        /// <summary>
        /// Draws all renderable entities
        /// </summary>
        public void Draw(GameTime gameTime)
        {
            // Get all entities for rendering
            var entities = _world.GetAllEntities();

            // Sort entities by layer depth for proper rendering order
            var sortedEntities = entities
                .Where(e => e.HasComponent<TransformComponent>() &&
                           (e.HasComponent<SpriteComponent>() || e.HasComponent<TilemapComponent>()))
                .OrderBy(e =>
                {
                    if (e.HasComponent<SpriteComponent>())
                        return e.GetComponent<SpriteComponent>().LayerDepth;
                    return 0f; // Tilemaps render at layer 0 by default
                })
                .ToList();

            // Begin sprite batch with camera transform
            _spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                null,
                null,
                null,
                _camera?.GetTransform());

            // Draw Tilemaps first (usually background)
            foreach (var entity in sortedEntities)
            {
                if (entity.HasComponent<TilemapComponent>() && entity.HasComponent<TransformComponent>())
                {
                    var transform = entity.GetComponent<TransformComponent>();
                    var tilemap = entity.GetComponent<TilemapComponent>();

                    // Apply transform to tilemap
                    //var oldScale = tilemap.Scale;
                    tilemap.Scale = transform.Scale;
                    
                    // Draw tilemap at transform position
                    // Note: You might want to modify TilemapComponent.Draw to accept position
                    tilemap.Draw(_spriteBatch);

                    //tilemap.Scale = oldScale;
                }
            }

            // Draw Sprites
            foreach (var entity in sortedEntities)
            {
                if (entity.HasComponent<SpriteComponent>() && entity.HasComponent<TransformComponent>())
                {
                    var transform = entity.GetComponent<TransformComponent>();
                    var sprite = entity.GetComponent<SpriteComponent>();

                    // Draw sprite using its own method
                    sprite.Draw(_spriteBatch, transform);
                }
            }

            _spriteBatch.End();
        }
    }
}
