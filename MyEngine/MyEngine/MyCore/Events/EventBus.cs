using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEngine.MyCore.Events
{
    public class EventBus
    {
        private readonly Dictionary<Type, List<object>> _handlers = new();
        private readonly Queue<GameEvent> _eventQueue = new();
        private readonly Dictionary<Type, int> _eventStats = new();
        private bool _isProcessingEvents = false;

        // Debug/Stats
        public bool EnableDebugLogging { get; set; } = false;
        public Dictionary<Type, int> EventStats => new(_eventStats);

        // ================================
        // SUSCRIPCIÓN A EVENTOS
        // ================================
        public void Subscribe<T>(Action<T> handler) where T : GameEvent
        {
            var eventType = typeof(T);
            if (!_handlers.ContainsKey(eventType))
                _handlers[eventType] = new List<object>();

            _handlers[eventType].Add(handler);

            if (EnableDebugLogging)
                Console.WriteLine($"[EventBus] Subscribed to {eventType.Name}. Total handlers: {_handlers[eventType].Count}");
        }

        public void Unsubscribe<T>(Action<T> handler) where T : GameEvent
        {
            var eventType = typeof(T);
            if (_handlers.ContainsKey(eventType))
            {
                _handlers[eventType].Remove(handler);
                if (EnableDebugLogging)
                    Console.WriteLine($"[EventBus] Unsubscribed from {eventType.Name}. Total handlers: {_handlers[eventType].Count}");
            }
        }

        // ================================
        // PUBLICACIÓN INMEDIATA
        // ================================
        public void Publish<T>(T gameEvent) where T : GameEvent
        {
            if (_isProcessingEvents)
            {
                // Si estamos procesando eventos, encolar para evitar recursión infinita
                _eventQueue.Enqueue(gameEvent);
                return;
            }

            var eventType = typeof(T);

            // Estadísticas
            if (_eventStats.ContainsKey(eventType))
                _eventStats[eventType]++;
            else
                _eventStats[eventType] = 1;

            if (EnableDebugLogging)
                Console.WriteLine($"[EventBus] Publishing {eventType.Name}");

            if (_handlers.ContainsKey(eventType))
            {
                foreach (Action<T> handler in _handlers[eventType])
                {
                    try
                    {
                        handler.Invoke(gameEvent);
                        if (gameEvent.IsHandled) break; // Permite que eventos se marquen como "manejados"
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[EventBus] Error handling {eventType.Name}: {ex.Message}");
                    }
                }
            }
        }

        // ================================
        // PUBLICACIÓN ENCOLADA
        // ================================
        public void QueueEvent<T>(T gameEvent) where T : GameEvent
        {
            _eventQueue.Enqueue(gameEvent);
        }

        public void ProcessQueuedEvents()
        {
            _isProcessingEvents = true;

            while (_eventQueue.Count > 0)
            {
                var gameEvent = _eventQueue.Dequeue();
                var eventType = gameEvent.GetType();

                // Usar reflexión para llamar Publish con el tipo correcto
                var publishMethod = GetType().GetMethod(nameof(Publish));
                var genericPublish = publishMethod.MakeGenericMethod(eventType);
                genericPublish.Invoke(this, new object[] { gameEvent });
            }

            _isProcessingEvents = false;
        }

        // ================================
        // UTILIDADES
        // ================================
        public void Clear()
        {
            _handlers.Clear();
            _eventQueue.Clear();
            _eventStats.Clear();
        }

        public int GetSubscriberCount<T>() where T : GameEvent
        {
            var eventType = typeof(T);
            return _handlers.ContainsKey(eventType) ? _handlers[eventType].Count : 0;
        }

        public void PrintStats()
        {
            Console.WriteLine("\n=== EventBus Statistics ===");
            foreach (var kvp in _eventStats)
            {
                Console.WriteLine($"{kvp.Key.Name}: {kvp.Value} events");
            }
            Console.WriteLine($"Total subscribers: {_handlers.Sum(h => h.Value.Count)}");
            Console.WriteLine($"Queued events: {_eventQueue.Count}");
        }
    }
}
