using System;
using System.Collections.Generic;

namespace MyEngineAPI.Resources.Helpers
{
    public static class ComponentHelpers
    {
        #region SpriteComponent Helper

        public static class SpriteComponentHelper
        {
            public static void Create(XMLGameManager manager, int entityId, string spriteName, string path)
            {
                var attributes = new Dictionary<string, string>
                {
                    {"spriteName", spriteName},
                    {"path", path}
                };

                manager.AddComponent(entityId, "SpriteComponent", attributes);
            }

            public static ComponentInfo Read(XMLGameManager manager, int entityId)
            {
                var components = manager.ListEntityComponents(entityId);
                return components.Find(c => c.Type == "SpriteComponent");
            }

            public static bool Update(XMLGameManager manager, int entityId, string spriteName = null, string path = null)
            {
                var attributes = new Dictionary<string, string>();

                if (!string.IsNullOrEmpty(spriteName))
                    attributes["spriteName"] = spriteName;

                if (!string.IsNullOrEmpty(path))
                    attributes["path"] = path;

                if (attributes.Count > 0)
                    return manager.EditComponent(entityId, "SpriteComponent", attributes);

                return false;
            }

            public static bool Delete(XMLGameManager manager, int entityId)
            {
                return manager.RemoveComponent(entityId, "SpriteComponent");
            }
        }

        #endregion

        #region AnimationComponent Helper

        public static class AnimationComponentHelper
        {
            public static void Create(XMLGameManager manager, int entityId, string animationName, string path, int sceneId, int targetEntityId)
            {
                var attributes = new Dictionary<string, string>
                {
                    {"animationName", animationName},
                    {"path", path},
                    {"sceneId", sceneId.ToString()},
                    {"entityId", targetEntityId.ToString()}
                };

                manager.AddComponent(entityId, "AnimationComponent", attributes);
            }

            public static ComponentInfo Read(XMLGameManager manager, int entityId)
            {
                var components = manager.ListEntityComponents(entityId);
                return components.Find(c => c.Type == "AnimationComponent");
            }

            public static bool Update(XMLGameManager manager, int entityId, string animationName = null, string path = null, int? sceneId = null, int? targetEntityId = null)
            {
                var attributes = new Dictionary<string, string>();

                if (!string.IsNullOrEmpty(animationName))
                    attributes["animationName"] = animationName;

                if (!string.IsNullOrEmpty(path))
                    attributes["path"] = path;

                if (sceneId.HasValue)
                    attributes["sceneId"] = sceneId.Value.ToString();

                if (targetEntityId.HasValue)
                    attributes["entityId"] = targetEntityId.Value.ToString();

                if (attributes.Count > 0)
                    return manager.EditComponent(entityId, "AnimationComponent", attributes);

                return false;
            }

            public static bool Delete(XMLGameManager manager, int entityId)
            {
                return manager.RemoveComponent(entityId, "AnimationComponent");
            }
        }

        #endregion

        #region ButtonComponent Helper

        public static class ButtonComponentHelper
        {
            public static void Create(XMLGameManager manager, int entityId, string keyboardKey, string gamePadButton, float x, float y, float width, float height)
            {
                var attributes = new Dictionary<string, string>
                {
                    {"keyboardKey", keyboardKey},
                    {"gamePadButton", gamePadButton},
                    {"x", x.ToString()},
                    {"y", y.ToString()},
                    {"width", width.ToString()},
                    {"height", height.ToString()}
                };

                manager.AddComponent(entityId, "ButtonComponent", attributes);
            }

            public static ComponentInfo Read(XMLGameManager manager, int entityId)
            {
                var components = manager.ListEntityComponents(entityId);
                return components.Find(c => c.Type == "ButtonComponent");
            }

            public static bool Update(XMLGameManager manager, int entityId, string keyboardKey = null, string gamePadButton = null,
                float? x = null, float? y = null, float? width = null, float? height = null)
            {
                var attributes = new Dictionary<string, string>();

                if (!string.IsNullOrEmpty(keyboardKey))
                    attributes["keyboardKey"] = keyboardKey;

                if (!string.IsNullOrEmpty(gamePadButton))
                    attributes["gamePadButton"] = gamePadButton;

                if (x.HasValue)
                    attributes["x"] = x.Value.ToString();

                if (y.HasValue)
                    attributes["y"] = y.Value.ToString();

                if (width.HasValue)
                    attributes["width"] = width.Value.ToString();

                if (height.HasValue)
                    attributes["height"] = height.Value.ToString();

                if (attributes.Count > 0)
                    return manager.EditComponent(entityId, "ButtonComponent", attributes);

                return false;
            }

            public static bool Delete(XMLGameManager manager, int entityId)
            {
                return manager.RemoveComponent(entityId, "ButtonComponent");
            }
        }

        #endregion

        #region ColliderComponent Helper

        public static class ColliderComponentHelper
        {
            public static void Create(XMLGameManager manager, int entityId, float sizeWidth, float sizeHeight,
                float offsetX, float offsetY, bool isTrigger, string tag, int layerId, int cmaskId, bool isEnabled)
            {
                var childElements = new Dictionary<string, Dictionary<string, string>>
                {
                    {"size", new Dictionary<string, string> {{"width", sizeWidth.ToString()}, {"height", sizeHeight.ToString()}}},
                    {"offset", new Dictionary<string, string> {{"x", offsetX.ToString()}, {"y", offsetY.ToString()}}},
                    {"isTrigger", new Dictionary<string, string> {{"value", isTrigger.ToString().ToLower()}}},
                    {"tag", new Dictionary<string, string> {{"name", tag}}},
                    {"layer", new Dictionary<string, string> {{"id", layerId.ToString()}}},
                    {"cmask", new Dictionary<string, string> {{"id", cmaskId.ToString()}}},
                    {"isEnabled", new Dictionary<string, string> {{"value", isEnabled.ToString().ToLower()}}}
                };

                manager.AddComponent(entityId, "ColliderComponent", null, childElements);
            }

            public static ComponentInfo Read(XMLGameManager manager, int entityId)
            {
                var components = manager.ListEntityComponents(entityId);
                return components.Find(c => c.Type == "ColliderComponent");
            }

            public static bool Update(XMLGameManager manager, int entityId, float? sizeWidth = null, float? sizeHeight = null,
                float? offsetX = null, float? offsetY = null, bool? isTrigger = null, string tag = null,
                int? layerId = null, int? cmaskId = null, bool? isEnabled = null)
            {
                var childElements = new Dictionary<string, Dictionary<string, string>>();

                if (sizeWidth.HasValue || sizeHeight.HasValue)
                {
                    var currentComponent = Read(manager, entityId);
                    var currentSizeWidth = sizeWidth ?? float.Parse(currentComponent.ChildElements["size"]["width"]);
                    var currentSizeHeight = sizeHeight ?? float.Parse(currentComponent.ChildElements["size"]["height"]);
                    childElements["size"] = new Dictionary<string, string>
                    {
                        {"width", currentSizeWidth.ToString()},
                        {"height", currentSizeHeight.ToString()}
                    };
                }

                if (offsetX.HasValue || offsetY.HasValue)
                {
                    var currentComponent = Read(manager, entityId);
                    var currentOffsetX = offsetX ?? float.Parse(currentComponent.ChildElements["offset"]["x"]);
                    var currentOffsetY = offsetY ?? float.Parse(currentComponent.ChildElements["offset"]["y"]);
                    childElements["offset"] = new Dictionary<string, string>
                    {
                        {"x", currentOffsetX.ToString()},
                        {"y", currentOffsetY.ToString()}
                    };
                }

                if (isTrigger.HasValue)
                    childElements["isTrigger"] = new Dictionary<string, string> { { "value", isTrigger.Value.ToString().ToLower() } };

                if (!string.IsNullOrEmpty(tag))
                    childElements["tag"] = new Dictionary<string, string> { { "name", tag } };

                if (layerId.HasValue)
                    childElements["layer"] = new Dictionary<string, string> { { "id", layerId.Value.ToString() } };

                if (cmaskId.HasValue)
                    childElements["cmask"] = new Dictionary<string, string> { { "id", cmaskId.Value.ToString() } };

                if (isEnabled.HasValue)
                    childElements["isEnabled"] = new Dictionary<string, string> { { "value", isEnabled.Value.ToString().ToLower() } };

                if (childElements.Count > 0)
                    return manager.EditComponent(entityId, "ColliderComponent", null, childElements);

                return false;
            }

            public static bool Delete(XMLGameManager manager, int entityId)
            {
                return manager.RemoveComponent(entityId, "ColliderComponent");
            }
        }

        #endregion

        #region TransformComponent Helper

        public static class TransformComponentHelper
        {
            public static void Create(XMLGameManager manager, int entityId, float positionX, float positionY,
                float scaleX, float scaleY, float rotationAngle)
            {
                var childElements = new Dictionary<string, Dictionary<string, string>>
                {
                    {"position", new Dictionary<string, string> {{"x", positionX.ToString()}, {"y", positionY.ToString()}}},
                    {"scale", new Dictionary<string, string> {{"x", scaleX.ToString()}, {"y", scaleY.ToString()}}},
                    {"rotation", new Dictionary<string, string> {{"angle", rotationAngle.ToString()}}}
                };

                manager.AddComponent(entityId, "TransformComponent", null, childElements);
            }

            public static ComponentInfo Read(XMLGameManager manager, int entityId)
            {
                var components = manager.ListEntityComponents(entityId);
                return components.Find(c => c.Type == "TransformComponent");
            }

            public static bool Update(XMLGameManager manager, int entityId, float? positionX = null, float? positionY = null,
                float? scaleX = null, float? scaleY = null, float? rotationAngle = null)
            {
                var childElements = new Dictionary<string, Dictionary<string, string>>();

                if (positionX.HasValue || positionY.HasValue)
                {
                    var currentComponent = Read(manager, entityId);
                    var currentPosX = positionX ?? float.Parse(currentComponent.ChildElements["position"]["x"]);
                    var currentPosY = positionY ?? float.Parse(currentComponent.ChildElements["position"]["y"]);
                    childElements["position"] = new Dictionary<string, string>
                    {
                        {"x", currentPosX.ToString()},
                        {"y", currentPosY.ToString()}
                    };
                }

                if (scaleX.HasValue || scaleY.HasValue)
                {
                    var currentComponent = Read(manager, entityId);
                    var currentScaleX = scaleX ?? float.Parse(currentComponent.ChildElements["scale"]["x"]);
                    var currentScaleY = scaleY ?? float.Parse(currentComponent.ChildElements["scale"]["y"]);
                    childElements["scale"] = new Dictionary<string, string>
                    {
                        {"x", currentScaleX.ToString()},
                        {"y", currentScaleY.ToString()}
                    };
                }

                if (rotationAngle.HasValue)
                    childElements["rotation"] = new Dictionary<string, string> { { "angle", rotationAngle.Value.ToString() } };

                if (childElements.Count > 0)
                    return manager.EditComponent(entityId, "TransformComponent", null, childElements);

                return false;
            }

            public static bool Delete(XMLGameManager manager, int entityId)
            {
                return manager.RemoveComponent(entityId, "TransformComponent");
            }
        }

        #endregion

        #region InputComponent Helper

        public static class InputComponentHelper
        {
            public static void Create(XMLGameManager manager, int entityId, bool mouse, bool gamePad, bool keyboard, bool isEnabled, int gamePadIndex)
            {
                var attributes = new Dictionary<string, string>
                {
                    {"mouse", mouse.ToString().ToLower()},
                    {"gamePad", gamePad.ToString().ToLower()},
                    {"keyboard", keyboard.ToString().ToLower()},
                    {"isEnabled", isEnabled.ToString().ToLower()},
                    {"gamePadIndex", gamePadIndex.ToString()}
                };

                manager.AddComponent(entityId, "InputComponent", attributes);
            }

            public static ComponentInfo Read(XMLGameManager manager, int entityId)
            {
                var components = manager.ListEntityComponents(entityId);
                return components.Find(c => c.Type == "InputComponent");
            }

            public static bool Update(XMLGameManager manager, int entityId, bool? mouse = null, bool? gamePad = null,
                bool? keyboard = null, bool? isEnabled = null, int? gamePadIndex = null)
            {
                var attributes = new Dictionary<string, string>();

                if (mouse.HasValue)
                    attributes["mouse"] = mouse.Value.ToString().ToLower();

                if (gamePad.HasValue)
                    attributes["gamePad"] = gamePad.Value.ToString().ToLower();

                if (keyboard.HasValue)
                    attributes["keyboard"] = keyboard.Value.ToString().ToLower();

                if (isEnabled.HasValue)
                    attributes["isEnabled"] = isEnabled.Value.ToString().ToLower();

                if (gamePadIndex.HasValue)
                    attributes["gamePadIndex"] = gamePadIndex.Value.ToString();

                if (attributes.Count > 0)
                    return manager.EditComponent(entityId, "InputComponent", attributes);

                return false;
            }

            public static bool Delete(XMLGameManager manager, int entityId)
            {
                return manager.RemoveComponent(entityId, "InputComponent");
            }
        }

        #endregion

        #region LifeComponent Helper

        public static class LifeComponentHelper
        {
            public static void Create(XMLGameManager manager, int entityId, int maxHealth)
            {
                var attributes = new Dictionary<string, string>
                {
                    {"maxHealth", maxHealth.ToString()}
                };

                manager.AddComponent(entityId, "LifeComponent", attributes);
            }

            public static ComponentInfo Read(XMLGameManager manager, int entityId)
            {
                var components = manager.ListEntityComponents(entityId);
                return components.Find(c => c.Type == "LifeComponent");
            }

            public static bool Update(XMLGameManager manager, int entityId, int? maxHealth = null)
            {
                var attributes = new Dictionary<string, string>();

                if (maxHealth.HasValue)
                    attributes["maxHealth"] = maxHealth.Value.ToString();

                if (attributes.Count > 0)
                    return manager.EditComponent(entityId, "LifeComponent", attributes);

                return false;
            }

            public static bool Delete(XMLGameManager manager, int entityId)
            {
                return manager.RemoveComponent(entityId, "LifeComponent");
            }
        }

        #endregion

        #region RigidbodyComponent Helper

        public static class RigidbodyComponentHelper
        {
            public static void Create(XMLGameManager manager, int entityId, float mass, float acceleration, float velocityX, float velocityY)
            {
                var attributes = new Dictionary<string, string>
                {
                    {"mass", mass.ToString()},
                    {"acceleration", acceleration.ToString()},
                    {"velocityX", velocityX.ToString()},
                    {"velocityY", velocityY.ToString()}
                };

                manager.AddComponent(entityId, "RigidbodyComponent", attributes);
            }

            public static ComponentInfo Read(XMLGameManager manager, int entityId)
            {
                var components = manager.ListEntityComponents(entityId);
                return components.Find(c => c.Type == "RigidbodyComponent");
            }

            public static bool Update(XMLGameManager manager, int entityId, float? mass = null, float? acceleration = null,
                float? velocityX = null, float? velocityY = null)
            {
                var attributes = new Dictionary<string, string>();

                if (mass.HasValue)
                    attributes["mass"] = mass.Value.ToString();

                if (acceleration.HasValue)
                    attributes["acceleration"] = acceleration.Value.ToString();

                if (velocityX.HasValue)
                    attributes["velocityX"] = velocityX.Value.ToString();

                if (velocityY.HasValue)
                    attributes["velocityY"] = velocityY.Value.ToString();

                if (attributes.Count > 0)
                    return manager.EditComponent(entityId, "RigidbodyComponent", attributes);

                return false;
            }

            public static bool Delete(XMLGameManager manager, int entityId)
            {
                return manager.RemoveComponent(entityId, "RigidbodyComponent");
            }
        }

        #endregion

        #region TilemapComponent Helper

        public static class TilemapComponentHelper
        {
            public static void Create(XMLGameManager manager, int entityId, string path)
            {
                var attributes = new Dictionary<string, string>
                {
                    {"path", path}
                };

                manager.AddComponent(entityId, "TilemapComponent", attributes);
            }

            public static ComponentInfo Read(XMLGameManager manager, int entityId)
            {
                var components = manager.ListEntityComponents(entityId);
                return components.Find(c => c.Type == "TilemapComponent");
            }

            public static bool Update(XMLGameManager manager, int entityId, string path = null)
            {
                var attributes = new Dictionary<string, string>();

                if (!string.IsNullOrEmpty(path))
                    attributes["path"] = path;

                if (attributes.Count > 0)
                    return manager.EditComponent(entityId, "TilemapComponent", attributes);

                return false;
            }

            public static bool Delete(XMLGameManager manager, int entityId)
            {
                return manager.RemoveComponent(entityId, "TilemapComponent");
            }
        }

        #endregion

        #region TilemapCollisionsComponent Helper

        public static class TilemapCollisionsComponentHelper
        {
            public static void Create(XMLGameManager manager, int entityId, string path)
            {
                var attributes = new Dictionary<string, string>
                {
                    {"path", path}
                };

                manager.AddComponent(entityId, "TilemapCollisionsComponent", attributes);
            }

            public static ComponentInfo Read(XMLGameManager manager, int entityId)
            {
                var components = manager.ListEntityComponents(entityId);
                return components.Find(c => c.Type == "TilemapCollisionsComponent");
            }

            public static bool Update(XMLGameManager manager, int entityId, string path = null)
            {
                var attributes = new Dictionary<string, string>();

                if (!string.IsNullOrEmpty(path))
                    attributes["path"] = path;

                if (attributes.Count > 0)
                    return manager.EditComponent(entityId, "TilemapCollisionsComponent", attributes);

                return false;
            }

            public static bool Delete(XMLGameManager manager, int entityId)
            {
                return manager.RemoveComponent(entityId, "TilemapCollisionsComponent");
            }
        }

        #endregion
    }

    #region Usage Examples

    /*
    // Ejemplo de uso de los helpers:

    var manager = new XMLGameManager("Game.xml");

    // Crear SpriteComponent
    ComponentHelpers.SpriteComponentHelper.Create(manager, 1, "player_idle", "sprites/player_atlas");

    // Leer SpriteComponent
    var spriteComponent = ComponentHelpers.SpriteComponentHelper.Read(manager, 1);

    // Actualizar SpriteComponent (solo el spriteName)
    ComponentHelpers.SpriteComponentHelper.Update(manager, 1, spriteName: "player_run");

    // Eliminar SpriteComponent
    ComponentHelpers.SpriteComponentHelper.Delete(manager, 1);

    // Crear ColliderComponent completo
    ComponentHelpers.ColliderComponentHelper.Create(manager, 1, 32, 32, 0, 0, false, "Player", 0, 14, true);

    // Actualizar solo la posición del TransformComponent
    ComponentHelpers.TransformComponentHelper.Update(manager, 1, positionX: 150, positionY: 200);

    // Crear InputComponent
    ComponentHelpers.InputComponentHelper.Create(manager, 1, false, true, true, true, 0);

    // Guardar cambios
    manager.SaveXML();
    */

    #endregion
}