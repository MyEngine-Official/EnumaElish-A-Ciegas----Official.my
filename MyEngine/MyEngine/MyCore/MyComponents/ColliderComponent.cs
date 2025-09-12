using System;
using Microsoft.Xna.Framework;

namespace MyEngine.MyCore.MyComponents
{
    /// <summary>
    /// Component for collision detection
    /// </summary>
    public class ColliderComponent
    {
        /// <summary>
        /// Tamaño del "hitbox" o caja de colisión (ancho y alto).
        /// </summary>
        public Vector2 Size { get; set; }

        /// <summary>
        /// Permite mover la caja respecto al centro o posición de la entidad (ej: si quieres que el collider esté un poco más abajo que el sprite).
        /// </summary>
        public Vector2 Offset { get; set; }
        
        /// <summary>
        /// false  es una colisión "sólida", choca físicamente (ej: una pared).true → es un "sensor", detecta la colisión pero no bloquea(ej: recoger un ítem, zona de daño, checkpoint).
        /// </summary>
        public bool IsTrigger { get; set; }

        /// <summary>
        /// Una etiqueta para identificar el tipo ("Player", "Enemy", "Pickup", etc.).
        /// </summary>
        public string Tag { get; set; }
        
        /// <summary>
        /// Layer for collision filtering
        /// </summary>
        public CollisionLayer Layer { get; set; }

        /// <summary>
        /// En qué capa está este collider (ej: 0 = jugador, 1 = enemigos, 2 = balas, etc.).
        /// </summary>
        public CollisionLayer CollisionMask { get; set; }

        /// <summary>
        /// Máscara que indica con qué capas puede colisionar.

        ///        Por defecto -1 significa "con todas".

        ///Se maneja con bits, por ejemplo:

        ///Layer = 0 (Player)

        ///CollisionMask = (1 << 1) → solo choca con Layer 1 (enemigos).
        /// </summary>

        ///<summary>
        ///Si false, el collider se ignora.
        ///</summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Creates a new collider component
        /// </summary>
        /// <param name="width">Width of the collider</param>
        /// <param name="height">Height of the collider</param>
        public ColliderComponent(float width, float height)
        {
            Size = new Vector2(width, height);
            Offset = Vector2.Zero;
            IsTrigger = false;
            Tag = "Default";
            Layer = 0;
            CollisionMask = CollisionLayer.All; // Collide with all layers by default
            IsEnabled = true;
        }
        
        /// <summary>
        /// Creates a new collider component with size
        /// </summary>
        /// <param name="size">Size of the collider</param>
        public ColliderComponent(Vector2 size)
        {
            Size = size;
            Offset = Vector2.Zero;
            IsTrigger = false;
            Tag = "Default";
            Layer = 0;
            CollisionMask = CollisionLayer.All;
            IsEnabled = true;
        }
        
        /// <summary>
        /// Gets the bounding rectangle for this collider at the given position
        /// </summary>
        public Rectangle GetBounds(Vector2 position, Vector2 scale)
        {
            return new Rectangle(
                (int)(position.X + Offset.X * scale.X),
                (int)(position.Y + Offset.Y * scale.Y),
                (int)(Size.X * scale.X),
                (int)(Size.Y * scale.Y)
            );
        }
        
        /// <summary>
        /// Checks if this collider can collide with another based on layers
        /// </summary>
        public bool CanCollideWith(ColliderComponent other)
        {
            if (!IsEnabled || !other.IsEnabled)
                return false;

            return (CollisionMask & other.Layer) != 0;
        }
    }

    [Flags]
    public enum CollisionLayer
    {
        None = 0,
        Player = 1 << 0, // 0001 = 1
        Enemy = 1 << 1, // 0010 = 2
        Item = 1 << 2, // 0100 = 4
        Bullet = 1 << 3, // 1000 = 8
                        // etc.
        All = ~0       // todos los bits en 1
    }

}
