using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MonoGameLibrary.Graphics
{
    public class Camera2D
    {
        public Vector2 Position { get; set; }
        public Vector2 Scale { get; set; } = Vector2.One;

        public Matrix GetTransform()
        {
            // 1. Escalado   2. Traslación inversa
            return Matrix.CreateScale(new Vector3(Scale, 1f))
                 * Matrix.CreateTranslation(new Vector3(-Position, 0f));
        }
    }
}
