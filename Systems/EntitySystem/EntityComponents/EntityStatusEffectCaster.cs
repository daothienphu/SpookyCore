using SpookyCore.Utilities.Editor.Gizmos;
using UnityEngine;

namespace SpookyCore.EntitySystem
{
    public class EntityStatusEffectCaster : EntityComponentBase
    {
        [SerializeField] private StatusEffectConfig _statusEffectConfig;
        private float _timer;
        private Vector2 _center;
        
        #region Life Cycle

        public override void OnUpdate()
        {
            if (_timer > 0)
            {
                _timer -= Time.deltaTime;
            }
        }

        #endregion
        
        #region Public Methods

        public void CastEffect(Vector2 center, float orientation = 0, StatusEffectConfig statusEffectConfig = null)
        {
            _timer = 2f;
            _center = center;
            var effectConfig = statusEffectConfig ?? _statusEffectConfig;
            var targets = effectConfig.GetAreaCast().GetTargets(center, effectConfig.EffectSize, orientation);
            
            foreach (var target in targets)
            {
                if (target == Entity)
                {
                    continue;
                }
                
                if (target.TryGet<EntityStatusEffectReceiver>(out var receiver))
                {
                    receiver.ApplyEffect(effectConfig);
                }
            }
        }

        #endregion

        #region Private Methods

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!Entity) return;
            
            var originalColor = Gizmos.color;

            if (GlobalGizmoController.GetGizmoState("Entities/Status Effect Cast"))
            {
                Gizmos.color = Color.blue;
                if (_timer > 0)
                {
                    switch (_statusEffectConfig.EffectShape)
                    {
                        case StatusEffectConfig.AreaShape.Circle:
                            Gizmos.DrawWireSphere(_center, _statusEffectConfig.EffectSize.x);
                            break;
                        case StatusEffectConfig.AreaShape.Rectangle:
                            break;
                        case StatusEffectConfig.AreaShape.Cone:
                            break;
                    }
                }
            }
            
            Gizmos.color = originalColor;
        }
#endif
        #endregion
    }
}