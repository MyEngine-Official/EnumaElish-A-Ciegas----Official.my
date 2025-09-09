using System;
using Microsoft.Xna.Framework;
using MyEngine_Core.ECS.MyComponents;

namespace MyEngine_Core.ECS.MySystems
{
    public class PhysicsSystem : ISystem
    {
        private World _world;
        private Vector2 _gravity = new Vector2(0, 980f); // Default gravity (pixels per second squared)
        private float _damping = 0.99f; // Velocity damping factor

        public PhysicsSystem()
        {
        }

        public void Initialize(World world)
        {
            _world = world;
        }

        /// <summary>
        /// Gets or sets the gravity vector
        /// </summary>
        public Vector2 Gravity
        {
            get => _gravity;
            set => _gravity = value;
        }

        /// <summary>
        /// Gets or sets the damping factor (0-1)
        /// </summary>
        public float Damping
        {
            get => _damping;
            set => _damping = MathHelper.Clamp(value, 0f, 1f);
        }

        /// <summary>
        /// Updates physics for all entities with rigidbody components
        /// </summary>
        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            // Get all entities with both Transform and Rigidbody components
            var physicsEntities = _world.GetEntitiesWithComponents<TransformComponent, RigidbodyComponent>();

            foreach (var entity in physicsEntities)
            {
                var transform = entity.GetComponent<TransformComponent>();
                var rigidbody = entity.GetComponent<RigidbodyComponent>();

                // Apply gravity if mass is greater than 0
                if (rigidbody.Mass > 0)
                {
                    rigidbody.Velocity += _gravity * deltaTime;
                }

                // Apply damping
                rigidbody.Velocity *= _damping;

                // Update position based on velocity
                transform.Position += rigidbody.Velocity * deltaTime;

                // Check for collision components if needed
                if (entity.HasComponent<ColliderComponent>())
                {
                    var collider = entity.GetComponent<ColliderComponent>();
                    CheckCollisions(entity, transform, rigidbody, collider);
                }
            }
        }

        /// <summary>
        /// Checks collisions between entities
        /// </summary>
        private void CheckCollisions(EntidadPadre entity, TransformComponent transform, 
            RigidbodyComponent rigidbody, ColliderComponent collider)
        {
            var otherEntities = _world.GetEntitiesWithComponents<TransformComponent, ColliderComponent>();
            
            foreach (var other in otherEntities)
            {
                if (other.Id == entity.Id) continue;

                var otherTransform = other.GetComponent<TransformComponent>();
                var otherCollider = other.GetComponent<ColliderComponent>();

                // Simple AABB collision detection
                Rectangle bounds1 = GetBounds(transform, collider);
                Rectangle bounds2 = GetBounds(otherTransform, otherCollider);

                if (bounds1.Intersects(bounds2))
                {
                    // Handle collision
                    ResolveCollision(entity, other, bounds1, bounds2, rigidbody);
                }
            }
        }

        /// <summary>
        /// Gets the bounding rectangle for collision detection
        /// </summary>
        private Rectangle GetBounds(TransformComponent transform, ColliderComponent collider)
        {
            return new Rectangle(
                (int)(transform.Position.X + collider.Offset.X),
                (int)(transform.Position.Y + collider.Offset.Y),
                (int)(collider.Size.X * transform.Scale.X),
                (int)(collider.Size.Y * transform.Scale.Y)
            );
        }

        /// <summary>
        /// Resolves collision between two entities
        /// </summary>
        private void ResolveCollision(EntidadPadre entity1, EntidadPadre entity2, 
            Rectangle bounds1, Rectangle bounds2, RigidbodyComponent rigidbody1)
        {
            // Calculate overlap
            Rectangle intersection = Rectangle.Intersect(bounds1, bounds2);
            
            // Simple collision response - push entity1 out of entity2
            var transform1 = entity1.GetComponent<TransformComponent>();
            
            // Determine push direction based on smallest overlap
            if (intersection.Width < intersection.Height)
            {
                // Push horizontally
                if (bounds1.Center.X < bounds2.Center.X)
                {
                    transform1.Position.X -= intersection.Width;
                    rigidbody1.Velocity.X = Math.Min(rigidbody1.Velocity.X, 0);
                }
                else
                {
                    transform1.Position.X += intersection.Width;
                    rigidbody1.Velocity.X = Math.Max(rigidbody1.Velocity.X, 0);
                }
            }
            else
            {
                // Push vertically
                if (bounds1.Center.Y < bounds2.Center.Y)
                {
                    transform1.Position.Y -= intersection.Height;
                    rigidbody1.Velocity.Y = Math.Min(rigidbody1.Velocity.Y, 0);
                }
                else
                {
                    transform1.Position.Y += intersection.Height;
                    rigidbody1.Velocity.Y = Math.Max(rigidbody1.Velocity.Y, 0);
                }
            }
        }

        /// <summary>
        /// Applies an impulse force to an entity
        /// </summary>
        public void ApplyImpulse(EntidadPadre entity, Vector2 impulse)
        {
            if (entity.HasComponent<RigidbodyComponent>())
            {
                var rigidbody = entity.GetComponent<RigidbodyComponent>();
                if (rigidbody.Mass > 0)
                {
                    rigidbody.Velocity += impulse / rigidbody.Mass;
                }
            }
        }

        /// <summary>
        /// Applies a continuous force to an entity
        /// </summary>
        public void ApplyForce(EntidadPadre entity, Vector2 force, float deltaTime)
        {
            if (entity.HasComponent<RigidbodyComponent>())
            {
                var rigidbody = entity.GetComponent<RigidbodyComponent>();
                if (rigidbody.Mass > 0)
                {
                    rigidbody.Velocity += (force / rigidbody.Mass) * deltaTime;
                }
            }
        }
    }
}

namespace MyEngine_Core.ECS.MyComponents
{
    /// <summary>
    /// Component for collision detection
    /// </summary>
    public class ColliderComponent
    {
        public Vector2 Size { get; set; }
        public Vector2 Offset { get; set; }
        public bool IsTrigger { get; set; }
        public string Tag { get; set; }

        public ColliderComponent(float width, float height)
        {
            Size = new Vector2(width, height);
            Offset = Vector2.Zero;
            IsTrigger = false;
            Tag = "Default";
        }
    }
}