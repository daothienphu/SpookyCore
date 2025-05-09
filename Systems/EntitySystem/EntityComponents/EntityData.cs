
using UnityEngine;

namespace SpookyCore.EntitySystem
{
    public class EntityData : EntityComponentBase
    {
        #region Fields

        public EntityDataConfig DataConfig;
        public EntityDataModel DefaultStats;
        public EntityDataModel BoostedStats;
        public EntityDataModel FinalStats;

        #endregion

        #region Life Cycle

        public override void OnAwake()
        {
            SetDefaultStats();
            SetFinalStats();
        }

        #endregion

        #region Public Methods

        // public void ApplyTemporaryStatBoost(TemporaryStatus temporaryStatus, float amount, bool isPercentage = false)
        // {
        //     switch (temporaryStatus)
        //     {
        //         case TemporaryStatus.MaxHealth:
        //             BoostedStats.MaxHealth = isPercentage 
        //                 ? DefaultStats.MaxHealth * amount 
        //                 : amount; 
        //             break;
        //         case TemporaryStatus.Damage:
        //             BoostedStats.Damage = isPercentage 
        //                 ? DefaultStats.Damage * amount 
        //                 : amount; 
        //             break;
        //         case TemporaryStatus.MovementSpeed:
        //             BoostedStats.MovementSpeed = isPercentage 
        //                 ? DefaultStats.MovementSpeed * amount 
        //                 : amount; 
        //             break;
        //         default:
        //             throw new ArgumentOutOfRangeException(nameof(temporaryStatus), temporaryStatus, null);
        //     }
        //     
        //     SetFinalStats();
        // }
        //
        // public void RevokeStatBoost(TemporaryStatus temporaryStatus)
        // {
        //     switch (temporaryStatus)
        //     {
        //         case TemporaryStatus.MaxHealth:
        //             BoostedStats.MaxHealth = 0;
        //             break;
        //         case TemporaryStatus.Damage:
        //             BoostedStats.Damage = 0;
        //             break;
        //         case TemporaryStatus.MovementSpeed:
        //             BoostedStats.MovementSpeed = 0;
        //             break;
        //         default:
        //             throw new ArgumentOutOfRangeException(nameof(temporaryStatus), temporaryStatus, null);
        //     }
        //     
        //     SetFinalStats();
        // }

        #endregion

        #region Private Methods

        private void SetDefaultStats()
        {
            if (!DataConfig)
            {
                Debug.LogError($"Please assign a data config to {Entity.name}");
                return;
            }
            DefaultStats.MaxHealth = DataConfig.MaxHealth;
            DefaultStats.MaxArmor = DataConfig.MaxArmor;
            DefaultStats.Damage = DataConfig.Damage;
            
            DefaultStats.MovementSpeed = DataConfig.MovementSpeed;
            DefaultStats.AttackSpeed = DataConfig.AttackSpeed;
            
            DefaultStats.AttackRange = DataConfig.AttackRange;
            DefaultStats.DetectionRange = DataConfig.DetectionRange;
            
            DefaultStats.ComfortableLowerLimit = DataConfig.ComfortableLowerLimit;
            DefaultStats.ComfortableUpperLimit = DataConfig.ComfortableUpperLimit;
        }

        private void SetFinalStats()
        {
            FinalStats.MaxHealth = DefaultStats.MaxHealth + BoostedStats.MaxHealth;
            FinalStats.MaxArmor = DefaultStats.MaxArmor + BoostedStats.MaxArmor;
            FinalStats.Damage = DefaultStats.Damage + BoostedStats.Damage;
            
            FinalStats.MovementSpeed = DefaultStats.MovementSpeed + BoostedStats.MovementSpeed;
            FinalStats.AttackSpeed = DefaultStats.AttackSpeed + BoostedStats.AttackSpeed;
            
            FinalStats.AttackRange = DefaultStats.AttackRange + BoostedStats.AttackRange;
            FinalStats.DetectionRange = DefaultStats.DetectionRange + BoostedStats.DetectionRange;
            FinalStats.ComfortableLowerLimit = DefaultStats.ComfortableLowerLimit + BoostedStats.ComfortableLowerLimit;
            FinalStats.ComfortableUpperLimit = DefaultStats.ComfortableUpperLimit + BoostedStats.ComfortableUpperLimit;
        }
        
        #endregion
    }
}