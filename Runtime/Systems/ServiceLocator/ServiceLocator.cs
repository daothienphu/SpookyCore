using System;
using System.Collections.Generic;
using SpookyCore.Utilities;
using UnityEngine;

namespace SpookyCore.SystemLoader
{
    public class ServiceLocator : PersistentMonoSingleton<ServiceLocator>, IGameSystem
    {
        private static readonly Dictionary<Type, IService> _services = new();

        private void Start()
        {
            Register<IEventBus>(new EventBus());
            Debug.Log("<color=cyan>[Service Locator]</color> system ready.");
        }

        public void Register<T>(T service) where T : class, IService
        {
            var type = typeof(T);
            _services[type] = service;
        }
        
        public T Get<T>() where T : class, IService
        {
            var type = typeof(T);
            if (_services.TryGetValue(type, out var service))
            {
                return service as T;
            }
            throw new Exception($"Service of type <color=cyan>{type}</color> not registered.");
        }
        
        public void Unregister<T>() where T : class, IService
        {
            var type = typeof(T);
            if (_services.ContainsKey(type))
            {
                _services.Remove(type);
            }
        }
        
        public void Clear()
        {
            _services.Clear();
        }
    }
}
