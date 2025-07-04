using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace SpookyCore.Runtime.Systems
{
    public class GameBootstrapper : MonoBehaviour
    {
        [SerializeField] private SystemRegistry _systemRegistry;
        [SerializeField] private bool _switchToTestUI;

        private async void Awake()
        {
            try
            {
                var context = new BootstrapContext();

                var allSystemTypes = DiscoverAllSystems();
                var sortedTypes = BootstrapSort.TopoSort(allSystemTypes);

                foreach (var type in sortedTypes)
                {
                    if (context.Has(type))
                    {
                        continue;
                    }

                    var deps = BootstrapSort.GetDependencies(type);
                    if (deps.Any(d => !context.Has(d)))
                    {
                        Debug.LogError($"Cannot bootstrap {type.Name}, unresolved dependencies.");
                        continue;
                    }

                    IBootstrapSystem instance;

                    if (typeof(MonoBehaviour).IsAssignableFrom(type))
                    {
                        instance = InstantiatePrefabSystem(type);
                    }
                    else
                    {
                        instance = Activator.CreateInstance(type) as IBootstrapSystem;
                    }

                    if (instance != null)
                    {
                        context.Register(instance);
                        await instance.OnBootstrapAsync(context);
                    }
                }

                OnBootstrapComplete();
            }
            catch (Exception e)
            {
                // TODO handle exception
                Debug.LogError(e.ToString());
            }
        }

        private List<Type> DiscoverAllSystems()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => 
                    typeof(IBootstrapSystem).IsAssignableFrom(t) && 
                    !t.IsAbstract && 
                    t.GetConstructor(Type.EmptyTypes) != null)
                .ToList();
        }
        
        private IBootstrapSystem InstantiatePrefabSystem(Type type)
        {
            var prefab = _systemRegistry.GetPrefabForType(type);
            if (!prefab)
            {
                Debug.LogError($"<color=cyan>[Game Bootstrapper]</color> Missing prefab for {type.Name}");
                return null;
            }

            var instance = Instantiate(prefab);
            return (IBootstrapSystem)instance.GetComponent(type);
        }

        private void OnBootstrapComplete()
        {
            Debug.Log("<color=cyan>[Game Bootstrapper]</color> Finished bootstrapping systems.");
            
            if (!SceneFlowSystem.Instance) Debug.LogError("<color=cyan>[Game Bootstrapper]</color> Please provide a SceneFlowSystem to switch between scenes.");
            SceneFlowSystem.Instance?.SwitchToSceneAsync(_switchToTestUI ? SceneID.UITest : SceneID.Gameplay);
        }
    }
}