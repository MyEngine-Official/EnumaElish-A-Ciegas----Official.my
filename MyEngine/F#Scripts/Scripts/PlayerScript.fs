namespace TestingCrossPlatformGame.Core.Content.Scripts

open Microsoft.Xna.Framework
open MyEngine.MyCore.MyEntities
open MyEngine.MyCore.MyComponents
open MyEngine.Scripting
open System


type PlayerScript() =
    interface IScript with
        member this.EntityId
            with get (): int = 
                raise (System.NotImplementedException())
            and set (v: int): unit = 
                raise (System.NotImplementedException())
   


        member this.Initialize(entity: MainEntity) =
            entity.AddComponent(new TransformComponent(Vector2.Zero, 4f, Vector2.One))


        member this.Update(gameTime: GameTime, entity: MainEntity) =
            if entity.HasComponent<TransformComponent>() then
                let transform = entity.GetComponent<TransformComponent>()
                // Aquí pondrías lógica para mover/actualizar transform
                ()
            else
                ()

        member this.Cleanup() =
            GC.Collect()



