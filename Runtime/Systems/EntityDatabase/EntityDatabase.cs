using SpookyCore.Utilities;
using UnityEngine;

namespace SpookyCore.SystemLoader.SubSystems.EntityDatabase
{
    public class EntityDatabase : PersistentMonoSingleton<EntityDatabase>, IGameSystem
    {
        [field: SerializeField] public EntityPrefabsConfig Prefabs;

        protected override void OnStart()
        {
            base.OnStart();
            Debug.Log("<color=cyan>[Entity Database]</color> system ready.");
        }
    }
}