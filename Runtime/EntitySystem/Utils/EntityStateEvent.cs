﻿using SpookyCore.SystemLoader;

namespace SpookyCore.EntitySystem
{
    public class EntityStateEvent : GameEventContext
    {
        public Entity.EntityState OldState;
        public Entity.EntityState NewState;

        public EntityStateEvent(Entity.EntityState oldState, Entity.EntityState newState)
        {
            OldState = oldState;
            NewState = newState;
        }

        public void Overload(Entity.EntityState oldState, Entity.EntityState newState)
        {
            OldState = oldState;
            NewState = newState;
        }
    }
}