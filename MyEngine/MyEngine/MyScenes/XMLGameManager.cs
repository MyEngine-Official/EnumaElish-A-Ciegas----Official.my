using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace MyEngine.MyScenes
{
    public class XMLGameManager
    {
        private string _xmlFilePath;
        private XDocument _xmlDocument;

        public XMLGameManager(string xmlFilePath)
        {
            _xmlFilePath = xmlFilePath;
            LoadXML();
        }

        #region Base XML Operations

        /// <summary>
        /// Crea un archivo XML base con estructura inicial
        /// </summary>
        public void CreateBaseXML(string projectName, string projectPath, string contentPath)
        {
            var baseXML = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("GameProject",
                    new XAttribute("name", projectName),
                    new XAttribute("path", projectPath),
                    new XElement("ContentPath", new XAttribute("path", contentPath)),
                    new XElement("GameScenes", new XAttribute("id", "0"),
                        new XElement("GameEntities",
                            new XElement("entity",
                                new XAttribute("id", "1"),
                                new XAttribute("name", "TestEntity"),
                                new XElement("Components")
                            )
                        )
                    )
                )
            );

            baseXML.Save(_xmlFilePath);
            _xmlDocument = baseXML;
        }

        /// <summary>
        /// Carga el archivo XML
        /// </summary>
        private void LoadXML()
        {
            if (File.Exists(_xmlFilePath))
            {
                _xmlDocument = XDocument.Load(_xmlFilePath);
            }
            else
            {
                throw new FileNotFoundException($"No se encontró el archivo XML en: {_xmlFilePath}");
            }
        }

        /// <summary>
        /// Guarda los cambios en el archivo XML
        /// </summary>
        public void SaveXML()
        {
            _xmlDocument.Save(_xmlFilePath);
        }

        #endregion

        #region Scene Management

        /// <summary>
        /// Agrega una nueva escena
        /// </summary>
        public void AddScene(int sceneId)
        {
            var root = _xmlDocument.Root;
            var existingScene = root.Descendants("GameScenes").FirstOrDefault(s => (int)s.Attribute("id") == sceneId);

            if (existingScene != null)
                throw new InvalidOperationException($"La escena con ID {sceneId} ya existe.");

            var newScene = new XElement("GameScenes",
                new XAttribute("id", sceneId),
                new XElement("GameEntities")
            );

            root.Add(newScene);
        }

        /// <summary>
        /// Elimina una escena por ID
        /// </summary>
        public bool RemoveScene(int sceneId)
        {
            var scene = _xmlDocument.Root.Descendants("GameScenes")
                .FirstOrDefault(s => (int)s.Attribute("id") == sceneId);

            if (scene != null)
            {
                scene.Remove();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Lista todas las escenas
        /// </summary>
        public List<SceneInfo> ListScenes()
        {
            return _xmlDocument.Root.Descendants("GameScenes")
                .Select(scene => new SceneInfo
                {
                    Id = (int)scene.Attribute("id"),
                    EntitiesCount = scene.Descendants("entity").Count()
                })
                .ToList();
        }

        /// <summary>
        /// Obtiene una escena por ID
        /// </summary>
        public SceneInfo GetSceneById(int sceneId)
        {
            var scene = _xmlDocument.Root.Descendants("GameScenes")
                .FirstOrDefault(s => (int)s.Attribute("id") == sceneId);

            if (scene == null)
                return null;

            return new SceneInfo
            {
                Id = sceneId,
                EntitiesCount = scene.Descendants("entity").Count()
            };
        }

        #endregion

        #region Entity Management

        /// <summary>
        /// Agrega una nueva entidad a una escena con ID generado automáticamente
        /// </summary>
        public int AddEntity(int sceneId, string entityName)
        {
            var scene = GetSceneElement(sceneId);
            var entitiesContainer = scene.Element("GameEntities");

            // Generar ID secuencial basado en el ID más alto existente
            var allEntities = _xmlDocument.Root.Descendants("entity");
            int nextId = 1;
            if (allEntities.Any())
            {
                nextId = allEntities.Max(e => (int)e.Attribute("id")) + 1;
            }

            var newEntity = new XElement("entity",
                new XAttribute("id", nextId),
                new XAttribute("name", entityName),
                new XElement("Components")
            );

            entitiesContainer.Add(newEntity);
            return nextId;
        }

        /// <summary>
        /// Agrega una nueva entidad a una escena con ID específico (método original)
        /// </summary>
        public void AddEntity(int sceneId, int entityId, string entityName)
        {
            // Verificar que el ID de entidad sea único globalmente
            var allEntities = _xmlDocument.Root.Descendants("entity");
            if (allEntities.Any(e => (int)e.Attribute("id") == entityId))
                throw new InvalidOperationException($"El ID de entidad {entityId} ya existe. Los IDs deben ser únicos globalmente.");

            var scene = GetSceneElement(sceneId);
            var entitiesContainer = scene.Element("GameEntities");

            var newEntity = new XElement("entity",
                new XAttribute("id", entityId),
                new XAttribute("name", entityName),
                new XElement("Components")
            );

            entitiesContainer.Add(newEntity);
        }

        /// <summary>
        /// Elimina una entidad por ID
        /// </summary>
        public bool RemoveEntity(int entityId)
        {
            var entity = _xmlDocument.Root.Descendants("entity")
                .FirstOrDefault(e => (int)e.Attribute("id") == entityId);

            if (entity != null)
            {
                entity.Remove();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Edita los datos básicos de una entidad
        /// </summary>
        public bool EditEntity(int entityId, string newName)
        {
            var entity = GetEntityElement(entityId);
            if (entity != null)
            {
                entity.Attribute("name").Value = newName;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Lista entidades por escena
        /// </summary>
        public List<EntityInfo> ListEntitiesByScene(int sceneId)
        {
            var scene = GetSceneElement(sceneId);
            if (scene == null) return new List<EntityInfo>();

            return scene.Descendants("entity")
                .Select(entity => new EntityInfo
                {
                    Id = (int)entity.Attribute("id"),
                    Name = entity.Attribute("name")?.Value ?? "",
                    SceneId = sceneId,
                    ComponentsCount = entity.Element("Components")?.Elements().Count() ?? 0
                })
                .ToList();
        }

        /// <summary>
        /// Obtiene una entidad por ID
        /// </summary>
        public EntityInfo GetEntityById(int entityId)
        {
            var entity = _xmlDocument.Root.Descendants("entity")
                .FirstOrDefault(e => (int)e.Attribute("id") == entityId);

            if (entity == null) return null;

            // Encontrar la escena de esta entidad
            var scene = entity.Ancestors("GameScenes").First();
            int sceneId = (int)scene.Attribute("id");

            return new EntityInfo
            {
                Id = entityId,
                Name = entity.Attribute("name")?.Value ?? "",
                SceneId = sceneId,
                ComponentsCount = entity.Element("Components")?.Elements().Count() ?? 0
            };
        }

        #endregion

        #region Component Management

        /// <summary>
        /// Agrega un componente a una entidad
        /// </summary>
        public void AddComponent(int entityId, string componentType, Dictionary<string, string> attributes = null, Dictionary<string, Dictionary<string, string>> childElements = null)
        {
            var entity = GetEntityElement(entityId);
            var componentsContainer = entity.Element("Components");

            // Verificar si ya existe este tipo de componente
            if (componentsContainer.Elements(componentType).Any())
                throw new InvalidOperationException($"La entidad {entityId} ya tiene un componente de tipo {componentType}");

            var component = new XElement(componentType, new XAttribute("active", "true"));

            // Agregar atributos
            if (attributes != null)
            {
                foreach (var attr in attributes)
                {
                    component.SetAttributeValue(attr.Key, attr.Value);
                }
            }

            // Agregar elementos hijos
            if (childElements != null)
            {
                foreach (var child in childElements)
                {
                    var childElement = new XElement(child.Key);
                    foreach (var childAttr in child.Value)
                    {
                        childElement.SetAttributeValue(childAttr.Key, childAttr.Value);
                    }
                    component.Add(childElement);
                }
            }

            componentsContainer.Add(component);
        }

        /// <summary>
        /// Elimina un componente de una entidad
        /// </summary>
        public bool RemoveComponent(int entityId, string componentType)
        {
            var entity = GetEntityElement(entityId);
            var component = entity.Element("Components")?.Element(componentType);

            if (component != null)
            {
                component.Remove();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Edita un componente de una entidad
        /// </summary>
        public bool EditComponent(int entityId, string componentType, Dictionary<string, string> newAttributes = null, Dictionary<string, Dictionary<string, string>> newChildElements = null)
        {
            var entity = GetEntityElement(entityId);
            var component = entity.Element("Components")?.Element(componentType);

            if (component == null) return false;

            // Actualizar atributos
            if (newAttributes != null)
            {
                foreach (var attr in newAttributes)
                {
                    component.SetAttributeValue(attr.Key, attr.Value);
                }
            }

            // Actualizar elementos hijos
            if (newChildElements != null)
            {
                // Remover elementos hijos existentes que van a ser actualizados
                foreach (var childName in newChildElements.Keys)
                {
                    component.Elements(childName).Remove();
                }

                // Agregar nuevos elementos hijos
                foreach (var child in newChildElements)
                {
                    var childElement = new XElement(child.Key);
                    foreach (var childAttr in child.Value)
                    {
                        childElement.SetAttributeValue(childAttr.Key, childAttr.Value);
                    }
                    component.Add(childElement);
                }
            }

            return true;
        }

        /// <summary>
        /// Lista componentes de una entidad
        /// </summary>
        public List<ComponentInfo> ListEntityComponents(int entityId)
        {
            var entity = GetEntityElement(entityId);
            if (entity == null) return new List<ComponentInfo>();

            var components = entity.Element("Components");
            if (components == null) return new List<ComponentInfo>();

            return components.Elements()
                .Select(comp => new ComponentInfo
                {
                    Type = comp.Name.LocalName,
                    Active = bool.Parse(comp.Attribute("active")?.Value ?? "true"),
                    Attributes = comp.Attributes().ToDictionary(a => a.Name.LocalName, a => a.Value),
                    ChildElements = comp.Elements().ToDictionary(
                        e => e.Name.LocalName,
                        e => e.Attributes().ToDictionary(a => a.Name.LocalName, a => a.Value)
                    )
                })
                .ToList();
        }

        #endregion

        #region Helper Methods

        private XElement GetSceneElement(int sceneId)
        {
            var scene = _xmlDocument.Root.Descendants("GameScenes")
                .FirstOrDefault(s => (int)s.Attribute("id") == sceneId);

            if (scene == null)
                throw new InvalidOperationException($"No se encontró la escena con ID {sceneId}");

            return scene;
        }

        private XElement GetEntityElement(int entityId)
        {
            var entity = _xmlDocument.Root.Descendants("entity")
                .FirstOrDefault(e => (int)e.Attribute("id") == entityId);

            if (entity == null)
                throw new InvalidOperationException($"No se encontró la entidad con ID {entityId}");

            return entity;
        }

        #endregion
    }

    #region Data Classes

    public class SceneInfo
    {
        public int Id { get; set; }
        public int EntitiesCount { get; set; }
    }

    public class EntityInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SceneId { get; set; }
        public int ComponentsCount { get; set; }
    }

    public class ComponentInfo
    {
        public string Type { get; set; }
        public bool Active { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public Dictionary<string, Dictionary<string, string>> ChildElements { get; set; }
    }

    #endregion
}