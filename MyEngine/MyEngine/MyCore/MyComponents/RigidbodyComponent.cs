using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MyEngine.MyCore.MyComponents
{
    public class RigidbodyComponent
    {
        public Vector2 Velocity { get; set; } = Vector2.Zero;
        public float Mass { get; set; } = 0f;
        public float Acceleration { get; set; } = 0f;
    }
}
