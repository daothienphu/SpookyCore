using UnityEngine;

namespace SpookyCore.EntitySystem
{
    [CreateAssetMenu(menuName = "SpookyCore/Status Effects/Stun Effect Config", fileName = "Stun_EffectConfig")]
    public class StunStatusEffectConfig : StatusEffectConfig
    {
        public override StatusEffectHandlerBase CreateEffectHandler()
        {
            return new StunStatusEffectHandler(this);
        }
    }

    public class StunStatusEffectHandler : StatusEffectHandlerBase
    {
        public StunStatusEffectHandler(StatusEffectConfig config) : base(config) { }

        public override void ApplyEffect(EntityStatusEffectReceiver receiver)
        {
            base.ApplyEffect(receiver);
            
            //Debug.Log($"{Receiver.name} is stunned!");
            Receiver.GetComponent<EntityMovement>()?.ToggleMovement(false);
        }

        public override void RemoveEffect()
        {
            //Debug.Log($"{Receiver.name} is no longer stunned!");
            Receiver.GetComponent<EntityMovement>()?.ToggleMovement(true);
        }
    }
}