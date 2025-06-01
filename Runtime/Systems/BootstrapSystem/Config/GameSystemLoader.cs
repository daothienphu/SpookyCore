using UnityEngine;

namespace SpookyCore.SystemLoader
{
    public abstract class GameSystemLoader : ScriptableObject
    {
        [field: SerializeField] public GameObject SystemPrefab;
    }
}