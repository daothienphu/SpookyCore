namespace SpookyCore.EntitySystem
{
    public interface IStatusEffectHandler
    {
        void ApplyEffect(EntityStatusEffectReceiver receiver);
        bool UpdateEffect(float deltaTime);
        void RemoveEffect();
    }
}