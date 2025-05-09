namespace SpookyCore.EntitySystem
{
    public interface IAttackPhase
    {
        void StartPhase(EntityAttack owner, EntityBase target = null);
        
        bool UpdatePhase(EntityAttack owner);
        
        void StopPhase(EntityAttack owner);
    }
}