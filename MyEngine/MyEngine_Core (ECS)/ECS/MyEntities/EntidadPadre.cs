using System;
using System.Collections.Generic;

namespace MyEngine_Core.ECS.MyEntities
{
    public class EntidadPadre
    {
        public readonly int Id;

        // Diccionario para guardar componentes por tipo
        private Dictionary<Type, object> _components = new();
        // Verificar si tiene un componente
        public EntidadPadre(int id)
        {
            Id = id;
        }

        // Agregar componente
        public void AddComponent<T>(T component)
        {
            _components[typeof(T)] = component;
        }

        // Verificar si tiene un componente
        public bool HasComponent<T>()
        {
            return _components.ContainsKey(typeof(T));
        }

        // Obtener un componente
        public T GetComponent<T>()
        {
            if (_components.TryGetValue(typeof(T), out var comp))
                return (T)comp;

            throw new Exception($"La entidad no tiene el componente {typeof(T).Name}");
        }
    }
}
