using System;
using System.Collections.Generic;
using MyEngine.MyCore.MyEntities;
using MyEngine.MyGraphics;

namespace MyEngine.MyCore.MyComponents
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
        public Action<MainEntity> OnAnimationComplete { get; set; }
        public Action<MainEntity> OnAnimationLoop { get; set; }

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
