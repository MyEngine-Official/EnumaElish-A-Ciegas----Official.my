using System;
using MyEngine_Core.MyGraphics;

namespace MyEngine_Core.ECS.MyComponents
{
    public class AnimationComponent
    {
        public Animation Animation { get; set; }
        public int CurrentFrame { get; set; }
        public TimeSpan ElapsedTime { get; set; }
    }
}
