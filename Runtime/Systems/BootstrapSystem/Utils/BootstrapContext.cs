using System;
using System.Collections.Generic;

namespace SpookyCore.Runtime.Systems
{
    public class BootstrapContext
    {
        private readonly Dictionary<Type, object> _services = new();

        public void Register<T>(T instance) where T : class => _services[typeof(T)] = instance;
        public T Resolve<T>() where T : class => (T)_services[typeof(T)];
        public bool TryResolve<T>(out T instance) where T : class => (instance = Resolve<T>()) != null;
        public bool Has<T>() where T : class => _services.ContainsKey(typeof(T));
        public bool Has(Type type) => _services.ContainsKey(type);
    }

}