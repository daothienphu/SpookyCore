using UnityEngine;

namespace SpookyCore.Runtime.EntitySystem
{
    [CreateAssetMenu(menuName = "SpookyCore/Entity System/Stat/Entity Stat Config", fileName = "Entity_StatConfig")]
    public class EntityStatConfig : ScriptableObject
    {
        [field: SerializeField] public Stat Health { get; private set; }
        [field: SerializeField] public Stat Armor { get; private set; }
        [field: SerializeField] public Stat Damage { get; private set; }
        [field: SerializeField] public Stat WalkSpeed { get; private set; }
        [field: SerializeField] public Stat RunSpeed { get; private set; }
        [field: SerializeField] public Stat VisionRange { get; private set; }
        [field: SerializeField] public Stat AttackRange { get; private set; }
    }
}