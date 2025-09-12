using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MyEngine.MyCore.MyComponents;
using MyEngine.MyCore.MyEntities;

namespace MyEngine.MyCore.MySystems
{
    public class PhysicsSystem
    {
        private WorldManager _world;

        public void Initialize(WorldManager world)
        {
            _world = world;
        }

        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            var entities = _world.GetEntitiesWithComponents<TransformComponent, RigidbodyComponent>();

            foreach (var entity in entities)
            {

                var transform = entity.GetComponent<TransformComponent>();
                var rigidbody = entity.GetComponent<RigidbodyComponent>();

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
        private void CheckCollisions(MainEntity entity, TransformComponent transform,
            RigidbodyComponent rigidbody, ColliderComponent collider)
        {
            var otherEntities = _world.GetEntitiesWithComponents<TransformComponent, ColliderComponent>();

            foreach (var other in otherEntities)
            {
                if (other.Id == entity.Id) continue;

                var otherTransform = other.GetComponent<TransformComponent>();

                var otherCollider = other.GetComponent<ColliderComponent>();
                if (!otherCollider.IsEnabled) return;

                // Verificar si pueden colisionar usando el sistema de capas
                if (!collider.CanCollideWith(otherCollider)) continue;

                // Simple AABB collision detection
                Rectangle bounds1 = GetBounds(transform, collider);
                Rectangle bounds2 = GetBounds(otherTransform, otherCollider);

                if (bounds1.Intersects(bounds2))
                {
                    // ✅ RESOLVER COLISIÓN (mejorado para top-down)
                    ResolveCollision(entity, other, bounds1, bounds2, rigidbody, otherCollider);
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

        private void ResolveCollision(MainEntity entity1, MainEntity entity2,
          Rectangle bounds1, Rectangle bounds2, RigidbodyComponent rigidbody1, ColliderComponent collider2)
        {
            // Si es un trigger, no resolver físicamente
            if (collider2.IsTrigger) return;
            // Calculate overlap
            Rectangle intersection = Rectangle.Intersect(bounds1, bounds2);
            var transform1 = entity1.GetComponent<TransformComponent>();

            // ✅ RESOLUCIÓN MEJORADA PARA TOP-DOWN
            // Calcular el vector de separación mínima
            Vector2 separation;


            if (intersection.Width < intersection.Height)
            {
                // Separación horizontal
                if (bounds1.Center.X < bounds2.Center.X)
                {
                    separation = new Vector2(-intersection.Width, 0);
                    // En top-down, cancelar velocidad horizontal
                    rigidbody1.Velocity = new Vector2(0, rigidbody1.Velocity.Y);
                }
                else
                {
                    separation = new Vector2(intersection.Width, 0);
                    rigidbody1.Velocity = new Vector2(0, rigidbody1.Velocity.Y);
                }
            }
            else
            {
                // Separación vertical
                if (bounds1.Center.Y < bounds2.Center.Y)
                {
                    separation = new Vector2(0, -intersection.Height);
                    // En top-down, cancelar velocidad vertical
                    rigidbody1.Velocity = new Vector2(rigidbody1.Velocity.X, 0);
                }
                else
                {
                    separation = new Vector2(0, intersection.Height);
                    rigidbody1.Velocity = new Vector2(rigidbody1.Velocity.X, 0);
                }
            }

            // Aplicar separación
            transform1.Position += separation;
        }



        // ✅ MÉTODO AUXILIAR: Detener completamente una entidad
        public void StopEntity(MainEntity entity)
        {
            if (entity.HasComponent<RigidbodyComponent>())
            {
                var rigidbody = entity.GetComponent<RigidbodyComponent>();
                rigidbody.Velocity = Vector2.Zero;
            }
        }

        // ✅ MÉTODO AUXILIAR: Mover hacia una dirección
        public void MoveTowards(MainEntity entity, Vector2 direction, float speed)
        {
            if (entity.HasComponent<RigidbodyComponent>())
            {
                var rigidbody = entity.GetComponent<RigidbodyComponent>();
                direction.Normalize();
                rigidbody.Velocity = direction * speed;
            }
        }

        /// <summary>
        /// Applies an impulse force to an entity
        /// </summary>
        public void ApplyImpulse(MainEntity entity, Vector2 impulse)
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
        public void ApplyForce(MainEntity entity, Vector2 force, float deltaTime)
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
