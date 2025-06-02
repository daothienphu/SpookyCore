using System;
using System.Collections.Generic;

namespace SpookyCore.Runtime.Systems
{
    public class EventBus : IEventBus
    {
        private static readonly Dictionary<Type, List<Action<GameEventContext>>> _eventListeners = new();

        public void Subscribe<T>(Action<T> listener) where T : GameEventContext
        {
            var eventType = typeof(T);
            if (!_eventListeners.ContainsKey(eventType))
            {
                _eventListeners[eventType] = new List<Action<GameEventContext>>();
            }

            _eventListeners[eventType].Add(ctx => listener((T)ctx));
        }

        public void Unsubscribe<T>(Action<T> listener) where T : GameEventContext
        {
            var eventType = typeof(T);
            
            if (!_eventListeners.ContainsKey(eventType)) return;
            
            _eventListeners[eventType].RemoveAll(e => e == listener);
            if (_eventListeners[eventType].Count == 0)
            {
                _eventListeners.Remove(eventType);
            }
        }

        public void Publish(GameEventContext gameEvent)
        {
            var eventType = gameEvent.GetType();
            
            if (!_eventListeners.ContainsKey(eventType)) return;
            
            foreach (var listener in _eventListeners[eventType])
            {
                listener(gameEvent);
            }
        }
    }
}