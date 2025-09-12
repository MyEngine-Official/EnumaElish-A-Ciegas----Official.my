using Microsoft.Xna.Framework;

namespace MyEngine_Core.ECS.MyComponents
{
    /// <summary>
    /// Component for collision detection
    /// </summary>
    public class ColliderComponent
    {
        /// <summary>
        /// Size of the collider in pixels
        /// </summary>
        public Vector2 Size { get; set; }
        
        /// <summary>
        /// Offset from the entity's position
        /// </summary>
        public Vector2 Offset { get; set; }
        
        /// <summary>
        /// If true, the collider only triggers events without physical collision
        /// </summary>
        public bool IsTrigger { get; set; }
        
        /// <summary>
        /// Tag for identifying collision types
        /// </summary>
        public string Tag { get; set; }
        
        /// <summary>
        /// Layer for collision filtering
        /// </summary>
        public int Layer { get; set; }
        
        /// <summary>
        /// Mask for determining which layers this collider can collide with
        /// </summary>
        public int CollisionMask { get; set; }
        
        /// <summary>
        /// Is this collider currently enabled
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Creates a new collider component
        /// </summary>
        /// <param name="width">Width of the collider</param>
        /// <param name="height">Height of the collider</param>
        public ColliderComponent(float width, float height)
        {
            Size = new Vector2(width, height);
            Offset = Vector2.Zero;
            IsTrigger = false;
            Tag = "Default";
            Layer = 0;
            CollisionMask = -1; // Collide with all layers by default
            IsEnabled = true;
        }
        
        /// <summary>
        /// Creates a new collider component with size
        /// </summary>
        /// <param name="size">Size of the collider</param>
        public ColliderComponent(Vector2 size)
        {
            Size = size;
            Offset = Vector2.Zero;
            IsTrigger = false;
            Tag = "Default";
            Layer = 0;
            CollisionMask = -1;
            IsEnabled = true;
        }
        
        /// <summary>
        /// Gets the bounding rectangle for this collider at the given position
        /// </summary>
        public Rectangle GetBounds(Vector2 position, Vector2 scale)
        {
            return new Rectangle(
                (int)(position.X + Offset.X * scale.X),
                (int)(position.Y + Offset.Y * scale.Y),
                (int)(Size.X * scale.X),
                (int)(Size.Y * scale.Y)
            );
        }
        
        /// <summary>
        /// Checks if this collider can collide with another based on layers
        /// </summary>
        public bool CanCollideWith(ColliderComponent other)
        {
            if (!IsEnabled || !other.IsEnabled)
                return false;
                
            return (CollisionMask & (1 << other.Layer)) != 0;
        }
    }
}
