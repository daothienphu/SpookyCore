namespace SpookyCore.EntitySystem
{
    public interface IAttackPhase
    {
        void StartPhase(EntityAttack owner, Entity target = null);
        
        bool UpdatePhase(EntityAttack owner);
        
        void StopPhase(EntityAttack owner);
    }
}