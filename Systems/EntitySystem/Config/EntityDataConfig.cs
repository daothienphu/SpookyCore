using UnityEngine;

namespace SpookyCore.EntitySystem
{
    [CreateAssetMenu(menuName = "SpookyCore/Entity Data/Default Entity Data", fileName = "Entity_Data_Config")]
    public class EntityDataConfig : ScriptableObject
    {
        [Header("Basic Data")]
        public float MaxHealth = 90;
        public float MaxArmor = 10;
        public float Damage = 10;
        
        public float MovementSpeed = 2;
        public float AttackSpeed = 2;
        
        public float AttackRange = 2;
        public float DetectionRange = 3;
        
        public float ComfortableLowerLimit = -1;
        public float ComfortableUpperLimit = -10;
    }
    
    [System.Serializable]
    public struct EntityDataModel
    {
        public float MaxHealth;
        public float MaxArmor;
        public float Damage;
        
        public float MovementSpeed;
        public float AttackSpeed;
        
        public float AttackRange;
        public float DetectionRange;
        
        public float ComfortableLowerLimit;
        public float ComfortableUpperLimit;
    }
    
    [System.Serializable]
    public enum TemporaryStatus
    {
        MaxHealth,
        MaxArmor,
        Damage,
        
        MovementSpeed,
        AttackSpeed,
        
        AttackRange,
        DetectionRange,
        
        ComfortableLowerLimit,
        ComfortableUpperLimit,
    }
}