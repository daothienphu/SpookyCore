namespace SpookyCore.EntitySystem
{
    public abstract class StatusEffectHandlerBase : IStatusEffectHandler
    {
        protected EntityStatusEffectReceiver Receiver;
        public readonly StatusEffectConfig Config;
        
        protected float RemainingDuration;

        protected StatusEffectHandlerBase(StatusEffectConfig config)
        {
            Config = config;
        }
        
        public virtual void ApplyEffect(EntityStatusEffectReceiver receiver)
        {
            Receiver = receiver;
            
            RemainingDuration = Config.Duration;
        }

        public virtual bool UpdateEffect(float deltaTime)
        {
            RemainingDuration -= deltaTime;
            return RemainingDuration <= 0;
        }

        public virtual void RemoveEffect() { }
    }
}