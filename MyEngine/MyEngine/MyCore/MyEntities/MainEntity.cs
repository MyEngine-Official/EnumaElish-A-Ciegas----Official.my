using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEngine.MyCore.MyEntities
{
    public class MainEntity
    {
        public readonly int Id;

        /// <summary>
        /// Diccionario para guardar componentes por tipo
        /// </summary>
        private Dictionary<Type, object> _components = new();

        public MainEntity(int id)
        {
            Id = id;
        }

        /// <summary>
        /// Agregar componente
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
        public void AddComponent<T>(T component)
        {
            _components[typeof(T)] = component;
        }


        /// <summary>
        /// Verificar si tiene un componente
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool HasComponent<T>()
        {
            return _components.ContainsKey(typeof(T));
        }

        /// <summary>
        /// Obtener un componente
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public T GetComponent<T>()
        {
            if (_components.TryGetValue(typeof(T), out var comp))
                return (T)comp;

            throw new Exception($"La entidad no tiene el componente {typeof(T).Name}");
        }
    }
}
