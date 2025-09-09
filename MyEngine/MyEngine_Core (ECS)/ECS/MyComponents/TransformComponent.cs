using Microsoft.Xna.Framework;

namespace MyEngine_Core.ECS.MyComponents
{
    public class TransformComponent
    {
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public Vector2 Scale { get; set; } = Vector2.One;
    }
}
