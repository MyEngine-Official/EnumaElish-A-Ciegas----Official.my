using System;
using Microsoft.Xna.Framework;
using MyEngine.MyCore.MyComponents;
using MyEngine.MyCore.MyEntities;


namespace MyEngine.MyCore.MySystems
{
    public class AnimationSystem : ISystem
    {
        private WorldManager _world;

        public void Initialize(WorldManager world)
        {
            _world = world;
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
                if (animComp.ElapsedTime >= animComp.CurrentAnimation.Delay)
                {
                    // Reset elapsed time
                    animComp.ElapsedTime = TimeSpan.Zero;

                    // Advance to next frame
                    animComp.CurrentFrame++;

                    // Check if we've reached the end of the animation
                    if (animComp.CurrentFrame >= animComp.CurrentAnimation.Frames.Count)
                    {
                        if (animComp.IsLooping)
                        {
                            // Loop back to the beginning
                            animComp.CurrentFrame = 0;
                            animComp.OnAnimationLoop?.Invoke(entity);
                        }
                        else
                        {
                            // Stop at the last frame
                            animComp.CurrentFrame = animComp.CurrentAnimation.Frames.Count - 1;
                            animComp.IsPlaying = false;
                            animComp.OnAnimationComplete?.Invoke(entity);
                        }
                    }

                    // Update the sprite's texture region
                    sprite.Region = animComp.CurrentAnimation.Frames[animComp.CurrentFrame];
                }
            }
        }

        /// <summary>
        /// Plays an animation for an entity
        /// </summary>
        public static void PlayAnimation(MainEntity entity, string animationName, bool loop = true)
        {
            if (!entity.HasComponent<AnimationComponent>())
                return;

            var animComp = entity.GetComponent<AnimationComponent>();

            if (animComp.Animations.TryGetValue(animationName, out var animation))
            {
                // Only reset if it's a different animation
                if (animComp.CurrentAnimation != animation)
                {
                    animComp.CurrentAnimation = animation;
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
    }
}
