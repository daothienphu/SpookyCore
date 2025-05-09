using System;

namespace SpookyCore.SystemLoader
{
    public interface IEventBus : IService
    {
        void Subscribe<T>(Action<T> listener) where T : GameEventContext;
        void Unsubscribe<T>(Action<T> listener) where T : GameEventContext;
        void Publish(GameEventContext gameEvent);
    }
}