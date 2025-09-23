using Microsoft.Xna.Framework;
using MyEngine.MyCore.MyEntities;
using MyEngine.MyCore.MyComponents;
using MyEngine.Scripting;
using System;

namespace TestingCrossPlatformGame.Core.Content.Scripts
{
    public class PlayerScript : IScript
    {
        public int EntityId { get; set; }

        public void Initialize(MainEntity entity)
        {
            EntityId = entity.Id;
            entity.AddComponent(new TransformComponent(Vector2.Zero, 4, Vector2.One)); 
        }

        public void Update(GameTime gametime, MainEntity entity)
        {
            // Ejemplo: si la entidad tiene un componente "Health"
            if (!entity.HasComponent<TransformComponent>())
            {
                TransformComponent transform = entity.GetComponent<TransformComponent>();

                

            }
            else
            {
                
            }
        }

        public void Cleanup()
        {
            GC.Collect();
        }

    }


}
