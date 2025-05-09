using SpookyCore.SystemLoader;

namespace SpookyCore.EntitySystem
{
    public class EntityStateEvent : GameEventContext
    {
        public EntityBase.EntityState OldState;
        public EntityBase.EntityState NewState;

        public EntityStateEvent(EntityBase.EntityState oldState, EntityBase.EntityState newState)
        {
            OldState = oldState;
            NewState = newState;
        }

        public void Overload(EntityBase.EntityState oldState, EntityBase.EntityState newState)
        {
            OldState = oldState;
            NewState = newState;
        }
    }
}