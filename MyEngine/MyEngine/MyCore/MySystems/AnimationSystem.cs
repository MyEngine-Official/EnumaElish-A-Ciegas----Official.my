using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyEngine.MyCore.Events;
using MyEngine.MyCore.MyComponents;
using MyEngine.MyCore.MyEntities;

namespace MyEngine.MyCore.MySystems
{
    public class AnimationSystem : ISystem, IEventSubscriber
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
            // Reaccionar a eventos de gameplay
            eventBus.Subscribe<CollisionEnterEvent>(OnCollision);
            eventBus.Subscribe<PlayerDiedEvent>(OnPlayerDied);
            eventBus.Subscribe<ItemCollectedEvent>(OnItemCollected);            
            // Suscribirse a eventos de dirección
            eventBus.Subscribe<EntityDirectionEvent>(OnEntityDirectionChanged);
        }

        /// <summary>
        /// Updates all animation components
        /// </summary>
        public void Update(GameTime gameTime)
        {
            var animatedEntities = _world.GetEntitiesWithComponents<AnimationComponent, SpriteComponent>();

            foreach (var entity in animatedEntities)
            {
                var animComp = entity.GetComponent<AnimationComponent>();
                var sprite = entity.GetComponent<SpriteComponent>();

                if (!animComp.IsPlaying || animComp.CurrentAnimation == null)
                    continue;

                // Update elapsed time
                animComp.ElapsedTime += gameTime.ElapsedGameTime;

                // Check if it's time to advance to the next frame
                if (animComp.ElapsedTime >= animComp.CurrentAnimation.Item2.Delay)
                {
                    // Reset elapsed time
                    animComp.ElapsedTime = TimeSpan.Zero;

                    // Advance to next frame
                    animComp.CurrentFrame++;

                    // Check if we've reached the end of the animation
                    if (animComp.CurrentFrame >= animComp.CurrentAnimation.Item2.Frames.Count)
                    {
                        if (animComp.IsLooping)
                        {
                            // Loop back to the beginning
                            animComp.CurrentFrame = 0;
                            animComp.OnAnimationLoop?.Invoke(entity);

                            _eventBus.Publish(new AnimationLoopedEvent
                            {
                                EntityId = entity.Id,
                                AnimationName = animComp.CurrentAnimation.Item1.ToString(),
                                LoopCount = animComp.LoopCount++
                            });
                        }
                        else
                        {
                            // Stop at the last frame
                            animComp.CurrentFrame = animComp.CurrentAnimation.Item2.Frames.Count - 1;
                            animComp.IsPlaying = false;
                            animComp.OnAnimationComplete?.Invoke(entity);

                            _eventBus.Publish(new AnimationCompletedEvent
                            {
                                EntityId = entity.Id,
                                AnimationName = animComp.CurrentAnimation.Item1.ToString()
                            });
                        }
                    }

                    // Update the sprite's texture region
                    sprite.Region = animComp.CurrentAnimation.Item2.Frames[animComp.CurrentFrame];
                }
            }
        }

        /// <summary>
        /// Plays an animation for an entity
        /// </summary>
        public static void PlayAnimation(MainEntity entity, AnimationAction animationName, bool loop = true)
        {
            if (!entity.HasComponent<AnimationComponent>())
                return;

            var animComp = entity.GetComponent<AnimationComponent>();

            if (animComp.Animations.TryGetValue(animationName, out var animation))
            {
                // Only reset if it's a different animation
                if (animComp.CurrentAnimation == null || animComp.CurrentAnimation.Item1 != animationName)
                {
                    animComp.SetCurrentAnimation(animationName);
                    animComp.CurrentFrame = 0;
                    animComp.ElapsedTime = TimeSpan.Zero;
                }

                animComp.IsPlaying = true;
                animComp.IsLooping = loop;

                // Update sprite immediately
                if (entity.HasComponent<SpriteComponent>())
                {
                    var sprite = entity.GetComponent<SpriteComponent>();
                    sprite.Region = animation.Frames[0];
                }
            }
        }

        /// <summary>
        /// Stops the current animation
        /// </summary>
        public static void StopAnimation(MainEntity entity)
        {
            if (!entity.HasComponent<AnimationComponent>())
                return;

            var animComp = entity.GetComponent<AnimationComponent>();
            animComp.IsPlaying = false;
        }

        /// <summary>
        /// Pauses the current animation
        /// </summary>
        public static void PauseAnimation(MainEntity entity)
        {
            if (!entity.HasComponent<AnimationComponent>())
                return;

            var animComp = entity.GetComponent<AnimationComponent>();
            animComp.IsPlaying = false;
        }

        /// <summary>
        /// Resumes the current animation
        /// </summary>
        public static void ResumeAnimation(MainEntity entity)
        {
            if (!entity.HasComponent<AnimationComponent>())
                return;

            var animComp = entity.GetComponent<AnimationComponent>();
            if (animComp.CurrentAnimation != null)
                animComp.IsPlaying = true;
        }

        // ================================
        // ✨ NUEVO: MANEJO DE DIRECCIÓN
        // ================================
        private void OnEntityDirectionChanged(EntityDirectionEvent e)
        {
            var entity = _world.GetAllEntities().FirstOrDefault(ent => ent.Id == e.EntityId);
            if (entity?.HasComponent<AnimationComponent>() != true || !entity.HasComponent<SpriteComponent>())
                return;

            var animComp = entity.GetComponent<AnimationComponent>();
            var spriteComp = entity.GetComponent<SpriteComponent>();

            // Determinar la dirección principal
            Vector2 direction = e.Direction;
            if (direction == Vector2.Zero) return;

            // Normalizar para obtener la dirección principal
            direction.Normalize();

            // Aplicar transformaciones basadas en la dirección
            ApplyDirectionToAnimation(entity, direction, animComp, spriteComp);

            // Cambiar animación según movimiento
            UpdateMovementAnimation(entity, direction, animComp);
        }

        private void ApplyDirectionToAnimation(MainEntity entity, Vector2 direction, 
            AnimationComponent animComp, SpriteComponent spriteComp)
        {
            // Reset effects
            spriteComp.Effects = SpriteEffects.None;
            
            var transform = entity.HasComponent<TransformComponent>() 
                ? entity.GetComponent<TransformComponent>() 
                : null;

            // Determinar transformaciones según la dirección
            if (Math.Abs(direction.X) > Math.Abs(direction.Y))
            {
                // Movimiento horizontal predominante
                if (direction.X < 0)
                {
                    // Izquierda: Reflejar horizontalmente las animaciones Right
                    spriteComp.Effects = SpriteEffects.FlipHorizontally;
                    
                    // Cambiar a versión Right si está disponible
                    TryChangeToDirectionalAnimation(entity, GetRightVersion(animComp.CurrentAnimation?.Item1));
                }
                else
                {
                    // Derecha: Usar animaciones Right normalmente
                    spriteComp.Effects = SpriteEffects.None;
                    TryChangeToDirectionalAnimation(entity, GetRightVersion(animComp.CurrentAnimation?.Item1));
                }
            }
            else
            {
                // Movimiento vertical predominante
                if (direction.Y < 0)
                {
                    // Arriba: Usar animaciones Up normalmente
                    spriteComp.Effects = SpriteEffects.None;
                    TryChangeToDirectionalAnimation(entity, GetUpVersion(animComp.CurrentAnimation?.Item1));
                }
                else
                {
                    // Abajo: Reflejar verticalmente las animaciones Up
                    spriteComp.Effects = SpriteEffects.FlipVertically;
                    TryChangeToDirectionalAnimation(entity, GetUpVersion(animComp.CurrentAnimation?.Item1));
                }
            }
        }

        private void TryChangeToDirectionalAnimation(MainEntity entity, AnimationAction? newAnimation)
        {
            if (newAnimation.HasValue && entity.HasComponent<AnimationComponent>())
            {
                var animComp = entity.GetComponent<AnimationComponent>();
                if (animComp.Animations.ContainsKey(newAnimation.Value))
                {
                    // Solo cambiar si es diferente a la actual
                    if (animComp.CurrentAnimation?.Item1 != newAnimation.Value)
                    {
                        PlayAnimation(entity, newAnimation.Value, true);
                    }
                }
            }
        }

        private AnimationAction? GetRightVersion(AnimationAction? currentAction)
        {
            if (!currentAction.HasValue) return null;

            return currentAction.Value switch
            {
                AnimationAction.RunUp => AnimationAction.RunRight,
                AnimationAction.JumpUp => AnimationAction.JumpRigth,
                AnimationAction.Hability1Up => AnimationAction.Hability1Rigth,
                AnimationAction.Hability2Up => AnimationAction.Hability2Rigth,
                AnimationAction.Hability3Up => AnimationAction.Hability3Rigth,
                AnimationAction.StopUp => AnimationAction.StopRight,
                // Si ya es Right, mantenerlo
                _ => currentAction.Value
            };
        }

        private AnimationAction? GetUpVersion(AnimationAction? currentAction)
        {
            if (!currentAction.HasValue) return null;

            return currentAction.Value switch
            {
                AnimationAction.RunRight => AnimationAction.RunUp,
                AnimationAction.JumpRigth => AnimationAction.JumpUp,
                AnimationAction.Hability1Rigth => AnimationAction.Hability1Up,
                AnimationAction.Hability2Rigth => AnimationAction.Hability2Up,
                AnimationAction.Hability3Rigth => AnimationAction.Hability3Up,
                AnimationAction.StopRight => AnimationAction.StopUp,
                // Si ya es Up, mantenerlo
                _ => currentAction.Value
            };
        }

        private void UpdateMovementAnimation(MainEntity entity, Vector2 direction, AnimationComponent animComp)
        {
            // Si no está reproduciendo una animación de acción importante, cambiar a movimiento
            var currentAnim = animComp.CurrentAnimation?.Item1;
            
            // No interrumpir animaciones importantes
            if (currentAnim == AnimationAction.Death ||
                currentAnim == AnimationAction.hited ||
                currentAnim == AnimationAction.looting ||
                currentAnim == AnimationAction.disapeared)
            {
                return;
            }

            // Determinar animación de movimiento
            AnimationAction targetAnimation;
            
            if (Math.Abs(direction.X) > Math.Abs(direction.Y))
            {
                targetAnimation = AnimationAction.RunRight;
            }
            else
            {
                targetAnimation = AnimationAction.RunUp;
            }

            // Cambiar a animación de movimiento si es necesario
            if (currentAnim != targetAnimation)
            {
                PlayAnimation(entity, targetAnimation, true);
            }
        }

        // ================================
        // EVENT HANDLERS
        // ================================
        private void OnCollision(CollisionEnterEvent e)
        {
            // Reproducir animación de impacto en ambas entidades
            PlayAnimationOnEntity(e.Entity1Id, AnimationAction.hited, false);
            PlayAnimationOnEntity(e.Entity2Id, AnimationAction.hited, false);
        }

        private void OnPlayerDied(PlayerDiedEvent e)
        {
            PlayAnimationOnEntity(e.PlayerId, AnimationAction.Death, false);
        }

        private void OnItemCollected(ItemCollectedEvent e)
        {
            // Animación de recolección en el jugador
            
            PlayAnimationOnEntity(e.ItemEntityId, AnimationAction.looting, false);

            // Animación de desaparición en el item
            PlayAnimationOnEntity(e.ItemEntityId, AnimationAction.disapeared, false);
        }

        private void OnEntityMoved(EntityMovedEvent e)
        {
            // Esta lógica ahora se maneja en OnEntityDirectionChanged
            // Mantener por compatibilidad o eliminar si no es necesaria
        }

        private void PlayAnimationOnEntity(int entityId, AnimationAction animationName, bool loop)
        {
            var entity = _world.GetAllEntities().FirstOrDefault(ent => ent.Id == entityId);
            if (entity != null)
            {
                PlayAnimation(entity, animationName, loop);

                _eventBus.Publish(new AnimationStartedEvent
                {
                    EntityId = entityId,
                    AnimationName = animationName,
                    IsLooping = loop
                });
            }
        }
    }
}
