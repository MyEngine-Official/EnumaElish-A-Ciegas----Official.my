using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MyEngine.MyCore.MyComponents
{
    public class TransformComponent
    {
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public Vector2 Scale { get; set; } = Vector2.One;
        public void Y(float y)
        {
            Position = new Vector2(Position.X, y);
        }

        public float Y()
        {
            return Position.Y;
        }

        public void X(float x)
        {
            Position = new Vector2(x, Position.Y);
        }

        public float X()
        {
            return Position.X;
        }

        public void SetPosition(Vector2 position)
        {
            Position = position;
        }

        public void MoverIzquierda(float movement)
        {
            X(Position.X - movement);
        }

        public void MoverDerecha(float movement)
        {
            X(Position.X + movement);
        }

        public void MoverArriba(float movement)
        {
            X(Position.Y - movement);
        }

        public void MoverAbajo(float movement)
        {
            X(Position.Y + movement);
        }
    }
}
