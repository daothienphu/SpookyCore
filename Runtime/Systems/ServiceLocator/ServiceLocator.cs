using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace SpookyCore.Runtime.Systems
{
    public class ServiceLocator : PureSingleton<ServiceLocator>, IBootstrapSystem
    {
        #region Fields

        private static readonly Dictionary<Type, IService> _services = new();

        #endregion

        #region Life Cycle

        public Task OnBootstrapAsync(BootstrapContext context)
        {
            Register<IEventBus>(new EventBus());
            Debug.Log("<color=cyan>[Service Locator System]</color> ready.");
            
            return Task.CompletedTask;
        }

        #endregion

        #region Public Methods

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

        #endregion
    }
}
