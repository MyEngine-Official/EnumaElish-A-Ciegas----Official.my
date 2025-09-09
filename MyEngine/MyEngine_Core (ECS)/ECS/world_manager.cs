using System;
using System.Collections.Generic;
using System.Linq;
using MyEngine_Core.ECS.MyEntities;

namespace MyEngine_Core.ECS
{
    /// <summary>
    /// Manages all entities and provides query capabilities for systems
    /// </summary>
    public class World
    {
        private readonly List<EntidadPadre> _entities = new();
        private readonly Dictionary<Type, ISystem> _systems = new();
        private int _nextEntityId = 0;
        private readonly Queue<int> _recycledIds = new();

        /// <summary>
        /// Creates a new entity in the world
        /// </summary>
        public EntidadPadre CreateEntity()
        {
            int id = _recycledIds.Count > 0 ? _recycledIds.Dequeue() : _nextEntityId++;
            var entity = new EntidadPadre(id);
            _entities.Add(entity);
            return entity;
        }

        /// <summary>
        /// Removes an entity from the world
        /// </summary>
        public void RemoveEntity(EntidadPadre entity)
        {
            _entities.Remove(entity);
            _recycledIds.Enqueue(entity.Id);
        }

        /// <summary>
        /// Gets all entities in the world
        /// </summary>
        public List<EntidadPadre> GetAllEntities()
        {
            return _entities;
        }

        /// <summary>
        /// Gets entities that have specific components
        /// </summary>
        public List<EntidadPadre> GetEntitiesWithComponents<T1>()
        {
            return _entities.Where(e => e.HasComponent<T1>()).ToList();
        }

        /// <summary>
        /// Gets entities that have two specific components
        /// </summary>
        public List<EntidadPadre> GetEntitiesWithComponents<T1, T2>()
        {
            return _entities.Where(e => e.HasComponent<T1>() && e.HasComponent<T2>()).ToList();
        }

        /// <summary>
        /// Gets entities that have three specific components
        /// </summary>
        public List<EntidadPadre> GetEntitiesWithComponents<T1, T2, T3>()
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
        void Initialize(World world);
    }
}