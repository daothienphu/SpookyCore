using UnityEngine;

namespace SpookyCore.EntitySystem.Utils.Stat
{
    [CreateAssetMenu(menuName = "SpookyCore/Components/Stat/Entity Stat Config", fileName = "EntityStat_Config")]
    public class EntityStatConfig : ScriptableObject
    {
        [field: SerializeField] public Stat Health { get; private set; }
        [field: SerializeField] public Stat Armor { get; private set; }
        [field: SerializeField] public Stat Damage { get; private set; }
    }
}