using System;
using System.Collections.Generic;
using SpookyCore.Runtime.AI;
using SpookyCore.Runtime.Systems;
using SpookyCore.Runtime.Utilities;
using UnityEngine;

namespace SpookyCore.Runtime.EntitySystem
{
    public class Entity : MonoBehaviour, IPoolable
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
        [ReadOnly]
        [field: SerializeField] public EntityState State { get; private set; }
        [Tooltip("Use Unity Awake and Start event calls, used when object is placed in the scene instead of spawned from a pool.")]
        [SerializeField] private bool _useUnityAwakeAndStart;
        [SerializeField] private List<EntityComponent> _componentsList = new();  
        
        private Dictionary<Type, EntityComponent> _componentsDict = new();
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
            if (!_useUnityAwakeAndStart) return;
            OnAwake();
            SetState(EntityState.Spawned);
        }
        
        private void Start()
        {
            if (!_useUnityAwakeAndStart) return;
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
            if (ID == EntityID.MISSING_ID)
            {
                Debug.LogError($"Unassigned Entity ID in {name}");
            }

            _entityStateEvent = new EntityStateEvent(EntityState.Default, EntityState.Default);
            _poolSystem = PoolSystem.Instance;
            
            RefreshComponentsCache();
            
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

        public T Get<T>() where T : EntityComponent
        {
            var componentType = typeof(T);

            if (_componentsDict.TryGetValue(componentType, out var component))
            {
                return component as T;
            }

            return null;
        }

        public bool TryGet<T>(out T component) where T : EntityComponent
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

        public void RefreshComponentsCache()
        {
            _componentsDict.Clear();
            _componentsDict = new Dictionary<Type, EntityComponent>();
            var components = GetComponents<EntityComponent>();
            foreach (var c in components)
            {
                _componentsDict.Add(c.GetType(), c);
            }
            
            _componentsList.Clear();
            _componentsList = new List<EntityComponent>();
            foreach (var c in _componentsDict.Values)
            {
                _componentsList.Add(c);
            }
        }

        public List<EntityComponent> GetAllComponents()
        {
            return _componentsList;
        }

        public void SetState(EntityState newState)
        {
            if (newState == State) return;

            switch (newState)
            {
                case EntityState.Spawned:
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

        // public virtual PlayerData SaveGameData()
        // {
        //     return null;
        // }
        //
        // public virtual void LoadGameData(PlayerData playerData)
        // {
        //     
        // }

        #endregion
    }
}