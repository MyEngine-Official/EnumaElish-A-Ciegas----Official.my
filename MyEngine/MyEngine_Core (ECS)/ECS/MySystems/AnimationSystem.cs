using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MyEngine_Core.ECS.MyComponents;
using MyEngine_Core.ECS.MyEntities;
using MyEngine_Core.MyGraphics;

namespace MyEngine_Core.ECS.MySystems
{
    public class AnimationSystem : ISystem
    {
        private World _world;

        public void Initialize(World world)
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
        public static void PlayAnimation(EntidadPadre entity, string animationName, bool loop = true)
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
        public static void StopAnimation(EntidadPadre entity)
        {
            if (!entity.HasComponent<AnimationComponent>())
                return;

            var animComp = entity.GetComponent<AnimationComponent>();
            animComp.IsPlaying = false;
        }

        /// <summary>
        /// Pauses the current animation
        /// </summary>
        public static void PauseAnimation(EntidadPadre entity)
        {
            if (!entity.HasComponent<AnimationComponent>())
                return;

            var animComp = entity.GetComponent<AnimationComponent>();
            animComp.IsPlaying = false;
        }

        /// <summary>
        /// Resumes the current animation
        /// </summary>
        public static void ResumeAnimation(EntidadPadre entity)
        {
            if (!entity.HasComponent<AnimationComponent>())
                return;

            var animComp = entity.GetComponent<AnimationComponent>();
            if (animComp.CurrentAnimation != null)
                animComp.IsPlaying = true;
        }
    }
}

namespace MyEngine_Core.ECS.MyComponents
{
    /// <summary>
    /// Enhanced animation component with multiple animations support
    /// </summary>
    public class AnimationComponent
    {
        // Animation state
        public Dictionary<string, Animation> Animations { get; set; }
        public Animation CurrentAnimation { get; set; }
        public int CurrentFrame { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public bool IsPlaying { get; set; }
        public bool IsLooping { get; set; }

        // Animation events
        public Action<EntidadPadre> OnAnimationComplete { get; set; }
        public Action<EntidadPadre> OnAnimationLoop { get; set; }

        public AnimationComponent()
        {
            Animations = new Dictionary<string, Animation>();
            IsLooping = true;
        }

        /// <summary>
        /// Adds an animation to this component
        /// </summary>
        public void AddAnimation(string name, Animation animation)
        {
            Animations[name] = animation;
        }

        /// <summary>
        /// Creates and adds an animation from a spritesheet
        /// </summary>
        public void AddAnimationFromSpriteSheet(string name, TextureRegion baseRegion,
            int frameCount, int frameWidth, int frameHeight, TimeSpan delay)
        {
            var frames = new List<TextureRegion>();

            for (int i = 0; i < frameCount; i++)
            {
                int x = baseRegion.SourceRectangle.X + (i * frameWidth);
                int y = baseRegion.SourceRectangle.Y;

                frames.Add(new TextureRegion(
                    baseRegion.Texture,
                    x, y,
                    frameWidth, frameHeight));
            }

            Animations[name] = new Animation(frames, delay);
        }
    }
}