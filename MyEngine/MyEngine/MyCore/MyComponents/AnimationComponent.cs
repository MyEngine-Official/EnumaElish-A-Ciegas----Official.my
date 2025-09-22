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
        public Dictionary<AnimationAction, Animation> Animations { get; set; }
        public Tuple<AnimationAction, Animation> CurrentAnimation { get; set; }
        public void SetCurrentAnimation(AnimationAction action)
        {
            if (Animations.TryGetValue(action, out var animation))
            {
                if (CurrentAnimation == null || CurrentAnimation.Item1 != action)
                {
                    CurrentAnimation = new Tuple<AnimationAction, Animation>(action, animation);
                    CurrentFrame = 0;
                    ElapsedTime = TimeSpan.Zero;
                    IsPlaying = true;
                }
            }
            else
            {
                throw new Exception($"Animation '{action}' not found in the AnimationComponent.");
            }
        }
        public int CurrentFrame { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public bool IsPlaying { get; set; }
        public bool IsLooping { get; set; }
        public int LoopCount { get; set; } = 0;

        // Animation events
        public Action<MainEntity> OnAnimationComplete { get; set; }
        public Action<MainEntity> OnAnimationLoop { get; set; }

        public AnimationComponent()
        {
            Animations = new Dictionary<AnimationAction, Animation>();
            IsLooping = true;
        }

        /// <summary>
        /// Adds an animation to this component
        /// </summary>
        public void AddAnimation(AnimationAction name, Animation animation)
        {
            Animations[name] = animation;
        }

        /// <summary>
        /// Creates and adds an animation from a spritesheet
        /// </summary>
        public void AddAnimationFromSpriteSheet(AnimationAction name, TextureRegion baseRegion,
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

    public enum AnimationAction
    {
        JumpRigth,
        JumpUp,
        RunRight,
        RunUp,
        emote,
        Hability1Rigth,
        Hability1Up,
        Hability2Up,
        Hability2Rigth,
        Hability3Up,
        Hability3Rigth,
        StopRight,
        StopUp,
        Death,
        disapeared,
        hited,
        looting,

    }
}
