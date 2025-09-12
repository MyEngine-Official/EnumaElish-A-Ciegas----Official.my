using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyEngine.MyGraphics
{
    public class Camara2D
    {
        public Vector2 Position { get; set; }
        public Vector2 Scale { get; set; } = Vector2.One;
        public Viewport Viewport { get; set; }
        public float Rotation { get; set; }
        public Vector2 Origin { get; set; }
        public Rectangle? Bounds { get; set; }
        
        /// <summary>
        /// Creates a new camera with the specified viewport
        /// </summary>
        public Camara2D(Viewport viewport)
        {
            Position = Vector2.Zero;
            Scale = Vector2.One;
            Viewport = viewport;
            Rotation = 0f;
            Origin = new Vector2(viewport.Width / 2f, viewport.Height / 2f);
        }
        
        /// <summary>
        /// Creates a new camera with default settings
        /// </summary>
        public Camara2D()
        {
            Position = Vector2.Zero;
            Scale = Vector2.One;
            Rotation = 0f;
            Origin = Vector2.Zero;
        }

        public Matrix GetTransform()
        {
            // Build the transformation matrix
            return Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0)) *
                   Matrix.CreateRotationZ(Rotation) *
                   Matrix.CreateScale(new Vector3(Scale.X, Scale.Y, 1)) *
                   Matrix.CreateTranslation(new Vector3(Origin.X, Origin.Y, 0));
        }
        
        /// <summary>
        /// Centers the camera on the given position
        /// </summary>
        public void CenterOn(Vector2 position)
        {
            Position = position - Origin;
        }
        
        /// <summary>
        /// Applies boundaries to keep the camera within limits
        /// </summary>
        public void ApplyBounds()
        {
            if (Bounds.HasValue)
            {
                Position = Vector2.Clamp(Position, 
                    new Vector2(Bounds.Value.Left, Bounds.Value.Top),
                    new Vector2(Bounds.Value.Right - Viewport.Width / Scale.X, 
                               Bounds.Value.Bottom - Viewport.Height / Scale.Y));
            }
        }
    }
}
