using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MyEngine.MyCore.Events;
using MyEngine.MyCore.MyComponents;
using MyEngine.MyCore.MyEntities;

namespace MyEngine.MyCore.MySystems
{
    public class PhysicsSystem : ISystem, IEventSubscriber
    {
        private WorldManager _world;
        private EventBus _eventBus;

        public void Initialize(WorldManager world)
        {
            _world = world;
            _eventBus = world.Events;
        }

        public void SubscribeToEvents(EventBus eventBus)
        {
            // Suscribirse a eventos que afectan la física
            eventBus.Subscribe<PlayerDiedEvent>(OnPlayerDied);
            eventBus.Subscribe<ItemCollectedEvent>(OnItemCollected);
        }
        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            var entities = _world.GetEntitiesWithComponents<TransformComponent, RigidbodyComponent>();

            foreach (var entity in entities)
            {

                var transform = entity.GetComponent<TransformComponent>();
                var rigidbody = entity.GetComponent<RigidbodyComponent>();
                Vector2 oldPosition = transform.Position;

                // Update position based on velocity
                transform.Position += rigidbody.Velocity * deltaTime;


                float distanceMoved = Vector2.Distance(oldPosition, transform.Position);
                if (distanceMoved > 1.0f) // Threshold para evitar spam
                {
                    _eventBus.QueueEvent(new EntityMovedEvent
                    {
                        EntityId = entity.Id,
                        OldPosition = oldPosition,
                        NewPosition = transform.Position
                    });
                }
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
            var tileMapColidersOtherEntities = _world.GetEntitiesWithComponents<TransformComponent, TilemapCollisionsComponent>();

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
                    Vector2 collisionPoint = new Vector2(
                    (bounds1.Center.X + bounds2.Center.X) / 2,
                    (bounds1.Center.Y + bounds2.Center.Y) / 2
                );
                    // ✨ NUEVO: Publicar evento según tipo de colisión
                    if (collider.IsTrigger || otherCollider.IsTrigger)
                    {
                        _eventBus.Publish(new TriggerEnterEvent
                        {
                            TriggerId = collider.IsTrigger ? entity.Id : other.Id,
                            EntityId = collider.IsTrigger ? other.Id : entity.Id,
                            TriggerTag = collider.IsTrigger ? collider.Tag : otherCollider.Tag,
                            EntityTag = collider.IsTrigger ? otherCollider.Tag : collider.Tag
                        });
                    }
                    else
                    {
                        _eventBus.Publish(new CollisionEnterEvent
                        {
                            Entity1Id = entity.Id,
                            Entity2Id = other.Id,
                            CollisionPoint = collisionPoint,
                            Entity1Tag = collider.Tag,
                            Entity2Tag = otherCollider.Tag,
                        });

                        // ✅ RESOLVER COLISIÓN (mejorado para top-down)
                        ResolveCollision(entity, other, bounds1, bounds2, rigidbody, otherCollider);
                    }
                }
            }

            foreach(var other in tileMapColidersOtherEntities)
            {
                var otherTransform = other.GetComponent<TransformComponent>();
                var tilemapColliders = other.GetComponent<TilemapCollisionsComponent>();
                var collisionRects = tilemapColliders.GetCollisionRects();
                foreach (var rect in collisionRects)
                {
                    // Ajustar el rectángulo de colisión según la posición y escala del tilemap
                    Rectangle adjustedRect = new Rectangle(
                        (int)(otherTransform.Position.X + rect.X * otherTransform.Scale.X),
                        (int)(otherTransform.Position.Y + rect.Y * otherTransform.Scale.Y),
                        (int)(rect.Width * otherTransform.Scale.X),
                        (int)(rect.Height * otherTransform.Scale.Y)
                    );
                    Rectangle bounds1 = GetBounds(transform, collider);
                    if (bounds1.Intersects(adjustedRect))
                    {
                        // Crear un ColliderComponent temporal para el rectángulo del tilemap
                        ColliderComponent tempCollider = new ColliderComponent(new Vector2(adjustedRect.Width, adjustedRect.Height))
                        {
                            Size = new Vector2(adjustedRect.Width, adjustedRect.Height),
                            Offset = Vector2.Zero,
                            IsTrigger = false,
                            IsEnabled = true,
                            Layer = CollisionLayer.All,
                            CollisionMask = CollisionLayer.All // Collide with all layers
                        };
                        // ✅ RESOLVER COLISIÓN (mejorado para top-down)
                        ResolveCollision(entity, other, bounds1, adjustedRect, rigidbody, tempCollider);
                    }
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

        private void OnPlayerDied(PlayerDiedEvent e)
        {
            // Detener física del jugador muerto
            var deadEntity = _world.GetAllEntities()
                .FirstOrDefault(ent => ent.Id == e.PlayerId);

            if (deadEntity?.HasComponent<RigidbodyComponent>() == true)
            {
                deadEntity.GetComponent<RigidbodyComponent>().Velocity = Vector2.Zero;
            }
        }

        private void OnItemCollected(ItemCollectedEvent e)
        {
            // El item ya no debe colisionar
            var itemEntity = _world.GetAllEntities()
                .FirstOrDefault(ent => ent.Id == e.ItemEntityId);

            if (itemEntity?.HasComponent<ColliderComponent>() == true)
            {
                itemEntity.GetComponent<ColliderComponent>().IsEnabled = false;
            }
        }


    }
}
