using System;

namespace SpookyCore.EntitySystem
{
    public interface IAnimationState
    {
        void Enter(Entity entity, float transitionTime);
        void Update();
        void Exit();
        void RegisterOnExitCallback(Action onExitCallback);
    }
}