using System.Threading.Tasks;
using SpookyCore.Runtime.EntitySystem;
using UnityEngine;

namespace SpookyCore.Runtime.Systems
{
    public class GlobalAssetRegistry : PersistentMonoSingleton<GlobalAssetRegistry>, IBootstrapSystem
    {
        [SerializeField] private EntityPrefabRegistry _prefabs;
        [SerializeField] private ScriptableObjectRegistry _scriptableObjects;

        public GameObject GetPrefab(EntityID key) => Instance._prefabs.Get(key);

        public bool TryGetPrefab(EntityID key, out GameObject prefab)
        {
            prefab = GetPrefab(key);
            return prefab;
        }
        public T GetSO<T>(string key) where T : ScriptableObject => Instance._scriptableObjects.Get<T>(key);
        
        public Task OnBootstrapAsync(BootstrapContext context)
        {
            Debug.Log("<color=cyan>[Global Asset Registry]</color> ready.");
            return Task.CompletedTask;
        }
    }
}