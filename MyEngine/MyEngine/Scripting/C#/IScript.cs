using Microsoft.Xna.Framework; // Para GameTime
using MyEngine.MyCore.MyEntities;

namespace MyEngine.Scripting
{
    /// <summary>
    /// Interface for scriptable objects in the engine.
    /// </summary>
    public interface IScript
    {
        // EntityIdAsociado
        int EntityId { get; set; }

        /// <summary>
        /// Initializes the script.
        /// </summary>
        void Initialize(MainEntity entity);

        /// <summary>
        /// Updates the script logic.
        /// </summary>
        void Update(GameTime gametime, MainEntity entity);

        /// <summary>
        /// Cleans up resources used by the script.
        /// </summary>
        void Cleanup();
    }
}