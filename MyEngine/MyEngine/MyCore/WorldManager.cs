using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MyEngine.MyCore.Events;
using MyEngine.MyCore.MyComponents;
using MyEngine.MyCore.MyEntities;
using MyEngine.MyCore.MySystems;

namespace MyEngine.MyCore
{
    /// <summary>
    /// Manages all entities and provides query capabilities for systems
    /// </summary>
    public class WorldManager
    {
        private readonly List<MainEntity> _entities = new();
        private readonly Dictionary<Type, ISystem> _systems = new();
        private int _nextEntityId = 0;
        private readonly Queue<int> _recycledIds = new();

        private readonly EventBus _eventBus = new();

        /// <summary>
        /// Acceso p�blico al EventBus
        /// </summary>
        public EventBus Events => _eventBus;
        /// <summary>
        /// Creates a new entity in the world
        /// </summary>
        public MainEntity CreateEntity(string entityType = "Unknown")
        {
            int id = _recycledIds.Count > 0 ? _recycledIds.Dequeue() : _nextEntityId++;
            var entity = new MainEntity(id);
            _entities.Add(entity);

            // Publicar evento de creaci�n
            _eventBus.Publish(new EntitySpawnedEvent
            {
                EntityId = id,
                EntityType = entityType,
                Position = Vector2.Zero
            });

            return entity;
        }

        /// <summary>
        /// Removes an entity from the world
        /// </summary>
        //  NUEVO: Remover entidad con evento
        public void RemoveEntity(MainEntity entity)
        {
            var transform = entity.HasComponent<TransformComponent>()
                ? entity.GetComponent<TransformComponent>()
                : null;

            _eventBus.Publish(new EntityDestroyedEvent
            {
                EntityId = entity.Id,
                EntityType = "Unknown", // Podr�as agregar un TypeComponent
                LastPosition = transform?.Position ?? Vector2.Zero
            });

            _entities.Remove(entity);
            _recycledIds.Enqueue(entity.Id);
        }

        /// <summary>
        /// Gets all entities in the world
        /// </summary>
        public List<MainEntity> GetAllEntities()
        {
            return _entities;
        }

        /// <summary>
        /// Gets entities that have specific components
        /// </summary>
        public List<MainEntity> GetEntitiesWithComponents<T1>()
        {
            return _entities.Where(e => e.HasComponent<T1>()).ToList();
        }

        /// <summary>
        /// Gets entities that have two specific components
        /// </summary>
        public List<MainEntity> GetEntitiesWithComponents<T1, T2>()
        {
            return _entities.Where(e => e.HasComponent<T1>() && e.HasComponent<T2>()).ToList();
        }

        /// <summary>
        /// Gets entities that have three specific components
        /// </summary>
        public List<MainEntity> GetEntitiesWithComponents<T1, T2, T3>()
        {
            return _entities.Where(e => 
                e.HasComponent<T1>() && 
                e.HasComponent<T2>() && 
                e.HasComponent<T3>()).ToList();
        }

        /// <summary>
        /// Registers a system to the world
        /// </summary>
        public void RegisterSystem<T>(T system) where T : ISystem
        {
            _systems[typeof(T)] = system;
            system.Initialize(this); // Los sistemas reciben referencia al WorldManager

            //  Permitir que sistemas se suscriban a eventos durante inicializaci�n
            if (system is IEventSubscriber eventSubscriber)
                eventSubscriber.SubscribeToEvents(_eventBus);
        }

        //  NUEVO: Update mejorado con eventos
        public void UpdateSystems(GameTime gameTime)
        {
            // 1. Procesar input
            GetSystem<InputSystem>()?.Update(gameTime);

            // 2. Procesar eventos encolados del frame anterior
            _eventBus.ProcessQueuedEvents();

            // 3. Actualizar f�sica (puede generar eventos de colisi�n)
            GetSystem<PhysicsSystem>()?.Update(gameTime);

            // 4. Procesar eventos de f�sica
            _eventBus.ProcessQueuedEvents();

            // 5. Actualizar animaciones
            GetSystem<AnimationSystem>()?.Update(gameTime);

            // 6. Procesar botones y UI
            GetSystem<ButtonSystem>()?.Update(gameTime);

            // 7. Procesar eventos finales del frame
            _eventBus.ProcessQueuedEvents();
            
            // Nota: RenderSystem no se actualiza aquí, tiene su propio método Draw()
        }
        /// <summary>
        /// Gets a registered system
        /// </summary>
        public T GetSystem<T>() where T : ISystem
        {
            if (_systems.TryGetValue(typeof(T), out var system))
                return (T)system;
            
            throw new Exception($"System {typeof(T).Name} not registered");
        }

        /// <summary>
        /// Clears all entities from the world
        /// </summary>
        public void Clear()
        {
            _entities.Clear();
            _recycledIds.Clear();
            _nextEntityId = 0;
        }
    }

    /// <summary>
    /// Base interface for all systems
    /// </summary>
    public interface ISystem
    {
        void Initialize(WorldManager world);
    }

    // NUEVO: Interfaz para sistemas que usan eventos
    public interface IEventSubscriber
    {
        void SubscribeToEvents(EventBus eventBus);
    }
}