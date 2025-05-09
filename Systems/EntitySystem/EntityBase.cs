using System;
using System.Collections.Generic;
using SpookyCore.BehaviourTree;
using SpookyCore.SystemLoader;
using SpookyCore.Utilities;
using UnityEngine;

namespace SpookyCore.EntitySystem
{
    public abstract class EntityBase : MonoBehaviour, IPoolable
    {
        #region Events

        public event Action<EntityStateEvent> OnEntityStateChanged;

        #endregion

        #region Structs & Enums

        public enum EntityState
        {
            Default,
            Spawned,
            Alive,
            Dead
        }

        #endregion

        #region Fields

        [field: SerializeField] public EntityID ID { get; private set; }
        [field: SerializeField] public EntityState State { get; private set; }
        [SerializeField] private bool _useEventAwakeAndStart = false;
        [SerializeField] private List<EntityComponentBase> _componentsList = new();  
        
        private Dictionary<Type, EntityComponentBase> _componentsDict = new();
        public AIContext AIContext;
        
        private EntityStateEvent _entityStateEvent;
        private PoolSystem _poolSystem;

        #endregion

        #region Life Cycle
        // State & events:           Spawned(Event)       Alive(Event)               Dead            (Event)
        // Life Cycle:        OnAwake              OnStart            On[Fixed]Update    OnBeforeDead       OnAfterDead
        // Pool:     OnPoolGet                                                                                         OnPoolReturn
        
        private void Awake()
        {
            if (!_useEventAwakeAndStart) return;
            OnAwake();
            SetState(EntityState.Spawned);
        }
        
        private void Start()
        {
            if (!_useEventAwakeAndStart) return;
            OnStart();
            SetState(EntityState.Alive);
        }

        private void Update()
        {
            if (State != EntityState.Alive) return;
            OnUpdate();
        }
        
        private void FixedUpdate()
        {
            if (State != EntityState.Alive) return;
            OnFixedUpdate();
        }
        
        /// <summary>
        /// Called after the object is retrieved from the pool.
        /// </summary>
        public void OnGettingFromPool(bool getPreviewVersion)
        {
            OnAwake();
            if (getPreviewVersion) return;

            SetState(EntityState.Spawned);
            OnStart();
            SetState(EntityState.Alive);
        }

        /// <summary>
        /// Called before the object is returned to the pool.
        /// </summary>
        public void OnReturningToPool() { }
        
        /// <summary>
        /// Called after OnGettingFromPool.
        /// </summary>
        protected virtual void OnAwake()
        {
            Init();
            CacheComponents();
            
            foreach (var component in _componentsList)
            {
                component.Init(this);
                component.OnAwake();
            }
        }

        /// <summary>
        /// Called after changing state to Spawned and firing the Spawned state change event.
        /// </summary>
        protected virtual void OnStart()
        {
            foreach (var component in _componentsList)
            {
                component.OnStart();
            }
        }

        /// <summary>
        /// Called after changing state to Alive and firing the Alive state change event.
        /// </summary>
        protected virtual void OnUpdate()
        {
            foreach (var component in _componentsList)
            {
                component.OnUpdate();
            }
        }

        /// <summary>
        /// Called after changing state to Alive and firing the Alive state change event.
        /// </summary>
        protected virtual void OnFixedUpdate()
        {
            foreach (var component in _componentsList)
            {
                component.OnFixedUpdate();
            }
        }

        /// <summary>
        /// Called after changing state to Dead and before firing the Dead state change event.
        /// </summary>
        protected virtual void OnBeforeDead()
        {
            foreach (var component in _componentsList)
            {
                if (!component.enabled) continue;
                component.OnBeforeDead();
            }
        }

        /// <summary>
        /// Called after changing state to Dead and firing the Dead state change event.
        /// </summary>
        protected virtual void OnAfterDead()
        {
            foreach (var component in _componentsList)
            {
                if (!component || !component.enabled) continue;
                component.OnAfterDead();
            }
            
            _poolSystem?.Return(ID, this);
        }

        #endregion

        #region Public Methods

        public T Get<T>() where T : EntityComponentBase
        {
            var componentType = typeof(T);

            if (_componentsDict.TryGetValue(componentType, out var component))
            {
                return component as T;
            }

            return null;
        }

        public bool TryGet<T>(out T component) where T : EntityComponentBase
        {
            var componentType = typeof(T);

            if (_componentsDict.TryGetValue(componentType, out var componentBase))
            {
                component = componentBase as T;
                return true;
            }

            component = null;
            return false;
        }

        public bool AddComponent(EntityComponentBase component)
        {
            if (component == null)
            {
                return false;
            }
            var componentType = component.GetType();

            if (!_componentsDict.ContainsKey(componentType))
            {
                _componentsDict[componentType] = component;
                RefreshComponentsList();
            }

            return true;
        }

        public void RemoveComponent(EntityComponentBase component)
        {
            var componentType = component.GetType();
            
            if (_componentsDict.ContainsKey(componentType))
            {
                _componentsDict.Remove(componentType);
                RefreshComponentsList();
            }
        }

        public void RefreshComponentsList()
        {
            _componentsDict.Clear();
            _componentsDict = new Dictionary<Type, EntityComponentBase>();
            var components = GetComponents<EntityComponentBase>();
            foreach (var c in components)
            {
                _componentsDict.Add(c.GetType(), c);
            }
            
            _componentsList.Clear();
            _componentsList = new List<EntityComponentBase>();
            foreach (var c in _componentsDict.Values)
            {
                _componentsList.Add(c);
            }
        }

        public void SetState(EntityState newState)
        {
            if (newState == State) return;

            switch (newState)
            {
                case EntityState.Spawned:
                    break;
                case EntityState.Alive:
                    break;
                case EntityState.Dead:
                    OnBeforeDead();
                    break;
            }

            _entityStateEvent.Overload(State, newState);
            State = newState;
            OnEntityStateChanged?.Invoke(_entityStateEvent);

            if (State == EntityState.Dead)
            {
                OnAfterDead();
            }
        }

        public virtual PlayerData SaveGameData()
        {
            return null;
        }

        public virtual void LoadGameData(PlayerData playerData)
        {
            
        }

        #endregion

        #region Private Methods

        private void Init()
        {
            if (ID == EntityID.MISSING_ID)
            {
                Debug.LogError($"Unassigned Entity ID in {name}");
            }
            
            _entityStateEvent = new EntityStateEvent(EntityState.Default, EntityState.Default);
            _poolSystem = PoolSystem.Instance;
        }
        
        private void CacheComponents()
        {
            var i = 0;
            while (i < _componentsList.Count)
            {
                var component = _componentsList[i];
                if (!component.enabled)
                {
                    Debug.Log($"Component {component.name} was manually disabled on entity {name}. It may cause some errors.");
                    continue;
                }
                if (!AddComponent(component))
                {
                    _componentsList.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }

        #endregion
    }
}