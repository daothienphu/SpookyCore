using System.Threading.Tasks;
using SpookyCore.Runtime.EntitySystem;
using UnityEngine;

namespace SpookyCore.Runtime.Systems
{
    public class GlobalAssetRegistry : PersistentMonoSingleton<GlobalAssetRegistry>, IBootstrapSystem
    {
        [SerializeField] private EntityPrefabRegistry _entityPrefabRegistry;
        [SerializeField] private ScriptableObjectRegistry _soRegistry;

        public GameObject GetPrefab(EntityID key) => Instance._entityPrefabRegistry.Get(key);

        public bool TryGetPrefab(EntityID key, out GameObject prefab)
        {
            prefab = GetPrefab(key);
            return prefab != null;
        }
        public T GetSO<T>(string key) where T : ScriptableObject => Instance._soRegistry.Get<T>(key);
        
        public Task OnBootstrapAsync(BootstrapContext context)
        {
            Debug.Log("<color=cyan>[Global Asset Registry]</color> system ready.");
            return Task.CompletedTask;
        }
    }
}